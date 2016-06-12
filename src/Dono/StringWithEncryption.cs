﻿namespace Dono
{
    using System;
    using System.Text;

    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
    using Windows.Storage.Streams;
    using Windows.System.Profile;

    internal static class StringWithAES
    {
        public static string Encrypt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }

            var key = StringWithAES.GetHardwareId().Remove(32);

            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            var hash = new byte[32];
            Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(key)));
            byte[] temp;
            CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

            Array.Copy(temp, 0, hash, 0, 16);
            Array.Copy(temp, 0, hash, 15, 16);

            AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

            IBuffer Buffer = CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(str));
            return CryptographicBuffer.EncodeToBase64String(CryptographicEngine.Encrypt(AES, Buffer, null));
        }

        public static string Decrypt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }

            var key = StringWithAES.GetHardwareId().Remove(32);

            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            var hash = new byte[32];
            Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(key)));
            byte[] temp;
            CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

            Array.Copy(temp, 0, hash, 0, 16);
            Array.Copy(temp, 0, hash, 15, 16);

            AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

            IBuffer Buffer = CryptographicBuffer.DecodeFromBase64String(str);
            byte[] Decrypted;
            CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(AES, Buffer, null), out Decrypted);
            return System.Text.Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);
        }

        private static string GetHardwareId()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer hardwareId = token.Id;
            DataReader dataReader = DataReader.FromBuffer(hardwareId);

            var bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes);
        }
    }
}
