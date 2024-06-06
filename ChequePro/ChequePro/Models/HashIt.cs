using System.Reflection.Metadata;
using System.Text;
using System.Security.Cryptography;

namespace ChequePro.Models
{
    internal class HashIt
    {
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
            { 
                ExceptionLogger.Log("GetHash error Exception " + ex); 
                return ""; 
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            var ret = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            ret = ret.Substring(0, ret.Length - 3);
            return ret;
        }
    }
}
