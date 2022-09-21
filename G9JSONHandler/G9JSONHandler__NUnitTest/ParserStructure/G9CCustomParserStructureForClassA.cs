using System;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Abstract;

namespace G9JSONHandler_NUnitTest.ParserStructure
{
    public class G9CCustomParserStructureForClassA : G9ACustomTypeParser<G9CClassA>
    {
        public override string ObjectToString(G9CClassA objectForParsing, G9IMemberGetter accessToObjectMember, Action<string> addCustomComment)
        {
            addCustomComment("This Comment added by custom comment process in custom parser.");
            return objectForParsing.A + "TM-" + (objectForParsing.B - 3);
        }

        public override G9CClassA StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
        {
            var data = stringForParsing.Split('-');

            return new G9CClassA()
            {
                A = data[0],
                B = int.Parse(data[1])
            };
        }
    }
}