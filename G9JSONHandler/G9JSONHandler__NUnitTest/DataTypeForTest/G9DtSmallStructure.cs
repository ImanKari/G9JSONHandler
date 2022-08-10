namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtSmallStructure
    {
        private static int counter = 1;

        public string A = $"Test {counter}";
        public int B = 0 + counter;
        public bool C = counter++ % 2 == 0;
    }
}