namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtPrivateMember
    {

        public G9DtPrivateMember(string name, string family,  int age)
        {
            _name = name;
            _family = family;
            Age = age;
        }

        private string _name;
        private static string _family;
        public int Age { get; }

        public string GetName()
        {
            return _name;
        }

        public string GetFamily()
        {
            return _family;
        }
    }
}