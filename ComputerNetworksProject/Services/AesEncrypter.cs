using System.Security.Cryptography;
using System.Text;

namespace ComputerNetworksProject.Services
{
    public class AesEncrypter
    {
        private readonly IConfiguration _config;
        private readonly byte[] key;
        private readonly byte[] iv;
        public AesEncrypter(IConfiguration config)
        {
            _config = config;
            if (config["AesKey"] is null || config["AesIv"] is null) {
                throw new ArgumentException("no key or iv");
            }
            key= Encoding.UTF8.GetBytes(config["AesKey"]);
            iv=Encoding.UTF8.GetBytes(config["AesIv"]);

        }

        public byte[] Encrypt(string plainText)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return msEncrypt.ToArray();
                }
            }
        }
        public string Decrypt(byte[] encryptText)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
