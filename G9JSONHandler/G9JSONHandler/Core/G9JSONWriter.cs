using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using G9AssemblyManagement;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;
using G9JSONHandler.Common;
using G9JSONHandler.DataType;
using G9JSONHandler.Enum;

namespace G9JSONHandler.Core
{
    /// <summary>
    ///     A pretty small library for JSON
    ///     A static helper class for writing JSON
    /// </summary>
    internal static class G9JsonWriter
    {
        /// <summary>
        ///     Specified the default order number for members
        /// </summary>
        private const uint DefaultOrderNumber = uint.MaxValue / 2;

        /// <summary>
        ///     Specifies the separator according to format mode.
        /// </summary>
        [ThreadStatic] private static string _separator;

        /// <summary>
        ///     Specifies that JSON result must be formatted or not
        /// </summary>
        [ThreadStatic] private static G9DtJsonWriterConfig _writerConfig;

        /// <summary>
        ///     Method to convert any object to JSON
        /// </summary>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="writerConfig">Specifies a custom config for the parsing process.</param>
        /// <returns>A string object that is filled by JSON</returns>
        public static string G9ObjectToJson(object objectItem, G9DtJsonWriterConfig writerConfig)
        {
            _writerConfig = writerConfig;
            _separator = _writerConfig.IsFormatted ? "\": " : "\":";
            var stringBuilder = new StringBuilder();
            var tabsNumber = 0;
            ParseObjectMembersToJson(stringBuilder, objectItem, ref tabsNumber);
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     A recursive method for parsing JSON values
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseObjectMembersToJson(StringBuilder stringBuilder, object objectItem, ref int tabsNumber)
        {
            if (objectItem == null)
            {
                stringBuilder.Append("null");
                return;
            }

            var type = objectItem.GetType();



            if (type == typeof(ExpandoObject))
                ParseExpandoObject(stringBuilder, (ExpandoObject)objectItem, ref tabsNumber);
            else if (!type.IsArray && !type.IsGenericType &&
                (type.IsEnum || G9Assembly.TypeTools.IsTypeBuiltInDotNetType(type)))
                ParseDotNetBuiltInTypes(stringBuilder, objectItem, type);
            else if (type.IsArray || G9Assembly.TypeTools.IsEnumerableType(type))
                ParseCollectionTypes(stringBuilder, objectItem, type, ref tabsNumber);
            else
                ParseCustomObjectTypes(stringBuilder, objectItem, type, ref tabsNumber);
        }

        /// <summary>
        ///     Method to prepare characters for storing as a JSON structure
        /// </summary>
        /// <param name="stringData">Specifies a string for analyze</param>
        private static string PrepareCharactersForStoring(string stringData)
        {
            var stringBuilder = new StringBuilder();
            foreach (var charItem in stringData)
                if (charItem < ' ' || charItem == '"' || charItem == '\\')
                {
                    stringBuilder.Append('\\');
                    var j = "\"\\\n\r\t\b\f".IndexOf(charItem);
                    if (j >= 0)
                        // ReSharper disable once StringLiteralTypo
                        stringBuilder.Append("\"\\nrtbf"[j]);
                    else
                        stringBuilder.AppendFormat("u{0:X4}", (uint)charItem);
                }
                else
                {
                    stringBuilder.Append(charItem);
                }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     The helper method to parse .NET built-in types
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="type">Specifies type of object item</param>
        private static void ParseDotNetBuiltInTypes(StringBuilder stringBuilder, object objectItem, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                    stringBuilder.Append($"\"{PrepareCharactersForStoring(objectItem.ToString())}\"");
                    break;
                case TypeCode.String:
                    stringBuilder.Append($"\"{PrepareCharactersForStoring((string)objectItem)}\"");
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                    if (type.IsEnum)
                        stringBuilder.Append(
                            G9Assembly.TypeTools.SmartChangeType(objectItem, System.Enum.GetUnderlyingType(type)));
                    else
                        stringBuilder.Append(G9Assembly.TypeTools.SmartChangeType<string>(objectItem));
                    break;
                default:
                    stringBuilder.Append($"\"{G9Assembly.TypeTools.SmartChangeType<string>(objectItem)}\"");
                    break;
            }
        }

        /// <summary>
        ///     The helper method to parse collection types
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="type">Specifies type of object item</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseCollectionTypes(StringBuilder stringBuilder, object objectItem, Type type,
            ref int tabsNumber)
        {
            if (objectItem is IList list)
            {
                stringBuilder.Append(!_writerConfig.IsFormatted
                    ? "["
                    : $"\n{new string('\t', tabsNumber)}[\n{new string('\t', ++tabsNumber)}");
                var isFirst = true;
                foreach (var m in list)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(!_writerConfig.IsFormatted ? "," : $",\n{new string('\t', tabsNumber)}");
                    ParseObjectMembersToJson(stringBuilder, m, ref tabsNumber);
                }

                stringBuilder.Append(!_writerConfig.IsFormatted
                    ? "]"
                    : $"\n{new string('\t', --tabsNumber)}]");
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = type.GetGenericArguments()[0];

                // Refuse to output dictionary keys that aren't of type string
                if (keyType != typeof(string))
                {
                    stringBuilder.Append("{}");
                    return;
                }

                if (!_writerConfig.IsFormatted)
                    stringBuilder.Append('{');
                else
                    stringBuilder.Append(stringBuilder.Length > 0
                        ? $"\n{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}"
                        : $"{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}");

                var dict = objectItem as IDictionary;
                var isFirst = true;
                if (dict != null)
                    foreach (var key in dict.Keys)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(
                                !_writerConfig.IsFormatted ? "," : $",\n{new string('\t', tabsNumber)}");
                        stringBuilder.Append('\"');
                        stringBuilder.Append((string)key);
                        stringBuilder.Append(_separator);
                        ParseObjectMembersToJson(stringBuilder, dict[key], ref tabsNumber);
                    }

                stringBuilder.Append(!_writerConfig.IsFormatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
            }
        }

