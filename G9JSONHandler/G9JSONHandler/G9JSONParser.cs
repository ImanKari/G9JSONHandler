using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using G9AssemblyManagement.DataType;
using G9AssemblyManagement.Enums;
using G9AssemblyManagement.Helper;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;
using G9JSONHandler.Common;

namespace G9JSONHandler
{
    /// <summary>
    ///     A pretty small library for JSON
    ///     A static helper class for parsing JSON
    /// </summary>
    public static class G9JsonParser
    {
        [ThreadStatic] private static Stack<List<string>> _splitArrayPool;
        [ThreadStatic] private static StringBuilder _stringBuilder;

        /// <summary>
        ///     Method to convert a JSON string to an Object.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object.</typeparam>
        /// <param name="json">Specifies JSON string for conversion.</param>
        /// <returns>An object that is converted by JSON string.</returns>
        public static T G9JsonToObject<T>(this string json)
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder();
            if (_splitArrayPool == null) _splitArrayPool = new Stack<List<string>>();

            //Remove all whitespace not within strings to make parsing simpler
            _stringBuilder.Length = 0;
            var ignoreComments = false;

            for (var i = 0; i < json.Length; i++)
            {
                // End of comment
                if (json[i] == '/' && json[i - 1] == '*')
                {
                    ignoreComments = false;
                    continue;
                }

                // Ignore text between comments signs
                if (ignoreComments) continue;

                // Start of comment
                if (json[i] == '/' && json[i + 1] == '*')
                {
                    ignoreComments = true;
                    continue;
                }

                var c = json[i];
                // It recognizes that the data is a string.
                if (c == '"')
                {
                    // The string between signs is caught and continues after the index of the string sign.
                    i = CatchStringBetweenSigns(true, i, json);
                    continue;
                }

                // Ignore white space
                if (char.IsWhiteSpace(c))
                    continue;

                _stringBuilder.Append(c);
            }

            // The method parses the pure JSON data
            return (T)ParsePureJsonData(typeof(T), _stringBuilder.ToString());
        }

        /// <summary>
        ///     The string between signs is caught and continues after the index of the string sign.
        /// </summary>
        /// <param name="appendUniqueCharacter">Specifies whether the unique character must be stored or not.</param>
        /// <param name="startIndex">Specifies the start index in JSON string for storing data</param>
        /// <param name="json">Specifies the JSON data</param>
        /// <returns>Specifies the end index of the string</returns>
        private static int CatchStringBetweenSigns(bool appendUniqueCharacter, int startIndex, string json)
        {
            _stringBuilder.Append(json[startIndex]);
            for (var i = startIndex + 1; i < json.Length; i++)
                switch (json[i])
                {
                    case '\\':
                    {
                        if (appendUniqueCharacter)
                            _stringBuilder.Append(json[i]);
                        _stringBuilder.Append(json[i + 1]);
                        i++; //Skip next character as it is escaped
                        break;
                    }
                    // End of string
                    case '"':
                        _stringBuilder.Append(json[i]);
                        return i;
                    // String Data
                    default:
                        _stringBuilder.Append(json[i]);
                        break;
                }

            return json.Length - 1;
        }

        /// <summary>
        ///     Method to split data
        /// </summary>
        /// <param name="json">Specifies JSON data for splitting</param>
        /// <returns>A collection of split data</returns>
        private static List<string> Splitter(string json)
        {
            var splitArray = _splitArrayPool.Count > 0 ? _splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if (json.Length == 2)
                return splitArray;
            var parseDepth = 0;
            _stringBuilder.Length = 0;
            for (var i = 1; i < json.Length - 1; i++)
            {
                switch (json[i])
                {
                    case '[':
                    case '{':
                        parseDepth++;
                        break;
                    case ']':
                    case '}':
                        parseDepth--;
                        break;
                    case '"':
                        i = CatchStringBetweenSigns(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0)
                        {
                            splitArray.Add(_stringBuilder.ToString());
                            _stringBuilder.Length = 0;
                            continue;
                        }

                        break;
                }

                _stringBuilder.Append(json[i]);
            }

            splitArray.Add(_stringBuilder.ToString());

            return splitArray;
        }

        /// <summary>
        ///     The method parses the pure JSON data
        /// </summary>
        /// <param name="type">Specifies the final object type of result</param>
        /// <param name="json">The pure JSON data</param>
        /// <returns></returns>
        private static object ParsePureJsonData(Type type, string json)
        {
            if (json == "null") return null;

