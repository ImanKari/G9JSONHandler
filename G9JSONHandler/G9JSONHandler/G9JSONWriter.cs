using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using G9AssemblyManagement.Enums;
using G9AssemblyManagement.Helper;
using G9JSONHandler.Attributes;
using G9JSONHandler.Common;

namespace G9JSONHandler
{
    /// <summary>
    ///     A pretty small library for JSON
    ///     A static helper class for writing JSON
    /// </summary>
    public static class G9JsonWriter
    {
        /// <summary>
        ///     Method to convert any object to JSON
        /// </summary>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="unformatted">Specifies that JSON result must be formatted or not</param>
        /// <returns>A string object that is filled by JSON</returns>
        public static string G9ObjectToJson(this object objectItem, bool unformatted = true)
        {
            var stringBuilder = new StringBuilder();
            var tabsNumber = 0;
            ParseValues(stringBuilder, objectItem, unformatted, ref tabsNumber);
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     A recursive method for parsing JSON values
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="unformatted">Specifies that JSON result must be formatted or not</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseValues(StringBuilder stringBuilder, object objectItem, bool unformatted,
            ref int tabsNumber)
        {
            if (objectItem == null)
            {
                stringBuilder.Append("null");
                return;
            }

            var type = objectItem.GetType();

            if (type.IsEnum)
                ParseEnumTypes(stringBuilder, objectItem, type);
            else if (type.G9IsTypeBuiltInDotNetType())
                ParseDotNetBuiltInTypes(stringBuilder, objectItem, type);
            else if (G9CCommonHelper.IsEnumerableType(type))
                ParseCollectionTypes(stringBuilder, objectItem, type, unformatted, ref tabsNumber);
            else
                ParseCustomObjectTypes(stringBuilder, objectItem, type, unformatted, ref tabsNumber);
        }

        /// <summary>
        ///     Method to convert enum value to JSON
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        private static void ParseEnumTypes(StringBuilder stringBuilder, object objectItem, Type type)
        {
            stringBuilder.Append(Convert.ChangeType(objectItem, Type.GetTypeCode(type)));
        }

        /// <summary>
        ///     Method to prepare a character for storing as a JSON structure
        /// </summary>
        /// <param name="stringBuilder">Specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="charItem">Specifies a character for analyze</param>
        private static void PrepareCharacterForStoring(StringBuilder stringBuilder, char charItem)
        {
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
                    stringBuilder.Append('"');
                    PrepareCharacterForStoring(stringBuilder, (char)objectItem);
                    stringBuilder.Append('"');
                    break;
                case TypeCode.String:
                    stringBuilder.Append('"');
                    foreach (var ch in (string)objectItem)
                        PrepareCharacterForStoring(stringBuilder, ch);
                    stringBuilder.Append('"');
                    break;
                case TypeCode.Single:
                    stringBuilder.Append(((float)objectItem).ToString(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Double:
                    stringBuilder.Append(((double)objectItem).ToString(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Decimal:
                    stringBuilder.Append(((decimal)objectItem).ToString(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Boolean:
                    stringBuilder.Append((bool)objectItem ? "true" : "false");
                    break;
                case TypeCode.DateTime:
                    stringBuilder.Append($"\"{((DateTime)objectItem).ToString(CultureInfo.InvariantCulture)}\"");
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    stringBuilder.Append(objectItem);
                    break;
                default:
                    stringBuilder.Append($"\"{objectItem}\"");
                    break;
            }
        }

        /// <summary>
        ///     The helper method to parse collection types
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="type">Specifies type of object item</param>
        /// <param name="unformatted">Specifies that JSON result must be formatted or not</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseCollectionTypes(StringBuilder stringBuilder, object objectItem, Type type,
            bool unformatted, ref int tabsNumber)
        {
            if (objectItem is IList list)
            {
                stringBuilder.Append(unformatted
                    ? "["
                    : $"\n{new string('\t', tabsNumber)}[\n{new string('\t', ++tabsNumber)}");
                var isFirst = true;
                foreach (var m in list)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(unformatted ? "," : $",\n{new string('\t', tabsNumber)}");
                    ParseValues(stringBuilder, m, unformatted, ref tabsNumber);
                }

                stringBuilder.Append(unformatted
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

                if (unformatted)
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
                            stringBuilder.Append(unformatted ? "," : $",\n{new string('\t', tabsNumber)}");
                        stringBuilder.Append('\"');
                        stringBuilder.Append((string)key);
                        stringBuilder.Append("\":");
                        ParseValues(stringBuilder, dict[key], unformatted, ref tabsNumber);
                    }

                stringBuilder.Append(unformatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
            }
        }

        /// <summary>
        ///     The helper method to parse custom object types
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="type">Specifies type of object item</param>
        /// <param name="unformatted">Specifies that JSON result must be formatted or not</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void ParseCustomObjectTypes(StringBuilder stringBuilder, object objectItem, Type type,
            bool unformatted,
            ref int tabsNumber)
        {
            // Write note comments if that existed
            var noteComments = (IList<G9JsonComment>)type.GetCustomAttributes(typeof(G9JsonComment), true);
            if (noteComments.Any())
                foreach (var note in noteComments)
                    WriteJSONNoteComment(stringBuilder, note.CustomNot, unformatted, tabsNumber);

            if (unformatted)
                stringBuilder.Append('{');
            else
                stringBuilder.Append(stringBuilder.Length > 0
                    ? $"\n{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}"
                    : $"{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}");

            var isFirst = true;
            var fieldInfos = objectItem.G9GetFieldsOfObject(G9EAccessModifier.Public,
                s => !s.GetCustomAttributes(typeof(G9JsonIgnoreMemberAttribute), true).Any());
            foreach (var m in fieldInfos)
            {
                var value = m.GetValue();
                if (value == null) continue;
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(unformatted ? "," : $",\n{new string('\t', tabsNumber)}");

                // Write note comments if that existed
                var fieldNoteComments = m.GetCustomAttributes<G9JsonComment>( true);
                if (fieldNoteComments.Any())
                    foreach (var note in fieldNoteComments)
                        WriteJSONNoteComment(stringBuilder, note.CustomNot, unformatted, tabsNumber);

                stringBuilder.Append('\"');
                var nameAttr = m.GetCustomAttributes<G9JsonCustomMemberNameAttribute>(true);
                stringBuilder.Append(nameAttr.Any() ? nameAttr[0].Name : m.Name);
                stringBuilder.Append("\":");
                if (m.FieldInfo.FieldType.IsEnum && m.GetCustomAttributes<G9JsonStoreEnumAsString>(true).Any())
                    ParseValues(stringBuilder, value.ToString(), unformatted, ref tabsNumber);
                else
                    ParseValues(stringBuilder, value, unformatted, ref tabsNumber);
            }

            var propertyInfo = objectItem.G9GetPropertiesOfObject(G9EAccessModifier.Public,
                s => s.CanRead && !s.GetCustomAttributes(typeof(G9JsonIgnoreMemberAttribute), true).Any());

            foreach (var m in propertyInfo)
            {
                var value = m.GetValue();
                if (value == null) continue;
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(unformatted ? "," : $",\n{new string('\t', tabsNumber)}");

                // Write note comments if that existed
                var propertyNoteComments = m.GetCustomAttributes<G9JsonComment>(true);
                if (propertyNoteComments.Any())
                    foreach (var note in propertyNoteComments)
                        WriteJSONNoteComment(stringBuilder, note.CustomNot, unformatted, tabsNumber);

                stringBuilder.Append('\"');
                var nameAttr = m.GetCustomAttributes<G9JsonCustomMemberNameAttribute>(true);
                stringBuilder.Append(nameAttr.Any() ? nameAttr[0].Name : m.Name);
                stringBuilder.Append("\":");
                if (m.PropertyInfo.PropertyType.IsEnum && m.GetCustomAttributes<G9JsonStoreEnumAsString>(true).Any())
                    ParseValues(stringBuilder, value.ToString(), unformatted, ref tabsNumber);
                else
                    ParseValues(stringBuilder, value, unformatted, ref tabsNumber);
            }

            stringBuilder.Append(unformatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
        }

        /// <summary>
        ///     Method to write note comment in JSON
        /// </summary>
        /// <param name="stringBuilder">specifies a StringBuilder object for storing JSON structure</param>
        /// <param name="noteComment">Specifies the note comment for writing</param>
        /// <param name="unformatted">Specifies that JSON result must be formatted or not</param>
        /// <param name="tabsNumber">If the formatted option is set to true, this parameter stores the nested tab number.</param>
        private static void WriteJSONNoteComment(StringBuilder stringBuilder, string noteComment, bool unformatted,
            int tabsNumber)
        {
            stringBuilder.Append(unformatted
                ? $"/* {noteComment} */"
                : $"/* {noteComment} */\n{new string('\t', tabsNumber)}");
        }
    }
}