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
    }
}