            if (type.IsEnum)
            {
                if (json[0] == '"')
                    json = json.Substring(1, json.Length - 2);
                try
                {
                    return Enum.Parse(type, json, false);
                }
                catch
                {
                    return 0;
                }
            }

            if (!G9CCommonHelper.IsEnumerableType(type) && type.G9IsTypeBuiltInDotNetType())
                return ParseDotNetBuiltInTypes(type, json);

            if (type.IsArray)
            {
                var arrayType = type.GetElementType();
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                var elems = Splitter(json);
                if (arrayType != null)
                {
                    var newArray = Array.CreateInstance(arrayType, elems.Count);
                    for (var i = 0; i < elems.Count; i++)
                        newArray.SetValue(ParsePureJsonData(arrayType, elems[i]), i);
                    _splitArrayPool.Push(elems);
                    return newArray;
                }
            }

            switch (type.IsGenericType)
            {
                case true when type.GetGenericTypeDefinition() == typeof(List<>):
                {
                    var listType = type.GetGenericArguments()[0];
                    if (json[0] != '[' || json[json.Length - 1] != ']')
                        return null;

                    var elems = Splitter(json);
                    var list = (IList)type.GetConstructor(new[] { typeof(int) })?.Invoke(new object[] { elems.Count });
                    foreach (var t in elems) list?.Add(ParsePureJsonData(listType, t));

                    _splitArrayPool.Push(elems);
                    return list;
                }
                case true when type.GetGenericTypeDefinition() == typeof(Dictionary<,>):
                {
                    Type keyType, valueType;
                    {
                        var args = type.GetGenericArguments();
                        keyType = args[0];
                        valueType = args[1];
                    }

                    //Refuse to parse dictionary keys that aren't of type string
                    if (keyType != typeof(string))
                        return null;
                    //Must be a valid dictionary element
                    if (json[0] != '{' || json[json.Length - 1] != '}')
                        return null;
                    //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                    var elems = Splitter(json);
                    if (elems.Count % 2 != 0)
                        return null;

                    var dictionary = (IDictionary)type.GetConstructor(new[] { typeof(int) })
                        ?.Invoke(new object[] { elems.Count / 2 });
                    for (var i = 0; i < elems.Count; i += 2)
                    {
                        if (elems[i].Length <= 2)
                            continue;
                        var keyValue = elems[i].Substring(1, elems[i].Length - 2);
                        var val = ParsePureJsonData(valueType, elems[i + 1]);
                        if (dictionary != null) dictionary[keyValue] = val;
                    }

                    return dictionary;
                }
            }

            if (type == typeof(object)) return ParseAnonymousValue(json);
            if (json[0] == '{' && json[json.Length - 1] == '}') return ParseObject(type, json);

            return null;
        }

        /// <summary>
        ///     Method to prepare data as a string
        /// </summary>
        /// <param name="json">JSON Data</param>
        /// <returns>Pure string data</returns>
        private static string PrepareStringType(string json)
        {
            if (json.Length <= 2)
                return string.Empty;
            var parseStringBuilder = new StringBuilder(json.Length);
            for (var i = 1; i < json.Length - 1; ++i)
            {
                if (json[i] == '\\' && i + 1 < json.Length - 1)
                {
                    // ReSharper disable once StringLiteralTypo
                    var j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                    if (j >= 0)
                    {
                        parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                        ++i;
                        continue;
                    }

                    if (json[i + 1] == 'u' && i + 5 < json.Length - 1)
                        if (uint.TryParse(json.Substring(i + 2, 4), NumberStyles.AllowHexSpecifier, null,
                                out var c))
                        {
                            parseStringBuilder.Append((char)c);
                            i += 5;
                            continue;
                        }
                }

                parseStringBuilder.Append(json[i]);
            }

            return parseStringBuilder.ToString();
        }

