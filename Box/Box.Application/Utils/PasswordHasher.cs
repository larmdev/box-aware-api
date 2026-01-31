using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Box.Application.Services
{
    interface IPasswordHasher
    {
        string Hash(string password, out string salt);
        string EncryptPassword(string rawPassword, out string salt);
        string DecryptPassword(string cipherText, string salt);
        bool VerifyPassword(string rawPassword, string cipherText, string salt);
        bool Verify(string password, string hash, string salt);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private const string EncryptionKey = "QIaT2hgh3EbDYzERIAOxfBIhkVqHlhDdM9xKIuunTMF48GLkCDSJpRDaeNlU8Pla";
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;

        public string Hash(string password, out string salt)
        {
            var _salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, _salt, Iterations, _hashAlgorithmName, KeySize);
            salt = Convert.ToBase64String(_salt);
            return Convert.ToBase64String(hash);
        }

        public string EncryptPassword(string rawPassword, out string salt)
        {
            var _salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] clearBytes = Encoding.Unicode.GetBytes(rawPassword);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, _salt, Iterations, _hashAlgorithmName);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    rawPassword = Convert.ToBase64String(ms.ToArray());
                }
            }
            salt = Convert.ToBase64String(_salt);
            return rawPassword;
        }

        public string DecryptPassword(string encryptedText, string salt)
        {
            var _salt = Convert.FromBase64String(salt);
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, _salt, Iterations, _hashAlgorithmName);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encryptedText;
        }

        public bool VerifyPassword(string rawPassword, string? cipherText, string salt)
        {
            var clearPassword = DecryptPassword(cipherText ?? string.Empty, salt);
            return rawPassword.Equals(clearPassword);
        }

        public bool Verify(string rawPassword, string passwordHash, string salt)
        {
            var _salt = Convert.FromBase64String(salt);
            var hash = Convert.FromBase64String(passwordHash);
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(rawPassword, _salt, Iterations, _hashAlgorithmName, KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
        }
    }
}
