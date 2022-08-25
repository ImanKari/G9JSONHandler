using System;
using System.Security.Cryptography;
using G9JSONHandler.Attributes;

namespace G9JSONHandler_NUnitTest.DataTypeForTest
{
    public class G9DtSampleClassForEncryptionDecryption
    {
        // With standard keys and default config
        [G9AttrJsonMemberEncryption("G-JaNdRgUkXp2s5v", "3t6w9z$C&F)J@NcR")]
        public string User = "G9TM";

        // With custom nonstandard keys and custom config
        [G9AttrJsonMemberEncryption("MyCustomKey", "MyCustomIV", PaddingMode.ANSIX923, CipherMode.CFB, enableAutoFixKeySize: true)]
        public string Password = "1990";

        // With custom nonstandard keys and custom config
        [G9AttrJsonMemberEncryption("MyCustomKey", "MyCustomIV", PaddingMode.ISO10126, CipherMode.ECB, enableAutoFixKeySize: true)]
        public DateTime Expire = DateTime.Now;
    }
}