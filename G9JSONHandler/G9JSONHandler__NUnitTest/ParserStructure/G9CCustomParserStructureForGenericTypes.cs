using System;
using System.Globalization;
using System.Linq;
using G9AssemblyManagement;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Abstract;

namespace G9JSONHandler_NUnitTest.ParserStructure
{
    public class G9CCustomParserStructureForGenericTypes : G9ACustomGenericTypeParser
    {
        public G9CCustomParserStructureForGenericTypes() : base(typeof(G9CClassD<>))
        {
        }

        public override string ObjectToString(object objectForParsing, Type[] genericTypes,
            G9IMemberGetter accessToObjectMember, Action<string> addCustomComment)
        {
            addCustomComment("This Comment added by custom comment process in custom parser. Test 1.");
            addCustomComment("This Comment added by custom comment process in custom parser. Test 2.");
            addCustomComment("This Comment added by custom comment process in custom parser. Test 3.");

            var fields = G9Assembly.ReflectionTools.GetFieldsOfObject(objectForParsing)
                .ToDictionary(s => s.Name);

            var extraData = string.Empty;
            if (genericTypes[0] == typeof(int))
                extraData = "99";
            else if (genericTypes[0] == typeof(decimal))
                extraData = fields[nameof(G9CClassD<object>.B)].GetValue<decimal>()
                    .ToString(CultureInfo.InvariantCulture);
            else
                extraData = "None";

            return fields[nameof(G9CClassD<object>.A)].GetValue<string>() + "-" + extraData;
        }

        public override object StringToObject(string stringForParsing, Type[] genericTypes,
            G9IMemberGetter accessToObjectMember)
        {
            var data = stringForParsing.Split('-');
            var instance =
                G9Assembly.InstanceTools.CreateInstanceFromGenericType(typeof(G9CClassD<>), genericTypes);
            var fields = G9Assembly.ReflectionTools.GetFieldsOfObject(instance).ToDictionary(s => s.Name);
            fields[nameof(G9CClassD<object>.A)].SetValue(data[0]);

            object extraData = null;
            if (genericTypes[0] == typeof(int))
                extraData = int.Parse(data[1]);
            else if (genericTypes[0] == typeof(decimal))
                extraData = decimal.Parse(data[1]);
            else
                extraData = data[1];

            fields[nameof(G9CClassD<object>.B)].SetValue(extraData);
            return instance;
        }
    }
}