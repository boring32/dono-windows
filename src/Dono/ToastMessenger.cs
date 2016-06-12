namespace Dono
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
