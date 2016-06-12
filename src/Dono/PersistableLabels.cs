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
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Windows.Storage;

    internal class PersistableLabels
    {
        private static string LABELS_FILE = ".labels";

        private static List<string> labels = new List<string>();

        internal async Task<bool> Add(string label)
        {
            label = label.ToLower().Trim();

            if (PersistableLabels.labels.Contains(label) || String.IsNullOrWhiteSpace(label))
            {
                return false;
            }

            PersistableLabels.labels.Add(label);

            PersistableLabels.labels.Sort();

            await this.SaveLabels();

            return true;
        }

        internal async Task<IEnumerable<string>> GetAll()
        {
            if (PersistableLabels.labels.Count > 0)
            {
                return PersistableLabels.labels;
            }

            await this.LoadLabels();

            PersistableLabels.labels.Sort();

            return PersistableLabels.labels;
        }

        internal string GetAt(int i)
        {
            return PersistableLabels.labels.ToArray()[i];
        }

        internal async void Delete(string label)
        {
            PersistableLabels.labels.Remove(label);

            await this.SaveLabels();
        }

        private async Task SaveLabels()
        {
            var folder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile labelsFile = await folder.CreateFileAsync(PersistableLabels.LABELS_FILE, CreationCollisionOption.OpenIfExists);

                var dump = "";

                foreach (var label in PersistableLabels.labels)
                {
                    dump += label + "\n";
                }

                await FileIO.WriteTextAsync(labelsFile, dump);
            }
            catch (Exception)
            {
            }
        }

        private async Task LoadLabels()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;

                StorageFile labelsFile = await folder.CreateFileAsync(PersistableLabels.LABELS_FILE, CreationCollisionOption.OpenIfExists);

                var dump = await FileIO.ReadTextAsync(labelsFile);

                var labels = dump.Trim().Split(new char[] { '\n' });

                foreach (var label in labels)
                {
                    if (!string.IsNullOrWhiteSpace(label))
                    {
                        PersistableLabels.labels.Add(label);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
