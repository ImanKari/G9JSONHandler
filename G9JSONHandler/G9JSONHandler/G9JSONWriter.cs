using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace G9JSONHandler
{
    // ReSharper disable once InconsistentNaming
    public static class G9JSONWriter
    {
        public static string ToJson(this object item, bool unformatted = false)
        {
            var stringBuilder = new StringBuilder();
            var tabsNumber = 0;
            AppendValue(stringBuilder, item, unformatted, ref tabsNumber);
            return stringBuilder.ToString();
        }

        private static void AppendValue(StringBuilder stringBuilder, object item, bool unformatted, ref int tabsNumber)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            var type = item.GetType();
            if (type == typeof(string) || type == typeof(char))
            {
                stringBuilder.Append('"');
                var str = item.ToString();
                foreach (var ch in str)
                    if (ch < ' ' || ch == '"' || ch == '\\')
                    {
                        stringBuilder.Append('\\');
                        var j = "\"\\\n\r\t\b\f".IndexOf(ch);
                        if (j >= 0)
                            // ReSharper disable once StringLiteralTypo
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat("u{0:X4}", (uint)ch);
                    }
                    else
                    {
                        stringBuilder.Append(ch);
                    }

                stringBuilder.Append('"');
            }
            else if (type == typeof(byte) || type == typeof(sbyte))
            {
                stringBuilder.Append(item);
            }
            else if (type == typeof(short) || type == typeof(ushort))
            {
                stringBuilder.Append(item);
            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                stringBuilder.Append(item);
            }
            else if (type == typeof(long) || type == typeof(ulong))
            {
                stringBuilder.Append(item);
            }
            else if (type == typeof(float))
            {
                stringBuilder.Append(((float)item).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                stringBuilder.Append(((double)item).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(decimal))
            {
                stringBuilder.Append(((decimal)item).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(bool))
            {
                stringBuilder.Append((bool)item ? "true" : "false");
            }
            else if (type == typeof(DateTime))
            {
                stringBuilder.Append($"\"{((DateTime)item).ToString(CultureInfo.InvariantCulture)}\"");
            }
            else if (type == typeof(TimeSpan))
            {
                stringBuilder.Append($"\"{(TimeSpan)item}\"");
            }
            else if (type.IsEnum)
            {
                stringBuilder.Append($"\"{item}\"");
            }
            else if (item is IList list)
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
                    AppendValue(stringBuilder, m, unformatted, ref tabsNumber);
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

                var dict = item as IDictionary;
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
                        AppendValue(stringBuilder, dict[key], unformatted, ref tabsNumber);
                    }

                stringBuilder.Append(unformatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
            }
            else
            {
                if (unformatted)
                    stringBuilder.Append('{');
                else
                    stringBuilder.Append(stringBuilder.Length > 0
                        ? $"\n{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}"
                        : $"{new string('\t', tabsNumber)}{{\n{new string('\t', ++tabsNumber)}");

                var isFirst = true;
                var fieldInfos =
                    type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (var m in fieldInfos)
                {
                    if (m.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    var value = m.GetValue(item);
                    if (value == null) continue;
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(unformatted ? "," : $",\n{new string('\t', tabsNumber)}");
                    stringBuilder.Append('\"');
                    stringBuilder.Append(GetMemberName(m));
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, value, unformatted, ref tabsNumber);
                }

                var propertyInfo =
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (var m in propertyInfo)
                {
                    if (!m.CanRead || m.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    var value = m.GetValue(item, null);
                    if (value == null) continue;
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(unformatted ? "," : $",\n{new string('\t', tabsNumber)}");
                    stringBuilder.Append('\"');
                    stringBuilder.Append(GetMemberName(m));
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, value, unformatted, ref tabsNumber);
                }

                stringBuilder.Append(unformatted ? "}" : $"\n{new string('\t', --tabsNumber)}}}");
            }
        }

        private static string GetMemberName(MemberInfo member)
        {
            if (!member.IsDefined(typeof(DataMemberAttribute), true)) return member.Name;
            var dataMemberAttribute =
                (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
            return !string.IsNullOrEmpty(dataMemberAttribute.Name) ? dataMemberAttribute.Name : member.Name;
        }
    }
}