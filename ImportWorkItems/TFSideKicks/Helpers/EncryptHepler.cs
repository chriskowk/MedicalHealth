using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TFSideKicks.Helpers
{
    public static class EncryptHepler
    {
        const string EncryptKey = "09BC1C3A-F043-49F3-B15F-EB9EE1F22C7E";//加密密钥
        private static SymmetricAlgorithm _symmetricAlgorithm;

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            return CryptographyHelper.Encrypt(SymmetricAlgorithm,
                plainText, EncryptKey, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="base64Text"></param>
        /// <returns></returns>
        public static string Decrypt(string base64Text)
        {
            return CryptographyHelper.Decrypt(SymmetricAlgorithm,
                base64Text, EncryptKey, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        private static SymmetricAlgorithm SymmetricAlgorithm
        {
            get
            {
                if (_symmetricAlgorithm == null)
                {
                    _symmetricAlgorithm = CryptographyHelper.CreateSymmAlgoTripleDes(); //3DES算法
                    _symmetricAlgorithm.IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
                }
                return _symmetricAlgorithm;
            }
        }
    }
}
