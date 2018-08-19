using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Security
{
    public static class Encryption
    {
        public static string GetNewSalt(int saltLengthLimit)
        {
            string _salt = GenerateSalt(saltLengthLimit);

            return _salt;
        }

        private static string GenerateSalt(int saltLengthLimit)
        {
            var salt = new byte[saltLengthLimit];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return Encoding.ASCII.GetString(salt);
        }

        public static string EncryptPassword(string salt, string password)
        {
            string saltPwd = password + salt;

            var sha = new SHA256CryptoServiceProvider();

            string encryptedPassword = Encoding.ASCII.GetString(sha.ComputeHash(Encoding.ASCII.GetBytes(saltPwd)));

            return encryptedPassword;
        }

        public static string EncryptString(string toBeEncrypted)
        {
            // https://stackoverflow.com/questions/9031537/really-simple-encryption-with-c-sharp-and-symmetricalgorithm

            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(toBeEncrypted);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        private static byte[] key = new byte[8] { 27, 19, 120, 244, 31, 118, 68, 127 };
        private static byte[] iv = new byte[8] { 206, 50, 119, 83, 120, 100, 189, 9 };

        public static string EncryptString(object toBeEncrypted)
        {
            return EncryptString(toBeEncrypted.ToString());
        }

        public static string DecryptString(string toBeDecrypted)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(toBeDecrypted);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}
