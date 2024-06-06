using System.Security.Cryptography;

namespace ChequePro.Models
{
    public class KeyGenerator
    {

        public static string GenerateApiKey(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);

                return Convert.ToBase64String(tokenData)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", ""); // Base64 URL-safe encoding
            }
        }
        
    }
}
