using System;
using System.Linq;
using System.Net;
using System.Reflection;
using G9AssemblyManagement;
using G9AssemblyManagement.Enums;
using G9JSONHandler;
using G9JSONHandler.Attributes;
using G9JSONHandler.DataType;
using G9JSONHandler.Enum;
using G9JSONHandler_NUnitTest.DataTypeForTest;
using G9JSONHandler_NUnitTest.ParserStructure;
using NUnit.Framework;

namespace G9JSONHandler_NUnitTest
{
    public class G9JSONHandlerNUnitTest
    {
#if NET35
        private readonly bool _isDotNet35 = true;
#else
        private readonly bool _isDotNet35 = false;
#endif

        // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
        private readonly G9DtTestObjectForParse testObjectForParsing = new G9DtTestObjectForParse();
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
            testJSONString_Unformatted = G9JSON.ObjectToJson(testObjectForParsing,
                new G9DtJsonWriterConfig(commentMode: G9ECommentMode.StandardMode));
            Assert.False(string.IsNullOrEmpty(testJSONString_Unformatted));

            // Converting to JSON by formatted type
            testJSONString_Formatted = G9JSON.ObjectToJson(testObjectForParsing,
                new G9DtJsonWriterConfig(G9EAccessModifier.Public,
                    true, G9ECommentMode.NonstandardMode));
            Assert.False(string.IsNullOrEmpty(testJSONString_Formatted));

            var nonstandardComment = "/* 1- This note comment is used just for tests! Nonstandard Type! */";
            var standardComment =
                "\"#__Comment0__#\":\"1- This note comment is used just for tests! Nonstandard Type!\"";

            Assert.True(testJSONString_Unformatted.Contains(standardComment) &&
                        testJSONString_Formatted.Contains(nonstandardComment));
        }

