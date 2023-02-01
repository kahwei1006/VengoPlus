using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Lavie.Crypto
{
    public class Aes256 : IDisposable
    {
        private static readonly string KEY = "R3t@s!#0265_@*Lv";
        private static readonly byte[] iv = Encoding.UTF8.GetBytes(KEY);
        private static readonly byte[] digest = SHA256.Create().ComputeHash(iv);

        private AesManaged aes = null;
        private ICryptoTransform encryptor = null;
        private ICryptoTransform decryptor = null;

        public Aes256()
        {
            aes = new AesManaged
            {
                Mode = CipherMode.CBC,
                IV = iv,
                Key = digest,
                Padding = PaddingMode.PKCS7
            };
            encryptor = aes.CreateEncryptor();
            decryptor = aes.CreateDecryptor();
        }

        public string Encrypt(string input)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch
            {
                return "";
            }
        }

        public string Decrypt(string inputBase64)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(inputBase64);
                return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch
            {
                return "";
            }
        }

        public static string EncryptSafe(string input)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(input);
                using (var aes = new AesManaged())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.IV = iv;
                    aes.Key = digest;
                    aes.Padding = PaddingMode.PKCS7;
                    return Convert.ToBase64String(aes.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
                }
            }
            catch
            {
                return "";
            }
        }

        public static string DecryptSafe(string inputBase64)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(inputBase64);
                using (var aes = new AesManaged())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.IV = iv;
                    aes.Key = digest;
                    aes.Padding = PaddingMode.PKCS7;
                    return Encoding.UTF8.GetString(aes.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
                }
            }
            catch
            {
                return "";
            }
        }

        public void Dispose()
        {
            try { encryptor.Dispose(); }
            catch { }
            try { decryptor.Dispose(); }
            catch { }
            try { aes.Dispose(); }
            catch { }
            encryptor = null;
            decryptor = null;
            aes = null;
        }
    }
}
