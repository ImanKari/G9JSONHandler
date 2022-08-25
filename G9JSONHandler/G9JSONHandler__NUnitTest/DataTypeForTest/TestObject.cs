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
        [G9AttrJsonComment("Custom JSON Comment", true)]
        public string Name = ".NET";
        public ConsoleColor Color = ConsoleColor.DarkMagenta;
        [G9AttrJsonStoreEnumAsString]
        public ConsoleColor Color2 = ConsoleColor.DarkGreen;
        [G9AttrJsonMemberCustomName("MyRegisterDateTime")]
        public DateTime RegisterDateTime = new(1990, 9, 1);
        public TimeSpan Time = new(9, 9, 9);
        public string[] Array = { "Item 1", "Item 2", "Item 3" };
        public Dictionary<string, string> Dictionary = new()
            { { "Key 1", "Value 1" }, { "Key 2", "Value 2" }, { "Key 3", "Value 3" } };
        [G9AttrJsonIgnoreMember]
        public string Address = "...";
    }
}
