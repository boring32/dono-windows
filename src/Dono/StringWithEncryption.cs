// Dono Windows UWP - Password Derivation Tool
// Copyright (C) 2016  Panos Sakkos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
