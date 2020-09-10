using System;
using System.Security.Cryptography;
using System.Text;

namespace CommonTool
{
    /// <summary>
    /// MD5
    /// 单向加密
    /// </summary>
    public class EncryptionMD5
    {
        /// <summary>
        /// 获得一个字符串的加密密文
        /// 此密文为单向加密，即不可逆(解密)密文
        /// </summary>
        /// <param name="plainText">待加密明文</param>
        /// <returns>已加密密文</returns>
        public static string EncryptStringMD5(string plainText)
        {
            string encryptText = "";
            if (string.IsNullOrEmpty(plainText)) return encryptText;
            //encryptText = FormsAuthentication.HashPasswordForStoringInConfigFile(plainText, "md5");
            return encryptText;
        }
    }

    /// <summary>
    /// SHA1
    /// 单向加密
    /// </summary>
    public class EncryptionSHA1
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 获得一个字符串的加密密文
        /// </summary>
        /// <param name="plainTextString"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GenerateSaltedSHA1Password(string plainTextString, string salt)
        {
            byte[] passwordBytes = encoding.GetBytes(plainTextString);
            byte[] saltBytes = StrToToHexByte(salt);

            byte[] buffer = new byte[passwordBytes.Length + saltBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, buffer, saltBytes.Length, passwordBytes.Length);

            var hashAlgorithm = HashAlgorithm.Create("SHA1");
            var s = hashAlgorithm.ComputeHash(buffer);

            byte[] saltedSHA1Bytes = s;
            for (int i = 1; i < 1024; i++)
            {
                saltedSHA1Bytes = hashAlgorithm.ComputeHash(saltedSHA1Bytes);
            }
            return salt + ByteToHexStr(saltedSHA1Bytes);
        }

        private static byte[] StrToToHexByte(string hexString)
        {
            int NumberChars = hexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("x2");
                }
            }
            return returnStr;
        }
    }

    /// <summary>
    /// 单例模式的aws加密解密类
    /// </summary>
    public class AESHelper
    {
        private static string Key;

        public AESHelper(string key)
        {
            Key = key;
        }

        #region AES加解密

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AesEncrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AesDecrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(str);

                RijndaelManaged rm = new RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(Key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return str;
            }
        }

        #endregion AES加解密
    }
}