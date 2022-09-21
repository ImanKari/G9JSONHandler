using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtOrders
    {
        [G9AttrOrder(3)]
        public string Order3 = "Order3";
        [G9AttrOrder(4)]
        public string Order4 = "Order4";
        [G9AttrOrder(8)]
        public string Order8 = "Order8";
        [G9AttrOrder(9)]
        public string Order9 = "Order9";
        [G9AttrOrder(1)]
        public string Order1 = "Order1";
        [G9AttrOrder(2)]
        public string Order2 = "Order2";
        [G9AttrOrder(6)]
        public string Order6 = "Order6";

        public string WithoutOrder = "WithoutOrder";
        [G9AttrOrder(7)]
        public string Order7 = "Order7";
        [G9AttrOrder(5)]
        public string Order5 = "Order5";
    }
}