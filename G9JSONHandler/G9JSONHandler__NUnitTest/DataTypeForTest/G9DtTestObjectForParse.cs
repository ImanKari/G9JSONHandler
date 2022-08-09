using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtTestObjectForParse
    {
        [G9JsonComment("1- This note comment is used just for tests!")]
        private string A1 = "G9TM1";

        public string A2 = "G9TM2";

        [G9JsonComment("1- This note comment is used just for tests!")]
        public G9DtCustomObject CustomObject = new();

        [G9JsonComment("1- This note comment is used just for tests!")]
        [G9JsonComment("2- This note comment is used just for tests!")]
        [G9JsonComment("3- This note comment is used just for tests!")]
        public G9DtDotNetBuiltInTypes DotNetBuiltInTypes = new();

        [G9JsonStoreEnumAsString] [G9JsonCustomMemberName("ChangeToCustomName")]
        public Gender Gender = Gender.Unknown;

        [G9JsonIgnoreMember] public string TestIgnoreCase = "Okay";

        private int B1 { set; get; } = 333;

        public int B2 { set; get; } = 999;

        public int B3 { set; private get; } = 369;

        public int B4 { get; } = 963;

        [G9JsonStoreEnumAsString]
        [G9JsonComment("1- This note comment is used just for tests!")]
        [G9JsonComment("2- This note comment is used just for tests!")]
        [G9JsonComment("3- This note comment is used just for tests!")]
        public Gender Gender2 { set; get; } = Gender.Women;
    }
}