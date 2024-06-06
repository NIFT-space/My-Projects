using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using IBCS.Data;

namespace NIFT_CMS
{
    public class EncDec
    {
        private byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private int BlockSize = 128;
        public string Encrypt(string Pass_)
        {
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.IV = IV;
                HashAlgorithm hash = MD5.Create();
                aes.Key = hash.ComputeHash(Encoding.Unicode.GetBytes("J2AxF88F"));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                byte[] src = Encoding.Unicode.GetBytes(Pass_);
                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] destin = encrypt.TransformFinalBlock(src, 0, src.Length);
                    return Convert.ToBase64String(destin);
                }
            }
            catch (Exception ex)
            { LogWriter.WriteToLog(ex); return ""; }
        }
        public string decrypt(string Enc_Pass)
        {
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.IV = IV;
                HashAlgorithm hash = MD5.Create();
                aes.Key = hash.ComputeHash(Encoding.Unicode.GetBytes("J2AxF88F"));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                byte[] src = System.Convert.FromBase64String(Enc_Pass);
                using (ICryptoTransform decrypt = aes.CreateDecryptor())
                {
                    byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                    return Encoding.Unicode.GetString(dest);
                }
            }
            catch (Exception ex)
            { LogWriter.WriteToLog(ex); return ""; }
        }
        public string GetHash(string Code)
        {
            try
            {
                string HASH;
                SHA256Managed shaHash = new SHA256Managed();

                Byte[] sText;
                StringBuilder objSB = new StringBuilder();

                objSB.Append(Code);

                sText = Encoding.UTF8.GetBytes(objSB.ToString());
                Byte[] sHash_utf8;
                sHash_utf8 = shaHash.ComputeHash(sText);
                HASH = Convert.ToBase64String(sHash_utf8);
                return HASH;
            }
            catch (Exception ex)
            { LogWriter.WriteToLog(ex); return ""; }
        }
        public string UndoHash(byte[] hash)
        {
            string hashPassword = BitConverter.ToString(hash).Replace("-", "");
            return hashPassword;

        }
    }
}