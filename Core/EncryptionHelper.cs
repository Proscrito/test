using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace yamvc.Core
{
    //it can be static, but if we want to avoid statics, we can use singletons...
    public class EncryptionHelper
    {
        public string Salt { get; } = "qwerty09";
        public int AESSize { get; } = 256;

        public string PublicKey { get; }
        public string PrivateKey { get; }

        private static readonly Lazy<EncryptionHelper> lazy =
            new Lazy<EncryptionHelper>(() => new EncryptionHelper());

        public static EncryptionHelper Instance => lazy.Value;

        private EncryptionHelper()
        {
            //let's init RSA keys with something
            var csp = new RSACryptoServiceProvider(2048);
            //generate keys

            var privKey = csp.ExportParameters(true);
            var pubKey = csp.ExportParameters(false);

            PublicKey = PackKey(pubKey);
            PrivateKey = PackKey(privKey);
        }

        private string PackKey(RSAParameters pubKey)
        {
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(RSAParameters));
                serializer.Serialize(sw, pubKey);
                return sw.ToString();
            }
        }

        private RSAParameters UnpackKey(string key)
        {
            using (var sw = new StringReader(key))
            {
                var serializer = new XmlSerializer(typeof(RSAParameters));
                return (RSAParameters)serializer.Deserialize(sw);
            }
        }

        public string GenerateRandomKey(int length = -1)
        {
            if (length == -1)
                length = AESSize;

            const string mask = "abcdefghijklmnopqrstuvwxyz0123456789";
            var rnd = new Random((int)DateTime.Now.Ticks);

            var str = Enumerable.Range(0, length).Select(c => mask[rnd.Next(0, mask.Length)]);
            return new string(str.ToArray());
        }

        public byte[] EncryptAES(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;
            var saltBytes = Encoding.UTF8.GetBytes(Salt);

            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public byte[] DecryptAES(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes;
            var saltBytes = Encoding.UTF8.GetBytes(Salt);


            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public byte[] EncryptRSA(string data)
        {
            var csp = new RSACryptoServiceProvider();
            var pubKey = UnpackKey(PublicKey);
            csp.ImportParameters(pubKey);

            var keyBytes = Encoding.UTF8.GetBytes(data);
            return csp.Encrypt(keyBytes, false);
        }

        public string DecryptRSA(byte[] encryptedData)
        {
            var csp = new RSACryptoServiceProvider();
            var privKey = UnpackKey(PrivateKey);
            csp.ImportParameters(privKey);

            var decryptedData = csp.Decrypt(encryptedData, false);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}
