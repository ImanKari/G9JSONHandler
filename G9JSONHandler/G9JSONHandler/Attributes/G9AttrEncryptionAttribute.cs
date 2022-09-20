using System;
using System.Security.Cryptography;
using G9AssemblyManagement.DataType;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute enables encrypting/decrypting the data of a member while parsing.
    ///     <para />
    ///     Note: The specified member must have a convertible value to the string type.
    ///     <para />
    ///     Note: The priority of executing this attribute is higher than the others.
    ///     <para />
    ///     Note: If your member data type is complex, or you need to implement the custom encryption process, you can
    ///     implement a custom (encryption/decryption) process with the attribute
    ///     <see cref="G9AttrCustomParserAttribute" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false)]
    public class G9AttrEncryptionAttribute : Attribute
    {
        /// <summary>
        ///     Specifies the custom config for aes encryption/decryption
        /// </summary>
        public readonly G9DtAESConfig AesConfig;

        /// <summary>
        ///     Specifies the initialization vector (IV) for encryption/decryption
        /// </summary>
        public readonly string InitializationVector;

        /// <summary>
        ///     Specifies the private key for encryption/decryption
        /// </summary>
        public readonly string PrivateKey;

        /// <summary>
        ///     Constructor for encryption/decryption
        /// </summary>
        /// <param name="privateKey">Specifies the private key for encryption/decryption</param>
        /// <param name="iv">Specifies the initialization vector (IV) for encryption/decryption</param>
        /// <param name="paddingMode">Specifies the padding mode of cryptography.</param>
        /// <param name="cipherMode">Specifies the cipher mode of cryptography.</param>
        /// <param name="keySize">Specifies the key size for cryptography.</param>
        /// <param name="blockSize">Specifies the block size for cryptography.</param>
        /// <param name="enableAutoFixKeySize">
        ///     Specifies that if the key size isn't standard. The process must fix it or not. (With
        ///     an arbitrary process)
        /// </param>
        public G9AttrEncryptionAttribute(string privateKey, string iv,
            PaddingMode paddingMode = PaddingMode.PKCS7,
            CipherMode cipherMode = CipherMode.CBC, int keySize = 128, int blockSize = 128,
            bool enableAutoFixKeySize = false)
        {
            PrivateKey = privateKey;
            InitializationVector = iv;
            AesConfig = new G9DtAESConfig(paddingMode, cipherMode, keySize, blockSize, enableAutoFixKeySize);
        }
    }
}