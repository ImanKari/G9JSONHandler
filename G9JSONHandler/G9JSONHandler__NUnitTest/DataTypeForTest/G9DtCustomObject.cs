using System.Collections.Generic;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public enum Gender : byte
    {
        Men,
        Women,
        Unknown
    }


    public class G9DtCustomObject
    {
        public Gender Gender = Gender.Unknown;

        public G9DtDotNetBuiltInTypes NestedObjectA = new();

        public G9DtDotNetBuiltInTypes NestedObjectB = new();
        public G9DtDotNetBuiltInTypes NestedObjectC = new();

        [G9AttrJsonComment("An Array For Test")] public string[] TestArray =
            { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8", "Item 9" };

        [G9AttrJsonComment("A Dictionary For Test")] public Dictionary<string, string> TestDictionary = new()
        {
            { "Key 1", "Value 1" }, { "Key 2", "Value 2" }, { "Key 3", "Value 3" }, { "Key 4", "Value 4" },
            { "Key 5", "\"Value 5" }, { "Key 6", "\"Value 6\"" }, { "Key 7", "Value 7\"" }, { "Key 8", "\"Value 8" },
            { "Key 9", "Value 9" }
        };

        public string FullName { set; get; } = "\"Iman\"Kari\"";

        public int Age { set; get; } = 32;
    }
}