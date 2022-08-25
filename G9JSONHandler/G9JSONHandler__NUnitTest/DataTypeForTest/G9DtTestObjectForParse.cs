using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtTestObjectForParse
    {
        public string TestMultiLine = @"
            Iman Kari
            Sep, 01, 1990
            G9TM
            ";

        
#pragma warning disable CS0414
        // ReSharper disable once InconsistentNaming
        private string A1 = "G9TM1";
#pragma warning restore CS0414

        [G9AttrJsonComment("1- This note comment is used just for tests! Nonstandard Type!", true)]
        public string A2 = "\"G9\"TM2\"";

        [G9AttrJsonComment("Test object array!")]
        public G9DtSmallStructure[] AAA = new[]
        {
            new G9DtSmallStructure(), new G9DtSmallStructure(), new G9DtSmallStructure()
        };

        [G9AttrJsonComment("1- This note comment is used just for tests!")]
        public G9DtCustomObject CustomObject = new();

        [G9AttrJsonComment("1- This note comment is used just for tests!")]
        [G9AttrJsonComment("2- This note comment is used just for tests!")]
        [G9AttrJsonComment("3- This note comment is used just for tests!")]
        public G9DtDotNetBuiltInTypes DotNetBuiltInTypes = new();

        [G9AttrJsonStoreEnumAsString] [G9AttrJsonMemberCustomName("ChangeToCustomName")]
        public Gender Gender = Gender.Unknown;

        [G9AttrJsonIgnoreMember] public string TestIgnoreCase = "Okay";

        private int B1 { set; get; } = 333;

        public int B2 { set; get; } = 999;

        public int B3 { set; private get; } = 369;

        public int B4 { get; } = 963;

        [G9AttrJsonStoreEnumAsString]
        [G9AttrJsonComment("1- This note comment is used just for tests!")]
        [G9AttrJsonComment("2- This note comment is used just for tests!")]
        [G9AttrJsonComment("3- This note comment is used just for tests!")]
        public Gender Gender2 { set; get; } = Gender.Women;
    }
}