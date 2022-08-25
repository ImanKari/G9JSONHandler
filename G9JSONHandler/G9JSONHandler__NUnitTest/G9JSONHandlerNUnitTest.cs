using System;
using System.Net;
using System.Reflection;
using G9AssemblyManagement;
using G9JSONHandler;
using G9JSONHandler.Attributes;
using G9JSONHandler_NUnitTest.DataTypeForTest;
using NUnit.Framework;

namespace G9JSONHandler_NUnitTest
{
    public class G9JSONHandlerNUnitTest
    {
        private readonly G9DtTestObjectForParse testObjectForParsing = new();
        private string testJSONString_Formatted = string.Empty;
        private string testJSONString_Unformatted = string.Empty;

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
                        testObjectForParsing.CustomObject.Gender == Gender.Unknown &&
                        testObjectForParsing.CustomObject.FullName == "\"Iman\"Kari\"" &&
                        testObjectForParsing.CustomObject.NestedObjectA.B == "G9TM" &&
                        testObjectForParsing.CustomObject.NestedObjectB.E.Equals(
                            DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectForParsing.CustomObject.NestedObjectC.H.Equals(IPAddress.Loopback) &&
                        testObjectForParsing.CustomObject.TestArray.Length == 9 &&
                        testObjectForParsing.CustomObject.TestArray[8] == "Item 9" &&
                        testObjectForParsing.CustomObject.TestDictionary.Count == 9 &&
                        testObjectForParsing.CustomObject.TestDictionary["Key 9"] == "Value 9" &&
                        testObjectForParsing.AAA[1].C && testObjectForParsing.AAA[1].B == 2 &&
                        testObjectForParsing.AAA[2].C == false && testObjectForParsing.AAA[2].B == 3);

            // Converting to JSON by unformatted type
            testJSONString_Unformatted = testObjectForParsing.G9ObjectToJson();
            Assert.False(string.IsNullOrEmpty(testJSONString_Unformatted));

            // Converting to JSON by formatted type
            testJSONString_Formatted = testObjectForParsing.G9ObjectToJson(true);
            Assert.False(string.IsNullOrEmpty(testJSONString_Formatted));

            var nonstandardComment = "/* 1- This note comment is used just for tests! Nonstandard Type! */";
            Assert.True(testJSONString_Unformatted.Contains(nonstandardComment) &&
                        testJSONString_Formatted.Contains(nonstandardComment));
        }