        /// <summary>
        ///     The helper method to parse .NET built-in types
        /// </summary>
        /// <param name="type">Specifies type of object item</param>
        /// <param name="json">Specifies JSON data</param>
        /// <returns>parsed object</returns>
        private static object ParseDotNetBuiltInTypes(Type type, string json)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                    return json.ToCharArray()[1];
                case TypeCode.String:
                    return PrepareStringType(json);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return Convert.ChangeType(json, type, CultureInfo.InvariantCulture);
                case TypeCode.DateTime:
                    return DateTime.Parse(json.Substring(1, json.Length - 2));
                default:
                    return ParseExceptionableTypes(type, json);
            }
        }

        /// <summary>
        ///     Method to parse exceptionable types
        /// </summary>
        /// <param name="type">Specifies type of object item</param>
        /// <param name="json">Specifies JSON data</param>
        /// <returns>parsed object</returns>
        private static object ParseExceptionableTypes(Type type, string json)
        {
            if (type.GetMethods().Any(s => s.Name == nameof(TimeSpan.Parse)))
            {
                var instance = FormatterServices.GetUninitializedObject(type);
                var method = instance
                    .G9GetMethodsOfObject(G9EAccessModifier.Everything,
                        s => s.Name == nameof(TimeSpan.Parse) && s.GetParameters().Length == 1 &&
                             s.GetParameters()[0].ParameterType == typeof(string));
                if (method.Any())
                    return method[0].CallMethodWithResult<object>(json.Substring(1, json.Length - 2));
            }

            try
            {
                return Convert.ChangeType(json, type, CultureInfo.InvariantCulture);
            }
            catch
            {
                return Convert.ChangeType(json, type);
            }
        }

        /// <summary>
        ///     The method to parse anonymous value
        /// </summary>
        /// <param name="json">Specifies JSON data to analyze</param>
        /// <returns></returns>
        private static object ParseAnonymousValue(string json)
        {
            if (json.Length == 0)
                return null;
            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                var elems = Splitter(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (var i = 0; i < elems.Count; i += 2)
                    dict[elems[i].Substring(1, elems[i].Length - 2)] = ParseAnonymousValue(elems[i + 1]);
                return dict;
            }

            if (json[0] == '[' && json[json.Length - 1] == ']')
            {
                var items = Splitter(json);
                var finalList = new List<object>(items.Count);
                finalList.AddRange(items.Select(ParseAnonymousValue));

                return finalList;
            }

            if (json[0] == '"' && json[json.Length - 1] == '"')
            {
                var str = json.Substring(1, json.Length - 2);
                return str.Replace("\\", string.Empty);
            }

            if (char.IsDigit(json[0]) || json[0] == '-')
            {
                if (json.Contains("."))
                {
                    double.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                    return result;
                }
                else
                {
                    int.TryParse(json, out var result);
                    return result;
                }
            }

            switch (json)
            {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    // handles json == "null" as well as invalid JSON
                    return null;
            }
        }

        /// <summary>
        ///     Method to create dictionary from object members
        /// </summary>
        /// <typeparam name="TType">Type of members</typeparam>
        /// <param name="dic">Reference dictionary for adding members</param>
        /// <param name="members">Object members</param>
        private static void CreateDictionaryOfMembers<TType>(
            ref Dictionary<string, G9IObjectMember> dic, IEnumerable<TType> members)
            where TType : G9IObjectMember
        {
            foreach (var m in members)
            {
                var nameAttr = m.GetCustomAttributes<G9JsonCustomMemberNameAttribute>(true);
                dic.Add(nameAttr.Any() ? nameAttr[0].Name : m.Name, m);
            }
        }

        /// <summary>
        ///     Method to convert JSON data to object
        /// </summary>
        /// <param name="type">Specifies type of object</param>
        /// <param name="json">Specifies JSON data</param>
        /// <returns>An object created by JSON data</returns>
        private static object ParseObject(Type type, string json)
        {
            var instance = FormatterServices.GetUninitializedObject(type);

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            var elems = Splitter(json);
            if (elems.Count % 2 != 0)
                return instance;


            var members = new Dictionary<string, G9IObjectMember>(StringComparer.OrdinalIgnoreCase);

            // Prepare Fields
            CreateDictionaryOfMembers(ref members,
                instance.G9GetFieldsOfObject(G9EAccessModifier.Public,
                    s => !s.GetCustomAttributes(typeof(G9JsonIgnoreMemberAttribute), true).Any()));

            // Prepare Properties
            CreateDictionaryOfMembers(ref members,
                instance.G9GetPropertiesOfObject(G9EAccessModifier.Public,
                    s => s.CanWrite && !s.GetCustomAttributes(typeof(G9JsonIgnoreMemberAttribute), true).Any()));

            for (var i = 0; i < elems.Count; i += 2)
            {
                if (elems[i].Length <= 2)
                    continue;
                var key = elems[i].Substring(1, elems[i].Length - 2);
                var value = elems[i + 1];

                if (!members.TryGetValue(key, out var memberInfo)) continue;
                switch (memberInfo)
                {
                    case G9DtFields fieldInfo:
                        fieldInfo.SetValue(ParsePureJsonData(fieldInfo.FieldInfo.FieldType, value));
                        break;
                    case G9DtProperties propertyInfo:
                        propertyInfo.SetValue(ParsePureJsonData(propertyInfo.PropertyInfo.PropertyType, value));
                        break;
                }
            }

            return instance;
        }
    }
}