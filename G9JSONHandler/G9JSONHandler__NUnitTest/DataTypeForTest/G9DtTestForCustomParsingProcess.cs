using System;
using G9AssemblyManagement;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9CCustomParser1
    {
        public object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            return new TestObject
            {
                Name = stringForParsing
            };
        }

        public string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return ((TestObject)objectForParsing).Name + "Okay";
        }
    }

    public class G9CCustomParser2
    {
        public static object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            return new TestObject
            {
                Time = TimeSpan.Parse(stringForParsing)
            };
        }

        public static string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return ((TestObject)objectForParsing).Time.Add(new TimeSpan(0, 9, 0)).ToString();
        }
    }

    public static class G9CCustomParser3
    {
        public static object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            return new TestObject
            {
                Color = G9Assembly.TypeTools.SmartChangeType<ConsoleColor>(stringForParsing),
                Color2 = G9Assembly.TypeTools.SmartChangeType<ConsoleColor>(stringForParsing)
            };
        }

        public static string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return ((TestObject)objectForParsing).Color2.ToString();
        }
    }

    public class G9DtTestForCustomParsingProcess
    {
        public string Name = "G9TM";

        /// <summary>
        ///     Test with standard object and standard methods
        /// </summary>
        [G9AttrJsonMemberCustomParser(typeof(G9CCustomParser1), nameof(G9CCustomParser1.StringToObject),
            nameof(G9CCustomParser1.ObjectToString))]
        public TestObject TestObject1 = new();

        /// <summary>
        ///     Test with standard object and static methods
        /// </summary>
        [G9AttrJsonMemberCustomParser(typeof(G9CCustomParser2), nameof(G9CCustomParser2.StringToObject),
            nameof(G9CCustomParser2.ObjectToString))]
        public TestObject TestObject2 = new();

        /// <summary>
        ///     Test with static object and static methods
        /// </summary>
        [G9AttrJsonMemberCustomParser(typeof(G9CCustomParser3), nameof(G9CCustomParser3.StringToObject),
            nameof(G9CCustomParser3.ObjectToString))]
        public TestObject TestObject3 = new();
    }
}