using System;
using System.Collections.Generic;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    /// <summary>
    /// Sample Object
    /// </summary>
    public class TestObject
    {
        [G9AttrComment("Custom JSON Comment")]
        public string Name = ".NET";
        public ConsoleColor Color = ConsoleColor.DarkMagenta;
        [G9AttrStoreEnumAsString]
        public ConsoleColor Color2 = ConsoleColor.DarkGreen;
        [G9AttrCustomName("MyRegisterDateTime")]
        public DateTime RegisterDateTime = new DateTime(1990, 9, 1);
        public TimeSpan Time = new TimeSpan(9, 9, 9);
        public string[] Array = { "Item 1", "Item 2", "Item 3" };
        public Dictionary<string, string> Dictionary = new Dictionary<string, string>()
            { { "Key 1", "Value 1" }, { "Key 2", "Value 2" }, { "Key 3", "Value 3" } };
        [G9AttrIgnore]
        public string Address = "...";
    }
}