        [Test]
        [Order(2)]
        public void TestJSONToObject()
        {
            // Converting to object by unformatted type
            var testObjectUnformattedType = G9JSON.JsonToObject<G9DtTestObjectForParse>(testJSONString_Unformatted);
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
            var testObjectFormattedType = G9JSON.JsonToObject<G9DtTestObjectForParse>(testJSONString_Formatted);
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
            var unformattedJson = G9JSON.ObjectToJson(testObject);
            // Formatted JSON
            var formattedJson =
                G9JSON.ObjectToJson(testObject, new G9DtJsonWriterConfig(G9EAccessModifier.Public, true));

            // Test
            var newObject = G9JSON.JsonToObject<TestObject>(unformattedJson);
            Assert.True(newObject.Name == testObject.Name);
            Assert.True(newObject.Color == testObject.Color);
            Assert.True(newObject.Array.Length == testObject.Array.Length);
            Assert.True(newObject.Dictionary.Count == testObject.Dictionary.Count);
            Assert.True(newObject.Dictionary["Key 2"] == testObject.Dictionary["Key 2"]);

            newObject = G9JSON.JsonToObject<TestObject>(formattedJson);
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
                var unformattedJson = G9JSON.ObjectToJson(testObject);
                var formattedJson =
                    G9JSON.ObjectToJson(testObject, new G9DtJsonWriterConfig(G9EAccessModifier.Public, true));
                var newObject = G9JSON.JsonToObject<TestObject>(unformattedJson);
                var newObject2 = G9JSON.JsonToObject<TestObject>(formattedJson);
                Assert.True(newObject.Name == testObject.Name);
                Assert.True(newObject2.Name == testObject.Name);
            }, _isDotNet35 ? 999 : 99_999);
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
                var stringJson = G9JSON.ObjectToJson(objectWithCustomParser,
                    new G9DtJsonWriterConfig(G9EAccessModifier.Public, true));
                Assert.True(!string.IsNullOrEmpty(stringJson));

                // Test custom parsing process for json to object (The custom values is set in custom parsing)
                var objectFromJson = G9JSON.JsonToObject<G9DtTestForCustomParsingProcess>(stringJson);
                Assert.True(objectFromJson.TestObject1.Name == objectWithCustomParser.TestObject1.Name + "Okay" &&
                            objectFromJson.TestObject2.Time ==
                            objectWithCustomParser.TestObject2.Time.Add(new TimeSpan(0, 9, 0)) &&
                            objectFromJson.TestObject3.Color == objectWithCustomParser.TestObject3.Color2 &&
                            objectFromJson.TestObject3.Color2 == objectWithCustomParser.TestObject3.Color2);

                // Test custom parsing process just for json to object
                var objectWithCustomParserJsonToObject = new G9DtTestForCustomParsingProcessObjectToString();
                stringJson = G9JSON.ObjectToJson(objectWithCustomParserJsonToObject,
                    new G9DtJsonWriterConfig(G9EAccessModifier.Public, true));

                // Test custom parsing process just for object to json
                var objectFromJsonObjectToString =
                    G9JSON.JsonToObject<G9DtTestForCustomParsingProcess>(stringJson);
                Assert.True(objectFromJsonObjectToString.TestObject1.Name ==
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
            G9Assembly.PerformanceTools.MultiThreadShockTest(_ => { TestCustomParser(); }, _isDotNet35 ? 999 : 99_999);

            var testObject = new CustomObject();
            var jsonTestObject =
                G9JSON.ObjectToJson(testObject, new G9DtJsonWriterConfig(G9EAccessModifier.Public, true));
            var parsedTestObject = G9JSON.JsonToObject<CustomObject>(jsonTestObject);

            Assert.True(parsedTestObject.CustomChild.Number1 == testObject.CustomChild.Number1 &&
                        parsedTestObject.CustomChild.Number2 == testObject.CustomChild.Number2 &&
                        parsedTestObject.CustomChild.Number3 == testObject.CustomChild.Number3);
        }


        [Test]
        [Order(6)]
        public void TestParsingWithIgnoreMismatching()
        {
            var smallObject = new G9DtSmallSampleClass();
            var json = G9JSON.ObjectToJson(smallObject);
            // {"Name":"Iman","Age":32,"Color":5}

            // Set incorrect string value according to object G9DtSmallSampleClass
            json = "{\"Name\":32,\"Age\":\"Iman\",\"Color\":5}";

            // It is expected an exception will occur because of the mismatching.
            try
            {
                G9JSON.JsonToObject<G9DtSmallSampleClass>(json);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message ==
                            $@"An exception occurred when the parser tried to parse the value 'Iman' for member '{nameof(G9DtSmallSampleClass.Age)}' in type '{typeof(G9DtSmallSampleClass).FullName}'.
If the value structure is correct, it seems that the default parser can't parse it, so that you can implement a custom parser for this type with the attribute '{nameof(G9AttrCustomParserAttribute)}'." &&
                            ex.InnerException is FormatException &&
                            ex.InnerException.Message == "Input string was not in a correct format.");
            }

            // It is expected an exception will occur because of the mismatching.
            // Test with custom parser
            try
            {
                G9JSON.JsonToObject<G9DtSmallSampleClassCustomParse>(json);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message ==
                            $@"An exception occurred when the custom parser '{typeof(G9AttrCustomParserAttribute).FullName}' tried to parse the value 'Iman' for member 'Age' in type 'G9JSONHandler_NUnitTest.DataTypeForTest.G9DtSmallSampleClassCustomParse'." &&
                            ex.InnerException is TargetInvocationException &&
                            ex.InnerException.Message == "Exception has been thrown by the target of an invocation." &&
                            ex.InnerException.InnerException is FormatException &&
                            ex.InnerException.InnerException.Message == "Input string was not in a correct format.");
            }

            // Ignore mismatching
            // both of them parsed without mismatch member
            var object1 = G9JSON.JsonToObject<G9DtSmallSampleClassCustomParse>(json,
                new G9DtJsonParserConfig(G9EAccessModifier.Public, true));
            Assert.True(object1.Name == "32" && object1.Color == ConsoleColor.DarkMagenta &&
                        Equals(object1.Age, default(int)));
            var object2 = G9JSON.JsonToObject<G9DtSmallSampleClassCustomParse>(json,
                new G9DtJsonParserConfig(G9EAccessModifier.Public, true));
            Assert.True(object2.Name == "32" && object2.Color == ConsoleColor.DarkMagenta &&
                        Equals(object2.Age, default(int)));
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
                var jsonData = G9JSON.ObjectToJson(objectWithEncryptionAttr,
                    new G9DtJsonWriterConfig(G9EAccessModifier.Public, true));
                Assert.True(jsonData.Contains("fESJe1TvMr00Q7BKTwVadg=="));

                // Test parsing json to object with auto decryption
                var objectData = G9JSON.JsonToObject<G9DtSampleClassForEncryptionDecryption>(jsonData);
                Assert.True(objectWithEncryptionAttr.User == objectData.User &&
                            objectWithEncryptionAttr.Password == objectData.Password &&
                            objectWithEncryptionAttr.Expire.ToString("s") == objectData.Expire.ToString("s"));
            }

            G9Assembly.PerformanceTools.MultiThreadShockTest(TestEncryptionAttr, _isDotNet35 ? 999 : 99_999);
        }

        [Test]
        [Order(7)]
        public void TestCustomParserStructure()
        {
            void TestCustomParserStructure(int randomNumber)
            {
                // Test setting a custom type parser on type G9CClassA.
                var testClass = new G9CClassA();

                // Test added comment by parser
                var jsonData = G9JSON.ObjectToJson(testClass,
                    new G9DtJsonWriterConfig(isFormatted: true, commentMode: G9ECommentMode.NonstandardMode));
                Assert.True(jsonData.Contains("This Comment added by custom comment process in custom parser."));
                var objectData = G9JSON.JsonToObject<G9CClassA>(jsonData);
                Assert.True(objectData.A == "G9TM" && objectData.B == 6);

                // Test setting a custom type parser on type G9CClassA that is a child of another type (G9CClassB).
                var testClass2 = new G9CClassB();
                var jsonData2 = G9JSON.ObjectToJson(testClass2);
                var objectData2 = G9JSON.JsonToObject<G9CClassB>(jsonData2);
                Assert.True(objectData2.A == "G9" && objectData2.B == 99 && objectData2.Extra.A == "G9TM" &&
                            objectData2.Extra.B == 6);


                // Set incorrect string value according to objects G9CClassA, G9CClassB
                var jsonWrongData = "{\"G9TM-G9TM\"}";
                var jsonWrongData1 = "{\"A\":\"G9\",\"B\":99,\"Extra\":\"G9TM-G9TM\"}";

                // It is expected an exception will occur because of the mismatching.
                try
                {
                    G9JSON.JsonToObject<G9CClassA>(jsonWrongData);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    Assert.True(e.Message ==
                                "An exception occurred when the custom parser 'G9JSONHandler_NUnitTest.ParserStructure.G9CCustomParserStructureForClassA' tried to parse the value '{\"G9TM-G9TM\"}' for type 'G9JSONHandler_NUnitTest.ParserStructure.G9CClassA'.");
                }

                // It is expected an exception will occur because of the mismatching.
                try
                {
                    G9JSON.JsonToObject<G9CClassB>(jsonWrongData1);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    Assert.True(e.Message ==
                                "An exception occurred when the custom parser 'G9JSONHandler_NUnitTest.ParserStructure.G9CCustomParserStructureForClassA' tried to parse the value 'G9TM-G9TM' for member 'Extra' in type 'G9JSONHandler_NUnitTest.ParserStructure.G9CClassB'.");
                }

                // Ignore mismatching
                var objectTest = G9JSON.JsonToObject<G9CClassB>(jsonWrongData1,
                    new G9DtJsonParserConfig(G9EAccessModifier.Public, true));
                Assert.True(objectTest.A == "G9" && objectTest.B == 99 && objectTest.Extra == null);


                // Test setting a custom unique type parser on type G9CClassC.
                var testClassUnique = new G9CClassC();
                var jsonDataUnique = G9JSON.ObjectToJson(testClassUnique);
                var objectDataUnique = G9JSON.JsonToObject<G9CClassC>(jsonDataUnique);
                Assert.True(objectDataUnique.A == "G9TM" && objectDataUnique.B == 96);

                // Test setting a custom generic type parser on type G9CClassD
                var testGenericClassA = new G9CClassD<int>();
                var jsonDataGenericA = G9JSON.ObjectToJson(testGenericClassA,
                    new G9DtJsonWriterConfig(isFormatted: true, commentMode: G9ECommentMode.StandardMode));
                Assert.True(jsonDataGenericA.Contains("\"G9-99\""));
                var objectDataGenericA = G9JSON.JsonToObject<G9CClassD<int>>(jsonDataGenericA);
                Assert.True(objectDataGenericA.A == "G9" && objectDataGenericA.B == 99);

                var testGenericClassB = new G9CClassD<string>();
                var jsonDataGenericB = G9JSON.ObjectToJson(testGenericClassB);
                Assert.True(jsonDataGenericB.Contains("{\"G9-None\"}"));
                var objectDataGenericB = G9JSON.JsonToObject<G9CClassD<string>>(jsonDataGenericB);
                Assert.True(objectDataGenericB.A == "G9" && objectDataGenericB.B == "None");

                // Test setting a custom generic type parser on type G9CClassD as a child
                var testGenericClassC = new G9CClassD2();
                var jsonDataGenericC = G9JSON.ObjectToJson(testGenericClassC);
                Assert.True(
                    jsonDataGenericC.Contains(
                        "This Comment added by custom comment process in custom parser. Test 1."));
                Assert.True(
                    jsonDataGenericC.Contains(
                        "This Comment added by custom comment process in custom parser. Test 2."));
                Assert.True(
                    jsonDataGenericC.Contains(
                        "This Comment added by custom comment process in custom parser. Test 3."));
                Assert.True(jsonDataGenericC.Contains("\"A\":\"G9\"") &&
                            jsonDataGenericC.Contains("\"Extra\":\"Okay-9999.9999\""));
                var objectDataGenericC = G9JSON.JsonToObject<G9CClassD2>(jsonDataGenericC);
                Assert.True(objectDataGenericC.A == "G9" && objectDataGenericC.Extra.A == "Okay" &&
                            objectDataGenericC.Extra.B == 9999.9999m);
            }

            TestCustomParserStructure(0);

            G9Assembly.PerformanceTools.MultiThreadShockTest(TestCustomParserStructure, _isDotNet35 ? 999 : 99_999);
        }

        [Test]
        [Order(8)]
        public void TestGetTotalCustomParser()
        {
            var customParserTypes = G9JSON.GetTotalCustomParser();
            Assert.True(customParserTypes.Length == 3 &&
                        customParserTypes.Any(s =>
                            s.Name == nameof(G9CCustomParserStructureForClassA) ||
                            s.Name == nameof(G9CCustomParserStructureForClassC) ||
                            s.Name == nameof(G9CCustomParserStructureForGenericTypes)
                        ));
        }


        [Test]
        [Order(9)]
        public void TestJsonProcessWithCustomAccessModifiers()
        {
            // This object has two private fields, the first one is normal field and the second one is static field.
            // In addition, it has a public property with private setter.
            var privateObject = new G9DtPrivateMember("G9", "TM", 39);

            // First test, must has Age property
            var jsonA = G9JSON.ObjectToJson(privateObject);
            Assert.That(jsonA, Is.EqualTo("{\"Age\":39}"));

            var objectJsonA = G9JSON.JsonToObject<G9DtPrivateMember>(jsonA);
            Assert.True(objectJsonA.Age == 0 && objectJsonA.GetName() != privateObject.GetName() &&
                        objectJsonA.GetFamily() == privateObject.GetFamily());

            // The second test, according to modifier, must have two fields and one property
            var jsonB = G9JSON.ObjectToJson(privateObject, new G9DtJsonWriterConfig(G9EAccessModifier.Everything));
            Assert.That(jsonB,
                Is.EqualTo("{\"_name\":\"G9\",\"<Age>k__BackingField\":39,\"_family\":\"TM\",\"Age\":39}"));

            var objectJsonB =
                G9JSON.JsonToObject<G9DtPrivateMember>(jsonB, new G9DtJsonParserConfig(G9EAccessModifier.Everything));
            Assert.True(objectJsonB.Age == 39 && objectJsonB.GetName() == privateObject.GetName() &&
                        objectJsonB.GetFamily() == privateObject.GetFamily());
        }

        [Test]
        [Order(9)]
        public void TestTheOrderAttributes()
        {
            var orderObject = new G9DtOrders();

            var jsonOrder = G9JSON.ObjectToJson(orderObject, new G9DtJsonWriterConfig(isFormatted: true));

            var lines = jsonOrder.Split('\n').Skip(1).Take(10).ToArray();
            for (var i = 1; i <= 9; i++)
                Assert.True(lines[i - 1].Contains($"\"Order{i}\": \"Order{i}\""));

            Assert.True(lines.Last().Contains("\"WithoutOrder\": \"WithoutOrder\""));
        }
    }
}