        /// <summary>
        ///     The helper method to parse custom object types
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="type">Specifies type of object item</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseCustomObjectTypes(StringBuilder stringBuilder, object objectItem, Type type,
            ref int tabsNumber)
        {
            var commentNumber = 0;

            var isBaseObject = stringBuilder.Length == 0;

            // Write note comments if that existed
            var noteComments =
                (IList<G9AttrCommentAttribute>)type.GetCustomAttributes(typeof(G9AttrCommentAttribute), true);
            if (noteComments.Any())
                foreach (var note in noteComments)
                    WriteJsonNoteComment(stringBuilder, note.CustomNote, tabsNumber,
                        commentNumber++);

            // Check custom parser
            object customParseValue = null;
            if (G9CJsonCommon.CustomParserCollection != null &&
                G9CJsonCommon.CustomParserCollection.ContainsKey(type.IsGenericType
                    ? type.GetGenericTypeDefinition()
                    : type))
            {
                var tNumber = tabsNumber;
                // If a custom parser for this type exists, the comments of that must be written before its value.
                // So, the writing comment process is performed here and its value for writing would be pass to next steps.
                customParseValue = G9CJsonCommon
                    .CustomParserCollection[type.IsGenericType ? type.GetGenericTypeDefinition() : type]
                    .Item2(objectItem, type, null,
                        customComment =>
                        {
                            // ReSharper disable once AccessToModifiedClosure
                            WriteJsonNoteComment(stringBuilder, customComment, tNumber, commentNumber++);
                        });

            }

            if (isBaseObject || customParseValue == null)
            {
                if (!_writerConfig.IsFormatted)
                    stringBuilder.Append('{');
                else
                    stringBuilder.Append(stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '\t'
                        ? $"\n{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}"
                        : $"{{\n{new string('\t', ++tabsNumber)}");

            }

            var isFirst = true;

            // Check custom parse value if existed
            if (customParseValue != null)
            {
                ParseObjectMembersToJson(stringBuilder, customParseValue, ref tabsNumber);
            }
            else
            {
                // Get total members
                var objectMembers = G9Assembly.ReflectionTools.GetFieldsOfObject(objectItem,
                        _writerConfig.AccessibleModifiers,
                        s => !s.GetCustomAttributes(typeof(G9AttrIgnoreAttribute), true).Any())
                    .Select(s => (G9IMember)s).Concat(
                        G9Assembly.ReflectionTools.GetPropertiesOfObject(objectItem,
                                _writerConfig.AccessibleModifiers,
                                s => s.CanRead && !s.GetCustomAttributes(typeof(G9AttrIgnoreAttribute), true)
                                    .Any())
                            .Select(s => (G9IMember)s)
                    ).OrderBy(MemberOrderHandler).ToArray();

                // Parsing process
                foreach (var m in objectMembers)
                {
                    var value = m.GetValue();
                    if (value == null) continue;
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(!_writerConfig.IsFormatted ? "," : $",\n{new string('\t', tabsNumber)}");

                    // Write note comments if that existed
                    var fieldNoteComments = m.GetCustomAttributes<G9AttrCommentAttribute>(true);
                    if (fieldNoteComments != null)
                        foreach (var note in fieldNoteComments)
                            WriteJsonNoteComment(stringBuilder, note.CustomNote, tabsNumber,
                                commentNumber++);

                    // Check custom parser
                    object nestedCustomParseValue = null;
                    if (G9CJsonCommon.CustomParserCollection != null &&
                        G9CJsonCommon.CustomParserCollection.ContainsKey(m.MemberType.IsGenericType
                            ? m.MemberType.GetGenericTypeDefinition()
                            : m.MemberType))
                    {
                        var tNumber = tabsNumber;
                        // If a custom parser for this type exists, the comments of that must be written before its value.
                        // So, the writing comment process is performed here and its value for writing would be pass to next steps.
                        nestedCustomParseValue = G9CJsonCommon
                            .CustomParserCollection[
                                m.MemberType.IsGenericType ? m.MemberType.GetGenericTypeDefinition() : m.MemberType]
                            .Item2(value, m.MemberType, m,
                                customComment =>
                                {
                                    WriteJsonNoteComment(stringBuilder, customComment, tNumber, commentNumber++);
                                });
                    }

                    stringBuilder.Append('\"');
                    var nameAttr = m.GetCustomAttribute<G9AttrCustomNameAttribute>(true);
                    stringBuilder.Append(nameAttr != null ? nameAttr.Name : m.Name);
                    stringBuilder.Append(_separator);

                    // Check encryption/decryption attr
                    var encryptionDecryption = m.GetCustomAttribute<G9AttrEncryptionAttribute>(true);
                    if (encryptionDecryption != null)
                    {
                        var encryptor = G9Assembly.CryptographyTools.InitAesCryptography(
                            encryptionDecryption.PrivateKey,
                            encryptionDecryption.InitializationVector,
                            encryptionDecryption.AesConfig);

                        value = encryptor.EncryptString(value.ToString());
                    }

                    // Check custom parse for a member in interface way
                    if (nestedCustomParseValue != null)
                    {
                        ParseObjectMembersToJson(stringBuilder, nestedCustomParseValue, ref tabsNumber);
                    }
                    else
                    {
                        // Check custom parser for a member
                        var customParser = m.GetCustomAttribute<G9AttrCustomParserAttribute>(true);
                        if (customParser != null && customParser.ParserType != G9ECustomParserType.StringToObject)
                            ParseObjectMembersToJson(stringBuilder,
                                customParser.ObjectToStringMethod.CallMethodWithResult<string>(value, m),
                                ref tabsNumber);
                        else if (m.MemberType.IsEnum &&
                                 m.GetCustomAttribute<G9AttrStoreEnumAsStringAttribute>(true) != null)
                            ParseObjectMembersToJson(stringBuilder, value.ToString(), ref tabsNumber);
                        else
                            ParseObjectMembersToJson(stringBuilder, value, ref tabsNumber);
                    }
                }
            }

            if (isBaseObject || customParseValue == null)
                stringBuilder.Append(!_writerConfig.IsFormatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
        }

        /// <summary>
        ///     Helper method for ordering the members for writing.
        /// </summary>
        private static uint MemberOrderHandler(G9IMember member)
        {
            return member.GetCustomAttribute<G9AttrOrderAttribute>(true)?.OrderNumber ?? DefaultOrderNumber;
        }

        /// <summary>
        ///     Method to write note comment in JSON
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="noteComment">Specifies the note comment for writing</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        /// <param name="commentNumber">Specifies comment number in each object</param>
        private static void WriteJsonNoteComment(StringBuilder stringBuilder, string noteComment, int tabsNumber,
            int commentNumber)
        {
            if (_writerConfig.CommentMode == G9ECommentMode.NonstandardMode)
                stringBuilder.Append(!_writerConfig.IsFormatted
                    ? $"/* {noteComment} */"
                    : $"/* {noteComment} */\n{new string('\t', tabsNumber)}");
            else
                stringBuilder.Append(!_writerConfig.IsFormatted
                    ? $"\"#__Comment{commentNumber}__#\":\"{noteComment}\","
                    : $"\"#__Comment{commentNumber}__#\": \"{PrepareCharactersForStoring(noteComment)}\",\n{new string('\t', tabsNumber)}");
        }

        /// <summary>
        ///     Converts a G9CDynamicObject to a JSON string.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object to convert.</param>
        /// <param name="writerConfig">Configuration for the JSON writer.</param>
        /// <returns>A JSON string representation of the dynamic object.</returns>
        public static string G9DynamicObjectToJson(G9CDynamicObject dynamicObject, G9DtJsonWriterConfig writerConfig)
        {
            if (dynamicObject == null)
                return "null";

            var stringBuilder = new StringBuilder();
            SerializeDynamicObject(dynamicObject, stringBuilder, writerConfig, 0);
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Serializes a G9CDynamicObject to a JSON string.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object to serialize.</param>
        /// <param name="stringBuilder">StringBuilder to store the JSON structure.</param>
        /// <param name="writerConfig">Configuration for the JSON writer.</param>
        /// <param name="indentLevel">Current indentation level.</param>
        private static void SerializeDynamicObject(G9CDynamicObject dynamicObject, StringBuilder stringBuilder, G9DtJsonWriterConfig writerConfig, int indentLevel)
        {
            var isFirst = true;
            var indent = writerConfig.IsFormatted ? new string('\t', indentLevel) : string.Empty;

            stringBuilder.Append(writerConfig.IsFormatted ? "{\n" : "{");

            foreach (var kvp in dynamicObject)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(writerConfig.IsFormatted ? ",\n" : ",");

                stringBuilder.Append(writerConfig.IsFormatted ? $"{indent}\t\"{kvp.Key}\": " : $"\"{kvp.Key}\":");

                if (kvp.Value is G9CDynamicObject nestedDynamicObject)
                {
                    stringBuilder.Append(writerConfig.IsFormatted ? "\n" + indent + "\t" : "");
                    SerializeDynamicObject(nestedDynamicObject, stringBuilder, writerConfig, indentLevel + 1);
                }
                else if (kvp.Value is List<object> list)
                {
                    stringBuilder.Append(writerConfig.IsFormatted ? "\n" + indent + "\t" : "");
                    SerializeList(list, stringBuilder, writerConfig, indentLevel + 1);
                }
                else
                {
                    SerializeValue(kvp.Value, stringBuilder);
                }
            }

            stringBuilder.Append(writerConfig.IsFormatted ? $"\n{indent}}}" : "}");
        }

        /// <summary>
        ///     Serializes a list of objects to a JSON string.
        /// </summary>
        /// <param name="list">The list of objects to serialize.</param>
        /// <param name="stringBuilder">StringBuilder to store the JSON structure.</param>
        /// <param name="writerConfig">Configuration for the JSON writer.</param>
        /// <param name="indentLevel">Current indentation level.</param>
        private static void SerializeList(List<object> list, StringBuilder stringBuilder, G9DtJsonWriterConfig writerConfig, int indentLevel)
        {
            var isFirst = true;
            var indent = writerConfig.IsFormatted ? new string('\t', indentLevel) : string.Empty;

            stringBuilder.Append(writerConfig.IsFormatted ? "[\n" : "[");

            foreach (var item in list)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(writerConfig.IsFormatted ? ",\n" : ",");

                stringBuilder.Append(writerConfig.IsFormatted ? indent + "\t" : string.Empty);

                if (item is G9CDynamicObject nestedDynamicObject)
                {
                    SerializeDynamicObject(nestedDynamicObject, stringBuilder, writerConfig, indentLevel + 1);
                }
                else if (item is List<object> nestedList)
                {
                    SerializeList(nestedList, stringBuilder, writerConfig, indentLevel + 1);
                }
                else
                {
                    SerializeValue(item, stringBuilder);
                }
            }

            stringBuilder.Append(writerConfig.IsFormatted ? $"\n{indent}]" : "]");
        }

