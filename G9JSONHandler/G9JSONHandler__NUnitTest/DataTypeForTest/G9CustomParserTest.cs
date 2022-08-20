using System.Linq;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    // A class that consists of custom parser methods.
    public class CustomParser
    {
        // Custom parser for parsing the object to string.
        // Note: The specified method must have two parameters (the first parameter is 'string' and the second one is 'G9IMemberGetter');
        // in continuation, it must return an object value that is parsed from the string value.
        public string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            if (accessToObjectMember.MemberType == typeof(CustomChildObject))
                return ((CustomChildObject)objectForParsing).Number1 + "-" +
                       ((CustomChildObject)objectForParsing).Number2 +
                       "-" + ((CustomChildObject)objectForParsing).Number3;
            return default;
        }

        // Custom parser for parsing the string to object.
        // Note: The specified method must have two parameters (the first parameter is 'object' and the second one is 'G9IMemberGetter');
        // in continuation, it must return a string value that is parsed from the object value.
        public object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            if (accessToObjectMember.MemberType != typeof(CustomChildObject))
                return default;
            var numberData = stringForParsing.Split('-').Select(int.Parse).ToArray();
            return new CustomChildObject
            {
                Number1 = numberData[0],
                Number2 = numberData[1],
                Number3 = numberData[2]
            };
        }
    }

    // Custom object
    public class CustomObject
    {
        // This attribute has several overloads.
        // The popular way (used below), for use, must specify the type of custom parse class in the first parameter,
        // the second parameter specifies the string to object method name, and the last one specifies the object to string method name.
        [G9AttrJsonCustomMemberParsingProcess(typeof(CustomParser), nameof(CustomParser.StringToObject),
            nameof(CustomParser.ObjectToString))]
        public CustomChildObject CustomChild = new();
    }

    // Custom child object
    public class CustomChildObject
    {
        public int Number1 = 9;
        public int Number2 = 8;
        public int Number3 = 7;
    }
}