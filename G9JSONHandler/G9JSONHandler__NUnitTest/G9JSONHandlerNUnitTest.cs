using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Mime;
using G9AssemblyManagement.Helper;
using G9JSONHandler;
using G9JSONHandler_NUnitTest.DataTypeForTest;
using NUnit.Framework;

namespace G9JSONHandler_NUnitTest
{
    public class G9JSONHandlerNUnitTest
    {

        private G9DtTestObjectForParse testObjectForParsing = new G9DtTestObjectForParse();
        private string testJSONString_Unformatted = string.Empty;
        private string testJSONString_Formatted = string.Empty;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Order(1)]
        public void TestObjectToJSON()
        {
            // Test various things before converting to JSON.
            Assert.True(testObjectForParsing.CustomObject != null && testObjectForParsing.DotNetBuiltInTypes != null &&
                        testObjectForParsing.Gender == Gender.Unknown &&
                        testObjectForParsing.DotNetBuiltInTypes.H.Equals(IPAddress.Loopback) &&
                        testObjectForParsing.DotNetBuiltInTypes.G.Equals(new TimeSpan(9, 9, 9)) &&
                        testObjectForParsing.DotNetBuiltInTypes.E.Equals(DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectForParsing.DotNetBuiltInTypes.C == 9.9f &&
                        testObjectForParsing.DotNetBuiltInTypes.C3 == 999.999m &&
                        testObjectForParsing.DotNetBuiltInTypes.D && testObjectForParsing.DotNetBuiltInTypes.F8 == 26 &&
                        testObjectForParsing.CustomObject.Gender == Gender.Men &&
                        testObjectForParsing.CustomObject.FullName == "Iman Kari" &&
                        testObjectForParsing.CustomObject.NestedObjectA.B == "G9TM" &&
                        testObjectForParsing.CustomObject.NestedObjectB.E.Equals(
                            DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectForParsing.CustomObject.NestedObjectC.H.Equals(IPAddress.Loopback) &&
                        testObjectForParsing.CustomObject.TestArray.Length == 9 &&
                        testObjectForParsing.CustomObject.TestArray[8] == "Item 9" &&
                        testObjectForParsing.CustomObject.TestDictionary.Count == 9 &&
                        testObjectForParsing.CustomObject.TestDictionary["Key 9"] == "Value 9" &&
                        ((G9DtSmallStructure)testObjectForParsing.AAA[1]).C && ((G9DtSmallStructure)testObjectForParsing.AAA[1]).B == 2 &&
                        ((G9DtSmallStructure)testObjectForParsing.AAA[2]).C == false && ((G9DtSmallStructure)testObjectForParsing.AAA[2]).B == 3);

            // Converting to JSON by unformatted type
            testJSONString_Unformatted = testObjectForParsing.G9ObjectToJson();
            Assert.False(string.IsNullOrEmpty(testJSONString_Unformatted));

            // Converting to JSON by formatted type
            testJSONString_Formatted = testObjectForParsing.G9ObjectToJson(false);
            Assert.False(string.IsNullOrEmpty(testJSONString_Formatted));
        }

        [Test]
        [Order(2)]
        public void TestJSONToObject()
        {
            // Converting to object by unformatted type
            var testObjectUnformattedType = testJSONString_Unformatted.G9JsonToObject<G9DtTestObjectForParse>();
            Assert.True(testObjectUnformattedType.CustomObject != null &&
                        testObjectUnformattedType.DotNetBuiltInTypes != null &&
                        testObjectUnformattedType.Gender == Gender.Unknown &&
                        testObjectUnformattedType.DotNetBuiltInTypes.H.Equals(IPAddress.Loopback) &&
                        testObjectUnformattedType.DotNetBuiltInTypes.G.Equals(new TimeSpan(9, 9, 9)) &&
                        testObjectUnformattedType.DotNetBuiltInTypes.E.Equals(DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectUnformattedType.DotNetBuiltInTypes.C == 9.9f &&
                        testObjectUnformattedType.DotNetBuiltInTypes.C3 == 999.999m &&
                        testObjectUnformattedType.DotNetBuiltInTypes.D &&
                        testObjectUnformattedType.DotNetBuiltInTypes.F8 == 26 &&
                        testObjectUnformattedType.CustomObject.Gender == Gender.Men &&
                        testObjectUnformattedType.CustomObject.FullName == "Iman Kari" &&
                        testObjectUnformattedType.CustomObject.NestedObjectA.B == "G9TM" &&
                        testObjectUnformattedType.CustomObject.NestedObjectB.E.Equals(
                            DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectUnformattedType.CustomObject.NestedObjectC.H.Equals(IPAddress.Loopback) &&
                        testObjectUnformattedType.CustomObject.TestArray.Length == 9 &&
                        testObjectUnformattedType.CustomObject.TestArray[8] == "Item 9" &&
                        testObjectUnformattedType.CustomObject.TestDictionary.Count == 9 &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 9"] == "Value 9" &&
                        ((G9DtSmallStructure)testObjectUnformattedType.AAA[1]).C && ((G9DtSmallStructure)testObjectUnformattedType.AAA[1]).B == 2 &&
                        ((G9DtSmallStructure)testObjectUnformattedType.AAA[2]).C == false && ((G9DtSmallStructure)testObjectUnformattedType.AAA[2]).B == 3 &&
                        testObjectUnformattedType.TestMultiLine == testObjectForParsing.TestMultiLine);

            // Converting to object by formatted type
            var testObjectFormattedType = testJSONString_Formatted.G9JsonToObject<G9DtTestObjectForParse>();
            Assert.True(testObjectFormattedType.CustomObject != null &&
                        testObjectFormattedType.DotNetBuiltInTypes != null &&
                        testObjectFormattedType.Gender == Gender.Unknown &&
                        testObjectFormattedType.DotNetBuiltInTypes.H.Equals(IPAddress.Loopback) &&
                        testObjectFormattedType.DotNetBuiltInTypes.G.Equals(new TimeSpan(9, 9, 9)) &&
                        testObjectFormattedType.DotNetBuiltInTypes.E.Equals(DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectFormattedType.DotNetBuiltInTypes.C == 9.9f &&
                        testObjectFormattedType.DotNetBuiltInTypes.C3 == 999.999m &&
                        testObjectFormattedType.DotNetBuiltInTypes.D &&
                        testObjectFormattedType.DotNetBuiltInTypes.F8 == 26 &&
                        testObjectFormattedType.CustomObject.Gender == Gender.Men &&
                        testObjectFormattedType.CustomObject.FullName == "Iman Kari" &&
                        testObjectFormattedType.CustomObject.NestedObjectA.B == "G9TM" &&
                        testObjectFormattedType.CustomObject.NestedObjectB.E.Equals(
                            DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectFormattedType.CustomObject.NestedObjectC.H.Equals(IPAddress.Loopback) &&
                        testObjectFormattedType.CustomObject.TestArray.Length == 9 &&
                        testObjectFormattedType.CustomObject.TestArray[8] == "Item 9" &&
                        testObjectFormattedType.CustomObject.TestDictionary.Count == 9 &&
                        testObjectFormattedType.CustomObject.TestDictionary["Key 9"] == "Value 9" &&
                        ((G9DtSmallStructure)testObjectFormattedType.AAA[1]).C && ((G9DtSmallStructure)testObjectFormattedType.AAA[1]).B == 2 &&
                        ((G9DtSmallStructure)testObjectFormattedType.AAA[2]).C == false && ((G9DtSmallStructure)testObjectFormattedType.AAA[2]).B == 3 &&
                        testObjectFormattedType.TestMultiLine == testObjectForParsing.TestMultiLine);

        }
    }
}