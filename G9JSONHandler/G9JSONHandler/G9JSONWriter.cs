using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using G9AssemblyManagement;
using G9AssemblyManagement.Enums;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;
using G9JSONHandler.Common;
using G9JSONHandler.Enum;

namespace G9JSONHandler
{
    /// <summary>
    ///     A pretty small library for JSON
    ///     A static helper class for writing JSON
    /// </summary>
    public static class G9JsonWriter
    {
        /// <summary>
        ///     Specifies the separator according to format mode.
        /// </summary>
        [ThreadStatic] private static string _separator;

        /// <summary>
        ///     Specifies that JSON result must be formatted or not
        /// </summary>
        [ThreadStatic] private static bool _unformatted;

        /// <summary>
        ///     Method to convert any object to JSON
        /// </summary>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="formatted">Specifies that JSON result must be formatted or not</param>
        /// <returns>A string object that is filled by JSON</returns>
        public static string G9ObjectToJson(this object objectItem, bool formatted = false)
        {
            _separator = formatted ? "\": " : "\":";
            var stringBuilder = new StringBuilder();
            var tabsNumber = 0;
            _unformatted = !formatted;
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

            if (!type.IsArray && !type.IsGenericType &&
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
                stringBuilder.Append(_unformatted
                    ? "["
                    : $"\n{new string('\t', tabsNumber)}[\n{new string('\t', ++tabsNumber)}");
                var isFirst = true;
                foreach (var m in list)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(_unformatted ? "," : $",\n{new string('\t', tabsNumber)}");
                    ParseObjectMembersToJson(stringBuilder, m, ref tabsNumber);
                }

                stringBuilder.Append(_unformatted
                    ? "]"
                    : $"\n{new string('\t', --tabsNumber)}]");
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = type.GetGenericArguments()[0];

                //Refuse to output dictionary keys that aren't of type string
                if (keyType != typeof(string))
                {
                    stringBuilder.Append("{}");
                    return;
                }

                if (_unformatted)
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
                            stringBuilder.Append(_unformatted ? "," : $",\n{new string('\t', tabsNumber)}");
                        stringBuilder.Append('\"');
                        stringBuilder.Append((string)key);
                        stringBuilder.Append(_separator);
                        ParseObjectMembersToJson(stringBuilder, dict[key], ref tabsNumber);
                    }

