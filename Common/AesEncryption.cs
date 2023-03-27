using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Common
{
    public static class AesEncryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("a2i-red-bfd-akso");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("a2i-red-bfd-aksw");

        public static byte[] Encrypt(Packet.Packet packet)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            byte[] data = BitConverter.GetBytes((int)packet.Type).Concat(packet.Data).ToArray();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(data, 0, data.Length);
            csEncrypt.FlushFinalBlock();

            return msEncrypt.ToArray();
        }

        public static byte[] Decrypt(byte[] cipherText, int length)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            byte[] decryptedData = new byte[cipherText.Length];
            int bytesRead = csDecrypt.Read(decryptedData, 0, decryptedData.Length);

            return decryptedData.Take(bytesRead).ToArray();
        }
    }
}
