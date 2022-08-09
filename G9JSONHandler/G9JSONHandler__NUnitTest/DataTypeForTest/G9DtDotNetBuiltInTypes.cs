using System;
using System.Net;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtDotNetBuiltInTypes
    {
        public char A = '9';
        public string B = "G9TM";
        public float C = 9.9f;
        public double C2 = 99.99;
        public decimal C3 = 999.999m;
        public bool D = true;

        [G9JsonComment("1- This note comment is used just for tests!")]
        [G9JsonComment("2- This note comment is used just for tests!")]
        [G9JsonComment("3- This note comment is used just for tests!")]
        public DateTime E = DateTime.Parse("1990/09/01 09:09:09");

        public sbyte F1 = 3;
        public byte F2 = 6;
        public short F3 = 9;
        public ushort F4 = 13;

        [G9JsonComment("1- This note comment is used just for tests!")]
        [G9JsonComment("2- This note comment is used just for tests!")]
        [G9JsonComment("3- This note comment is used just for tests!")]
        public int F5 = 16;

        public uint F6 = 19;
        public long F7 = 23;
        public ulong F8 = 26;
        public TimeSpan G = new(9, 9, 9);
        public IPAddress H = IPAddress.Loopback;

    }
}