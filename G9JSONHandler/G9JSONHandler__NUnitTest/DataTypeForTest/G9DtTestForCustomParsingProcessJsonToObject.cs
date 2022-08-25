using System;
using G9AssemblyManagement;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9CCustomParserJsonToObject1
    {
        public object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            return new TestObject
            {
                Name = stringForParsing
            };
        }
    }

    public class G9CCustomParserJsonToObject2
    {
        public static object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            return new TestObject
            {
                Time = TimeSpan.Parse(stringForParsing)
            };
        }
    }

    public static class G9CCustomParserJsonToObject3
    {
        public static object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            return new TestObject
            {
                Color = G9Assembly.TypeTools.SmartChangeType<ConsoleColor>(stringForParsing),
                Color2 = G9Assembly.TypeTools.SmartChangeType<ConsoleColor>(stringForParsing)
            };
        }
    }

    public class G9DtTestForCustomParsingProcessJsonToObject
    {
        public string Name = "G9TM";

        /// <summary>
        ///     Test with standard object and standard methods
        /// </summary>
        [G9AttrJsonMemberCustomParser(true, typeof(G9CCustomParserJsonToObject1),
            nameof(G9CCustomParserJsonToObject1.StringToObject))]
        public TestObject TestObject1 = new();

        /// <summary>
        ///     Test with standard object and static methods
        /// </summary>
        [G9AttrJsonMemberCustomParser(true, typeof(G9CCustomParserJsonToObject2),
            nameof(G9CCustomParserJsonToObject2.StringToObject))]
        public TestObject TestObject2 = new();

        /// <summary>
        ///     Test with static object and static methods
        /// </summary>
        [G9AttrJsonMemberCustomParser(true, typeof(G9CCustomParserJsonToObject3),
            nameof(G9CCustomParserJsonToObject3.StringToObject))]
        public TestObject TestObject3 = new();
    }
}