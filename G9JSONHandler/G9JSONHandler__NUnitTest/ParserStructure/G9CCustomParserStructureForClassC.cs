using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Abstract;

namespace G9JSONHandler_NUnitTest.ParserStructure
{
    public class G9CCustomParserStructureForClassC : G9ACustomTypeParserUnique<G9CClassC>
    {
        private int _testNumber = 3;

        public override string ObjectToString(G9CClassC objectForParsing, G9IMemberGetter accessToObjectMember)
        {
            return objectForParsing.A + "TM-" + (objectForParsing.B - _testNumber--);
        }

        public override G9CClassC StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            var data = stringForParsing.Split('-');
            return new G9CClassC
            {
                A = data[0],
                B = int.Parse(data[1])
            };
        }
    }
}