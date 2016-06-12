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

    using Windows.UI.Popups;

    class ToastMessenger
    {
        public ToastMessenger()
        {
        }

        public async void ShowInfo(string info)
        {
            var messageDialog = new MessageDialog(info);

            await messageDialog.ShowAsync();
        }

        public async void ShowError(string error)
        {
            var messageDialog = new MessageDialog(error);

            await messageDialog.ShowAsync();
        }
    }
}