                stringBuilder.Append(_unformatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
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

            // Write note comments if that existed
            var noteComments =
                (IList<G9AttrJsonCommentAttribute>)type.GetCustomAttributes(typeof(G9AttrJsonCommentAttribute), true);
            if (noteComments.Any())
                foreach (var note in noteComments)
                    WriteJsonNoteComment(stringBuilder, note.CustomNote, note.IsNonstandardComment, tabsNumber,
                        commentNumber++);

            if (_unformatted)
                stringBuilder.Append('{');
            else
                stringBuilder.Append(stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '\t'
                    ? $"\n{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}"
                    : $"{{\n{new string('\t', ++tabsNumber)}");

            var isFirst = true;

            // Check custom parse for a member in interface way
            if (G9CJsonCommon.CustomParserCollection != null &&
                G9CJsonCommon.CustomParserCollection.ContainsKey(type.IsGenericType
                    ? type.GetGenericTypeDefinition()
                    : type))
            {
                ParseObjectMembersToJson(stringBuilder,
                    G9CJsonCommon.CustomParserCollection[type.IsGenericType ? type.GetGenericTypeDefinition() : type]
                        .Item2(objectItem, type, null),
                    ref tabsNumber);
            }
            else
            {
                // Get total members
                var objectMembers = G9Assembly.ObjectAndReflectionTools.GetFieldsOfObject(objectItem,
                        G9EAccessModifier.Public,
                        s => !s.GetCustomAttributes(typeof(G9AttrJsonIgnoreMemberAttribute), true).Any())
                    .Select(s => (G9IMember)s).Concat(
                        G9Assembly.ObjectAndReflectionTools.GetPropertiesOfObject(objectItem,
                                G9EAccessModifier.Public,
                                s => s.CanRead && !s.GetCustomAttributes(typeof(G9AttrJsonIgnoreMemberAttribute), true)
                                    .Any())
                            .Select(s => (G9IMember)s)
                    ).ToArray();

                // Parsing process
                foreach (var m in objectMembers)
                {
                    var value = m.GetValue();
                    if (value == null) continue;
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(_unformatted ? "," : $",\n{new string('\t', tabsNumber)}");

                    // Write note comments if that existed
                    var fieldNoteComments = m.GetCustomAttributes<G9AttrJsonCommentAttribute>(true);
                    if (fieldNoteComments.Any())
                        foreach (var note in fieldNoteComments)
                            WriteJsonNoteComment(stringBuilder, note.CustomNote, note.IsNonstandardComment, tabsNumber,
                                commentNumber++);

                    stringBuilder.Append('\"');
                    var nameAttr = m.GetCustomAttributes<G9AttrJsonMemberCustomNameAttribute>(true);
                    stringBuilder.Append(nameAttr.Any() ? nameAttr[0].Name : m.Name);
                    stringBuilder.Append(_separator);

                    // Check encryption/decryption attr
                    var encryptionDecryption = m.GetCustomAttributes<G9AttrJsonMemberEncryptionAttribute>(true)
                        .FirstOrDefault();
                    if (encryptionDecryption != null)
                        value = G9Assembly.CryptographyTools.AesEncryptString(value.ToString(),
                            encryptionDecryption.PrivateKey, encryptionDecryption.InitializationVector,
                            encryptionDecryption.AesConfig);

                    // Check custom parse for a member in interface way
                    if (G9CJsonCommon.CustomParserCollection != null &&
                        G9CJsonCommon.CustomParserCollection.ContainsKey(m.MemberType.IsGenericType
                            ? m.MemberType.GetGenericTypeDefinition()
                            : m.MemberType))
                    {
                        ParseObjectMembersToJson(stringBuilder,
                            G9CJsonCommon
                                .CustomParserCollection[
                                    m.MemberType.IsGenericType ? m.MemberType.GetGenericTypeDefinition() : m.MemberType]
                                .Item2(value, m.MemberType, m),
                            ref tabsNumber);
                    }
                    else
                    {
                        // Check custom parser for a member
                        var customParser = m.GetCustomAttributes<G9AttrJsonMemberCustomParserAttribute>(true)
                            .FirstOrDefault();
                        if (customParser != null && customParser.ParserType != G9ECustomParserType.StringToObject)
                            ParseObjectMembersToJson(stringBuilder,
                                customParser.ObjectToStringMethod.CallMethodWithResult<string>(value, m),
                                ref tabsNumber);
                        else if (m.MemberType.IsEnum &&
                                 m.GetCustomAttributes<G9AttrJsonStoreEnumAsStringAttribute>(true).Any())
                            ParseObjectMembersToJson(stringBuilder, value.ToString(), ref tabsNumber);
                        else
                            ParseObjectMembersToJson(stringBuilder, value, ref tabsNumber);
                    }
                }
            }

            stringBuilder.Append(_unformatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
        }

        /// <summary>
        ///     Method to write note comment in JSON
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="noteComment">Specifies the note comment for writing</param>
        /// <param name="isNonstandardComment">Specifies that comment is a nonstandard type</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        /// <param name="commentNumber">Specifies comment number in each object</param>
        private static void WriteJsonNoteComment(StringBuilder stringBuilder, string noteComment,
            bool isNonstandardComment, int tabsNumber, int commentNumber)
        {
            if (isNonstandardComment)
                stringBuilder.Append(_unformatted
                    ? $"/* {noteComment} */"
                    : $"/* {noteComment} */\n{new string('\t', tabsNumber)}");
            else
                stringBuilder.Append(_unformatted
                    ? $"\"#__Comment{commentNumber}__#\":\"{noteComment}\","
                    : $"\"#__Comment{commentNumber}__#\": \"{PrepareCharactersForStoring(noteComment)}\",\n{new string('\t', tabsNumber)}");
        }
    }
}