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
    using System.Threading.Tasks;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PersistableKey persistableKey = new PersistableKey();
        private PersistableLabels persistableLabels = new PersistableLabels();

        public MainPage()
        {
            this.InitializeComponent();

            if (!this.MouseIsPresent())
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
        }

        #region Events

        #region Humburger Menu Events

        private void LabelsButton_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowPanel(MyLabelsGrid);
            MyLabelsGridFadein.Begin();
            this.LoadLabels();
        }

        private void AddLabelButton_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowPanel(AddLabelGrid);
            AddLabelGridFadein.Begin();
            newLabelTextBox.Focus(FocusState.Keyboard);
        }

        private async void KeyButton_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowPanel(KeyGrid);
            KeyGridFadein.Begin();
            passwordBox.Focus(FocusState.Keyboard);
            passwordBox.Password = await this.persistableKey.GetKey();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationPane.IsPaneOpen = !NavigationPane.IsPaneOpen;
//            ResizeOptions();
        }

        #endregion humburger Menu Events

        #region Loaded Events

        private void MyLabelsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadLabels();
        }

        private async void KeyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = await this.persistableKey.GetKey();
        }

        #endregion Loaded Events

        #region LostFocus Events

        private async void newLabelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            await this.TryAddLabel();
        }

        public async void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            await this.TrySetKey();
        }

        #endregion LostFocus Events

        #region OnKeyDown

        private async void newLabelTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await this.TryAddLabel();
            }
        }

        private async void yourKeyTextBlock_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await this.TrySetKey();
            }
        }

        #endregion OnKeyDown

        #region Tapped Events

        private void deleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var labelToDelete = (sender as Button).Tag as string;

            this.persistableLabels.Delete(labelToDelete);

            this.LoadLabels();
        }

        #endregion Tapped Events

        #endregion Events

        #region Helper Methods

        private void ClosePanel()
        {
            NavigationPane.IsPaneOpen = false;
        }

        private void ShowPanel(Panel panel)
        {
            this.ClosePanel();

            MyLabelsGrid.Visibility = Visibility.Collapsed;
            AddLabelGrid.Visibility = Visibility.Collapsed;
            KeyGrid.Visibility = Visibility.Collapsed;

            panel.Visibility = Visibility.Visible;
        }

        private async void LoadLabels()
        {
            MyLabelsListView.Items.Clear();

            var labels = await this.persistableLabels.GetAll();

            foreach (var label in labels)
            {
                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.VerticalAlignment = VerticalAlignment.Center;

                var labelTextBlock = new TextBlock();
                labelTextBlock.Text = label;

                if (MouseIsPresent())
                {
                    labelTextBlock.DoubleTapped += LabelTextBlock_DoubleTapped;
                    labelTextBlock.RightTapped += LabelTextBlock_RightTapped;
                }
                else
                {
                    labelTextBlock.Tapped += LabelTextBlock_Tapped;
                    labelTextBlock.Holding += LabelTextBlock_Holding;
                }

                labelTextBlock.Style = Application.Current.Resources["TitleTextBlockStyle"] as Style;
                labelTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                var color = new Color();
                color.R = 3;
                color.G = 169;
                color.B = 244;
                color.A = 255;
                labelTextBlock.Foreground = new SolidColorBrush(color);
                labelTextBlock.Margin = new Thickness(10, 0, 0, 0);

                var image = new Image();
                image.Source = new BitmapImage(new Uri("ms-appx:///Assets/tag.png", UriKind.Absolute));

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(labelTextBlock);

                MyLabelsListView.Items.Add(stackPanel);
            }

            if (MyLabelsListView.Items.Count > 0)
            {
                MyLabelsListView.Visibility = Visibility.Visible;
                LonelyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                MyLabelsListView.Visibility = Visibility.Collapsed;
                LonelyTextBlock.Visibility = Visibility.Visible;

                LonelyFadein.Begin();
            }
        }

        private void LabelTextBlock_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ShowDelete(sender);
        }

        private async void LabelTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await TryFetchPassword(sender);
        }

        private async void LabelTextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await TryFetchPassword(sender);
        }

        private async Task TryFetchPassword(object sender)
        {
            var labelTextBlock = sender as TextBlock;

            var label = labelTextBlock.Text;
            var key = await this.persistableKey.GetKey();

            if (string.IsNullOrWhiteSpace(key))
            {
                new ToastMessenger().ShowError("Set your Key and then you will be able to retrieve your passwords");

                passwordBox.Password = await this.persistableKey.GetKey();

                return;
            }

            var password = Dono.ComputePassword(key, label);

            this.CopyToClipboard(password);

            new ToastMessenger().ShowInfo("Your password for " + label + " is ready to be pasted");
        }

        private void LabelTextBlock_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowDelete(sender);
        }

        private void ShowDelete(object sender)
        {
            var flyout = new Flyout();
            var deleteButton = new Button();
            deleteButton.Content = "Delete";
            deleteButton.Tag = (sender as TextBlock).Text;
            deleteButton.Tapped += deleteButton_Tapped;
            flyout.Content = deleteButton;
            flyout.Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom;
            flyout.ShowAt(sender as TextBlock);
        }

        private void CopyToClipboard(string password)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(password);

            Clipboard.SetContent(dataPackage);
        }

        private bool MouseIsPresent()
        {
            return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse;
        }

        private async Task TryAddLabel()
        {
            var label = newLabelTextBox.Text;

            if (await this.persistableLabels.Add(label))
            {
                new ToastMessenger().ShowInfo("Label " + label + " was added to your Labels!");
            }

            newLabelTextBox.Text = "";
        }

        private async Task TrySetKey()
        {
            var password = passwordBox.Password;

            if (password.Length < Dono.MIN_KEY_LENGTH)
            {
                new ToastMessenger().ShowError("Your Key has to be longer than 16 characters long");
                passwordBox.Password = await this.persistableKey.GetKey();
            }
            else
            {
                var previousKey = await this.persistableKey.GetKey();
                await this.persistableKey.SetKey(password);

                if (previousKey != password)
                {
                    new ToastMessenger().ShowInfo("Your Key was set!");
                }
            }
        }

        #region Humburger Menu

        private void ResizeOptions()
        {
            // calculate the actual width of the navigation pane

            var width = NavigationPane.CompactPaneLength;
            if (NavigationPane.IsPaneOpen)
            {
                width = NavigationPane.OpenPaneLength;
            }

            // change the width of all control in the navigation pane

            HamburgerButton.Width = width;

            foreach (var option in NavigationMenu.Children)
            {
                var radioButton = (option as RadioButton);
                if (radioButton != null)
                {
                    radioButton.Width = width;
                }
            }
        }

        private void Shell_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            FocusNavigationDirection direction = FocusNavigationDirection.None;

            switch (e.Key)
            {
                // both Space and Enter will trigger navigation

                case Windows.System.VirtualKey.Space:
                case Windows.System.VirtualKey.Enter:
                    {
                        var control = FocusManager.GetFocusedElement() as Control;
                        var option = control as RadioButton;
                        if (option != null)
                        {
                            var automation = new RadioButtonAutomationPeer(option);
                            automation.Select();
                        }
                    }
                    return;

                // otherwise, find next focusable element in the appropriate direction

                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.GamepadDPadLeft:
                case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
                case Windows.System.VirtualKey.NavigationLeft:
                    direction = FocusNavigationDirection.Left;
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.GamepadDPadRight:
                case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
                case Windows.System.VirtualKey.NavigationRight:
                    direction = FocusNavigationDirection.Right;
                    break;

                case Windows.System.VirtualKey.Up:
                case Windows.System.VirtualKey.GamepadDPadUp:
                case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
                case Windows.System.VirtualKey.NavigationUp:
                    direction = FocusNavigationDirection.Up;
                    break;

                case Windows.System.VirtualKey.Down:
                case Windows.System.VirtualKey.GamepadDPadDown:
                case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
                case Windows.System.VirtualKey.NavigationDown:
                    direction = FocusNavigationDirection.Down;
                    break;
            }

            if (direction != FocusNavigationDirection.None)
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                if (control != null)
                {
                    control.Focus(FocusState.Programmatic);
                    e.Handled = true;
                }
            }
        }

        #endregion Humburger Menu

        #endregion Helper Methods
    }
}
