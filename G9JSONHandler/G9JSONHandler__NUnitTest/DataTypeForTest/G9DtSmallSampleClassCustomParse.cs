using System;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public static class CustomParserTest
    {
        public static object StringToObject(string value, G9IMemberGetter member)
        {
            return int.Parse(value);
        }
    }

    public class G9DtSmallSampleClassCustomParse
    {
        public string Name = "Iman";
        [G9AttrCustomParser(true, typeof(CustomParserTest), nameof(CustomParserTest.StringToObject))]
        public int Age = 32;

        public ConsoleColor Color = ConsoleColor.DarkMagenta;
        
    }
}