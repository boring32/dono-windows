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
    using System.IO;
    using System.Threading.Tasks;

    using Windows.Storage;

    public class PersistableKey
    {
        private static string KEY_FILE = ".key";

        private static string Key = "";

        public async Task<string> GetKey()
        {
            if (!string.IsNullOrEmpty(PersistableKey.Key))
            {
                return PersistableKey.Key;
            }

            await this.LoadKey();

            return PersistableKey.Key;
        }

        public async Task SetKey(string key)
        {
            if (key != PersistableKey.Key)
            {
                PersistableKey.Key = key;
                await this.SaveKey();
            }
        }

        #region Helper Methods

        public async Task SaveKey()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;

                StorageFile keyFile = await folder.CreateFileAsync(PersistableKey.KEY_FILE, CreationCollisionOption.OpenIfExists);

                await FileIO.WriteTextAsync(keyFile, PersistableKey.Key.Encrypt());
            }
            catch (Exception)
            {
            }

        }

        public async Task LoadKey()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;

                StorageFile keyFile = await folder.CreateFileAsync(PersistableKey.KEY_FILE, CreationCollisionOption.OpenIfExists);

                var encryptedKey = await FileIO.ReadTextAsync(keyFile);
                PersistableKey.Key = encryptedKey.Decrypt();
            }
            catch(Exception)
            {
            }
        }

        #endregion Helper Methods
    }
}
