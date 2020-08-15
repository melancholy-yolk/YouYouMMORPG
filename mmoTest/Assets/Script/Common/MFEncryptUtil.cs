using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

    /// <summary>
    /// ���ܽ��� MD5 SHA1
    /// </summary>
    public static class MFEncryptUtil
    {
        #region DES���ܽ���
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        public static string KeyDES = "87654321";

        /// <summary>
        /// DES�����ַ���
        /// </summary>
        /// <param name="encryptString">�����ܵ��ַ���</param>
        /// <param name="encryptKey">������Կ,Ҫ��Ϊ8λ</param>
        /// <returns>���ܳɹ����ؼ��ܺ���ַ�����ʧ�ܷ���Դ��</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES�����ַ���
        /// </summary>
        /// <param name="decryptString">�����ܵ��ַ���</param>
        /// <param name="decryptKey">������Կ,Ҫ��Ϊ8λ,�ͼ�����Կ��ͬ</param>
        /// <returns>���ܳɹ����ؽ��ܺ���ַ�����ʧ�ܷ�Դ��</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        #endregion

        #region Sha1
        /// <summary>
        /// Sha1����
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static string Sha1(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            System.Security.Cryptography.SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] bytResult = sha1.ComputeHash(System.Text.Encoding.Default.GetBytes(value));
            string strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult;

        }
        #endregion Sha1

        #region Md5
        /// <summary>
        /// Md5����
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static string Md5(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(value));
            string strResult = BitConverter.ToString(bytResult);
            strResult = strResult.Replace("-", "");
            return strResult;
        }

        /// <summary>
        /// Md5����
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="Is16">�Ƿ�16λ����</param>
        /// <returns></returns>
        public static string Md5(this string value, bool Is16)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (Is16)
            {

                return value.Md5().Substring(8, 16);
            }
            else
            {
                return value.Md5();
            }
        }
        #endregion Md5
    }