        [Test]
        [Order(2)]
        public void TestJSONToObject()
        {
            // Converting to object by unformatted type
            var testObjectUnformattedType = testJSONString_Unformatted.G9JsonToObject<G9DtTestObjectForParse>();
            Assert.True(testObjectUnformattedType.CustomObject != null &&
                        testObjectUnformattedType.DotNetBuiltInTypes != null &&
                        testObjectUnformattedType.A2 == "\"G9\"TM2\"" &&
                        testObjectUnformattedType.Gender == Gender.Unknown &&
                        testObjectUnformattedType.DotNetBuiltInTypes.H.Equals(IPAddress.Loopback) &&
                        testObjectUnformattedType.DotNetBuiltInTypes.G.Equals(new TimeSpan(9, 9, 9)) &&
                        testObjectUnformattedType.DotNetBuiltInTypes.E.Equals(DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectUnformattedType.DotNetBuiltInTypes.C == 9.9f &&
                        testObjectUnformattedType.DotNetBuiltInTypes.C3 == 999.999m &&
                        testObjectUnformattedType.DotNetBuiltInTypes.D &&
                        testObjectUnformattedType.DotNetBuiltInTypes.F8 == 26 &&
                        testObjectUnformattedType.CustomObject.Gender == Gender.Unknown &&
                        testObjectUnformattedType.CustomObject.FullName == "\"Iman\"Kari\"" &&
                        testObjectUnformattedType.CustomObject.NestedObjectA.B == "G9TM" &&
                        testObjectUnformattedType.CustomObject.NestedObjectB.E.Equals(
                            DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectUnformattedType.CustomObject.NestedObjectC.H.Equals(IPAddress.Loopback) &&
                        testObjectUnformattedType.CustomObject.TestArray.Length == 9 &&
                        testObjectUnformattedType.CustomObject.TestArray[8] == "Item 9" &&
                        testObjectUnformattedType.CustomObject.TestDictionary.Count == 9 &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 5"] == "\"Value 5" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 6"] == "\"Value 6\"" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 7"] == "Value 7\"" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 8"] == "\"Value 8" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 9"] == "Value 9" &&
                        testObjectUnformattedType.AAA[1].C && testObjectUnformattedType.AAA[1].B == 2 &&
                        testObjectUnformattedType.AAA[2].C == false && testObjectUnformattedType.AAA[2].B == 3 &&
                        testObjectUnformattedType.TestMultiLine == testObjectForParsing.TestMultiLine);

            // Converting to object by formatted type
            var testObjectFormattedType = testJSONString_Formatted.G9JsonToObject<G9DtTestObjectForParse>();
            Assert.True(testObjectFormattedType.CustomObject != null &&
                        testObjectFormattedType.DotNetBuiltInTypes != null &&
                        testObjectUnformattedType.A2 == "\"G9\"TM2\"" &&
                        testObjectFormattedType.Gender == Gender.Unknown &&
                        testObjectFormattedType.DotNetBuiltInTypes.H.Equals(IPAddress.Loopback) &&
                        testObjectFormattedType.DotNetBuiltInTypes.G.Equals(new TimeSpan(9, 9, 9)) &&
                        testObjectFormattedType.DotNetBuiltInTypes.E.Equals(DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectFormattedType.DotNetBuiltInTypes.C == 9.9f &&
                        testObjectFormattedType.DotNetBuiltInTypes.C3 == 999.999m &&
                        testObjectFormattedType.DotNetBuiltInTypes.D &&
                        testObjectFormattedType.DotNetBuiltInTypes.F8 == 26 &&
                        testObjectFormattedType.CustomObject.Gender == Gender.Unknown &&
                        testObjectFormattedType.CustomObject.FullName == "\"Iman\"Kari\"" &&
                        testObjectFormattedType.CustomObject.NestedObjectA.B == "G9TM" &&
                        testObjectFormattedType.CustomObject.NestedObjectB.E.Equals(
                            DateTime.Parse("1990/09/01 09:09:09")) &&
                        testObjectFormattedType.CustomObject.NestedObjectC.H.Equals(IPAddress.Loopback) &&
                        testObjectFormattedType.CustomObject.TestArray.Length == 9 &&
                        testObjectFormattedType.CustomObject.TestArray[8] == "Item 9" &&
                        testObjectFormattedType.CustomObject.TestDictionary.Count == 9 &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 5"] == "\"Value 5" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 6"] == "\"Value 6\"" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 7"] == "Value 7\"" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 8"] == "\"Value 8" &&
                        testObjectUnformattedType.CustomObject.TestDictionary["Key 9"] == "Value 9" &&
                        testObjectFormattedType.AAA[1].C && testObjectFormattedType.AAA[1].B == 2 &&
                        testObjectFormattedType.AAA[2].C == false && testObjectFormattedType.AAA[2].B == 3 &&
                        testObjectFormattedType.TestMultiLine == testObjectForParsing.TestMultiLine);
        }

        [Test]
        [Order(3)]
        public void TestSampleClass()
        {
            var testObject = new TestObject();
            // Unformatted JSON
            var unformattedJson = testObject.G9ObjectToJson();
            // Formatted JSON
            var formattedJson = testObject.G9ObjectToJson(true);

            // Test
            var newObject = unformattedJson.G9JsonToObject<TestObject>();
            Assert.True(newObject.Name == testObject.Name);
            Assert.True(newObject.Color == testObject.Color);
            Assert.True(newObject.Array.Length == testObject.Array.Length);
            Assert.True(newObject.Dictionary.Count == testObject.Dictionary.Count);
            Assert.True(newObject.Dictionary["Key 2"] == testObject.Dictionary["Key 2"]);

            newObject = formattedJson.G9JsonToObject<TestObject>();
            Assert.True(newObject.Name == testObject.Name);
            Assert.True(newObject.Color == testObject.Color);
            Assert.True(newObject.Array.Length == testObject.Array.Length);
            Assert.True(newObject.Dictionary.Count == testObject.Dictionary.Count);
            Assert.True(newObject.Dictionary["Key 2"] == testObject.Dictionary["Key 2"]);
        }

        [Test]
        [Order(4)]
        public void TestSafeThreadShock()
        {
            G9Assembly.PerformanceTools.MultiThreadShockTest(randomNumber =>
            {
                var testObject = new TestObject
                {
                    Name = $"Name {randomNumber}"
                };
                var unformattedJson = testObject.G9ObjectToJson();
                var formattedJson = testObject.G9ObjectToJson(true);
                var newObject = unformattedJson.G9JsonToObject<TestObject>();
                var newObject2 = formattedJson.G9JsonToObject<TestObject>();
                Assert.True(newObject.Name == testObject.Name);
                Assert.True(newObject2.Name == testObject.Name);
            }, 99_999);
        }

        [Test]
        [Order(5)]
        public void TestCustomParsing()
        {
            void TestCustomParser()
            {
                // Create an object that consists of custom parsing process
                var objectWithCustomParser = new G9DtTestForCustomParsingProcess();
                // Custom parsing process calls automatically in parsing
                var stringJson = objectWithCustomParser.G9ObjectToJson(true);
                Assert.True(!string.IsNullOrEmpty(stringJson));

                // Test custom parsing process for json to object (The custom values is set in custom parsing)
                var objectFromJson = stringJson.G9JsonToObject<G9DtTestForCustomParsingProcess>();
                Assert.True(objectFromJson is { TestObject1: { }, TestObject2: { }, TestObject3: { } } &&
                            objectFromJson.TestObject1.Name == objectWithCustomParser.TestObject1.Name + "Okay" &&
                            objectFromJson.TestObject2.Time ==
                            objectWithCustomParser.TestObject2.Time.Add(new TimeSpan(0, 9, 0)) &&
                            objectFromJson.TestObject3.Color == objectWithCustomParser.TestObject3.Color2 &&
                            objectFromJson.TestObject3.Color2 == objectWithCustomParser.TestObject3.Color2);

                // Test custom parsing process just for json to object
                var objectWithCustomParserJsonToObject = new G9DtTestForCustomParsingProcessObjectToString();
                stringJson = objectWithCustomParserJsonToObject.G9ObjectToJson(true);

                // Test custom parsing process just for object to json
                var objectFromJsonObjectToString =
                    stringJson.G9JsonToObject<G9DtTestForCustomParsingProcessJsonToObject>();
                Assert.True(objectFromJsonObjectToString is { TestObject1: { }, TestObject2: { }, TestObject3: { } } &&
                            objectFromJsonObjectToString.TestObject1.Name ==
                            objectWithCustomParser.TestObject1.Name + "Okay" &&
                            objectFromJsonObjectToString.TestObject2.Time ==
                            objectWithCustomParser.TestObject2.Time.Add(new TimeSpan(0, 9, 0)) &&
                            objectFromJsonObjectToString.TestObject3.Color ==
                            objectWithCustomParser.TestObject3.Color2 &&
                            objectFromJsonObjectToString.TestObject3.Color2 ==
                            objectWithCustomParser.TestObject3.Color2);
            }

            // Normal test
            TestCustomParser();

            // Multi-Thread test
            G9Assembly.PerformanceTools.MultiThreadShockTest(_ => { TestCustomParser(); });

            var testObject = new CustomObject();
            var jsonTestObject = testObject.G9ObjectToJson(true);
            var parsedTestObject = jsonTestObject.G9JsonToObject<CustomObject>();

            Assert.True(parsedTestObject.CustomChild.Number1 == testObject.CustomChild.Number1 &&
                        parsedTestObject.CustomChild.Number2 == testObject.CustomChild.Number2 &&
                        parsedTestObject.CustomChild.Number3 == testObject.CustomChild.Number3);
        }


        [Test]
        [Order(6)]
        public void TestParsingWithIgnoreMismatching()
        {
            var smallObject = new G9DtSmallSampleClass();
            var json = smallObject.G9ObjectToJson();
            // {"Name":"Iman","Age":32,"Color":5}

            // Set incorrect string value according to object G9DtSmallSampleClass
            json = "{\"Name\":32,\"Age\":\"Iman\",\"Color\":5}";

            // It is expected an exception will occur because of the mismatching.
            try
            {
                json.G9JsonToObject<G9DtSmallSampleClass>();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message ==
                            $@"An exception occurred when the parser tried to parse the value 'Iman' for member '{nameof(G9DtSmallSampleClass.Age)}' in type '{typeof(G9DtSmallSampleClass).FullName}'.
If the value structure is correct, it seems that the default parser can't parse it, so that you can implement a custom parser for this type with the attribute '{nameof(G9AttrJsonMemberCustomParserAttribute)}'." &&
                            ex.InnerException is FormatException &&
                            ex.InnerException.Message == "Input string was not in a correct format.");
            }

            // It is expected an exception will occur because of the mismatching.
            // Test with custom parser
            try
            {
                json.G9JsonToObject<G9DtSmallSampleClassCustomParse>();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message ==
                            @"An exception occurred when the custom parser 'G9JSONHandler.Attributes.G9AttrJsonMemberCustomParserAttribute' tried to parse the value 'Iman' for member 'Age' in type 'G9JSONHandler_NUnitTest.DataTypeForTest.G9DtSmallSampleClassCustomParse'." &&
                            ex.InnerException is TargetInvocationException &&
                            ex.InnerException.Message == "Exception has been thrown by the target of an invocation." &&
                            ex.InnerException.InnerException is FormatException &&
                            ex.InnerException.InnerException.Message == "Input string was not in a correct format.");
            }

            // Ignore mismatching
            // both of them parsed without mismatch member
            var object1 = json.G9JsonToObject<G9DtSmallSampleClass>(true);
            Assert.True(object1.Name == "32" && object1.Color == ConsoleColor.DarkMagenta && Equals(object1.Age, default(int)));
            var object2 = json.G9JsonToObject<G9DtSmallSampleClassCustomParse>(true);
            Assert.True(object2.Name == "32" && object2.Color == ConsoleColor.DarkMagenta && Equals(object2.Age, default(int)));

        }

        [Test]
        [Order(6)]
        public void TestEncryptingAndDecryption()
        {
            void TestEncryptionAttr(int randomNumber)
            {
                // Creating an object with encryption/decryption attribute
                var objectWithEncryptionAttr = new G9DtSampleClassForEncryptionDecryption();
                objectWithEncryptionAttr.Expire = DateTime.Now.AddDays(randomNumber);

                // Test parsing object to json with auto encryption
                var jsonData = objectWithEncryptionAttr.G9ObjectToJson(true);
                Assert.True(jsonData.Contains("fESJe1TvMr00Q7BKTwVadg==") && jsonData.Contains("sWNkdxQ="));

                // Test parsing json to object with auto decryption
                var objectData = jsonData.G9JsonToObject<G9DtSampleClassForEncryptionDecryption>();
                Assert.True(objectWithEncryptionAttr.User == objectData.User &&
                            objectWithEncryptionAttr.Password == objectData.Password &&
                            objectWithEncryptionAttr.Expire.ToString("s") == objectData.Expire.ToString("s"));
            }

            G9Assembly.PerformanceTools.MultiThreadShockTest(TestEncryptionAttr, 99_999);
        }
    }
}