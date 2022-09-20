using System;
using G9AssemblyManagement;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9CCustomParserObjectToString1
    {
        public string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return ((TestObject)objectForParsing).Name + "Okay";
        }
    }

    public class G9CCustomParserObjectToString2
    {
        public static string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return ((TestObject)objectForParsing).Time.Add(new TimeSpan(0, 9, 0)).ToString();
        }
    }

    public static class G9CCustomParserObjectToString3
    {
        public static string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return ((TestObject)objectForParsing).Color2.ToString();
        }
    }

    public class G9DtTestForCustomParsingProcessObjectToString
    {
        public string Name = "G9TM";

        /// <summary>
        ///     Test with standard object and standard methods
        /// </summary>
        [G9AttrCustomParser(false, typeof(G9CCustomParserObjectToString1),
            nameof(G9CCustomParserObjectToString1.ObjectToString))]
        public TestObject TestObject1 = new TestObject();

        /// <summary>
        ///     Test with standard object and static methods
        /// </summary>
        [G9AttrCustomParser(false, typeof(G9CCustomParserObjectToString2),
            nameof(G9CCustomParserObjectToString2.ObjectToString))]
        public TestObject TestObject2 = new TestObject();

        /// <summary>
        ///     Test with static object and static methods
        /// </summary>
        [G9AttrCustomParser(false, typeof(G9CCustomParserObjectToString3),
            nameof(G9CCustomParserObjectToString3.ObjectToString))]
        public TestObject TestObject3 = new TestObject();
    }
}