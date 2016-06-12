namespace Dono
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
