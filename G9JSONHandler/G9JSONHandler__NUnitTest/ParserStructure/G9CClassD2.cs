namespace G9JSONHandler_NUnitTest.ParserStructure
{
    public class G9CClassD2
    {
        public string A = "G9";

        public G9CClassD<decimal> Extra = new()
        {
            A = "Okay",
            B = 9999.9999m
        };
    }
}