        /// <summary>
        ///     Serializes a primitive value to a JSON string.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="stringBuilder">StringBuilder to store the JSON structure.</param>
        private static void SerializeValue(object value, StringBuilder stringBuilder)
        {
            if (value == null)
            {
                stringBuilder.Append("null");
            }
            else if (value is string str)
            {
                stringBuilder.Append($"\"{EscapeString(str)}\"");
            }
            else if (value is bool boolean)
            {
                stringBuilder.Append(boolean.ToString().ToLower());
            }
            else
            {
                stringBuilder.Append(value);
            }
        }

        /// <summary>
        ///     Escapes special characters in a string for JSON.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        private static string EscapeString(string str)
        {
            var stringBuilder = new StringBuilder();
            foreach (var c in str)
            {
                switch (c)
                {
                    case '\\': stringBuilder.Append("\\\\"); break;
                    case '\"': stringBuilder.Append("\\\""); break;
                    case '\n': stringBuilder.Append("\\n"); break;
                    case '\r': stringBuilder.Append("\\r"); break;
                    case '\t': stringBuilder.Append("\\t"); break;
                    default:
                        if (c < ' ')
                            stringBuilder.AppendFormat("\\u{0:X4}", (int)c);
                        else
                            stringBuilder.Append(c);
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     The helper method to parse ExpandoObject types
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="expando">Specifies an ExpandoObject for converting to JSON</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseExpandoObject(StringBuilder stringBuilder, ExpandoObject expando, ref int tabsNumber)
        {
            if (!_writerConfig.IsFormatted)
                stringBuilder.Append('{');
            else
                stringBuilder.Append(stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '\t'
                    ? $"\n{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}"
                    : $"{{\n{new string('\t', ++tabsNumber)}");

            var isFirst = true;
            var dictionary = (IDictionary<string, object>)expando;
            foreach (var kvp in dictionary)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(!_writerConfig.IsFormatted ? "," : $",\n{new string('\t', tabsNumber)}");

                stringBuilder.Append('\"');
                stringBuilder.Append(kvp.Key);
                stringBuilder.Append(_separator);

                ParseObjectMembersToJson(stringBuilder, kvp.Value, ref tabsNumber);
            }

            stringBuilder.Append(!_writerConfig.IsFormatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
        }
    }
}