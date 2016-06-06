using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SCMS.CoreBusinessLogic.Security
{
    public class EncryptionService : IEncryptionService
    {
        private static readonly string SystemSalt = "C5^7uj6+";

        public virtual string CreateSaltKey(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        public string GetSHA256(string value, String salt, bool useSystemSalt)
        {
            var sha256 = SHA256.Create();

            byte[] data = sha256.ComputeHash(Encoding.Default.GetBytes((useSystemSalt ? SystemSalt : "")+value+salt));

            return data.Select(b => b.ToString("x2")).StringJoin("", "");
        }

    }
}
