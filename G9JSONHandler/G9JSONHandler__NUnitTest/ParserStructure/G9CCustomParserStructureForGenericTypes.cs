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
            G9IMemberGetter accessToObjectMember)
        {
            var fields = G9Assembly.ObjectAndReflectionTools.GetFieldsOfObject(objectForParsing)
                .ToDictionary(s => s.Name);
            return fields[nameof(G9CClassD<object>.A)].GetValue<string>() + "-" +
                   (genericTypes[0] == typeof(int)
                       ? 99
                       : genericTypes[0] == typeof(decimal)
                           ? fields[nameof(G9CClassD<object>.B)].GetValue<decimal>()
                               .ToString(CultureInfo.InvariantCulture)
                           : "None");
        }

        public override object StringToObject(string stringForParsing, Type[] genericTypes,
            G9IMemberGetter accessToObjectMember)
        {
            var data = stringForParsing.Split("-");
            var instance =
                G9Assembly.InstanceTools.CreateInstanceFromGenericType(typeof(G9CClassD<>), genericTypes);
            var fields = G9Assembly.ObjectAndReflectionTools.GetFieldsOfObject(instance).ToDictionary(s => s.Name);
            fields[nameof(G9CClassD<object>.A)].SetValue(data[0]);
            fields[nameof(G9CClassD<object>.B)].SetValue(genericTypes[0] == typeof(int)
                ? int.Parse(data[1])
                : genericTypes[0] == typeof(decimal)
                    ? decimal.Parse(data[1])
                    : data[1]
            );
            return instance;
        }
    }
}