using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUIGallery.Helper;

namespace WinUIGallery.SamplePages
{
    public sealed partial class SampleBuiltInSystemBackdropsWindow : Window
    {
        BackdropType currentBackdrop;
        public BackdropType[] AllowedBackdrops;

        public SampleBuiltInSystemBackdropsWindow()
        {
            InitializeComponent();
            AppWindow.SetIcon(@"Assets\Tiles\GalleryIcon.ico");
            ((FrameworkElement)Content).RequestedTheme = ThemeHelper.RootTheme;
            this.SetTitleBarTheme();
            SetBackdrop(BackdropType.Mica);
            ThemeComboBox.SelectedIndex = 0;
        }

        public enum BackdropType
        {
            None,
            Mica,
            MicaAlt,
            Acrylic
        }

        public void SetBackdrop(BackdropType type)
        {
            // Reset to default color. If the requested type is supported, we'll update to that.
            // Note: This sample completely removes any previous controller to reset to the default
            //       state. This is done so this sample can show what is expected to be the most
            //       common pattern of an app simply choosing one controller type which it sets at
            //       startup. If an app wants to toggle between Mica and Acrylic it could simply
            //       call RemoveSystemBackdropTarget() on the old controller and then setup the new
            //       controller, reusing any existing m_configurationSource and Activated/Closed
            //       event handlers.

            currentBackdrop = BackdropType.None;
            tbCurrentBackdrop.Text = "None";
            tbChangeStatus.Text = "";
            SystemBackdrop = null;

            if (type == BackdropType.Mica)
            {
                if (TrySetMicaBackdrop(false))
                {
                    tbCurrentBackdrop.Text = "Built-in Mica";
                    currentBackdrop = type;
                }
                else
                {
                    // Mica isn't supported. Try Acrylic.
                    type = BackdropType.Acrylic;
                    tbChangeStatus.Text += "  Mica isn't supported. Trying Acrylic.";
                }
            }
            if (type == BackdropType.MicaAlt)
            {
                if (TrySetMicaBackdrop(true))
                {
                    tbCurrentBackdrop.Text = "Built-in MicaAlt";
                    currentBackdrop = type;
                }
                else
                {
                    // MicaAlt isn't supported. Try Acrylic.
                    type = BackdropType.Acrylic;
                    tbChangeStatus.Text += "  MicaAlt isn't supported. Trying Acrylic.";
                }
            }
            if (type == BackdropType.Acrylic)
            {
                if (TrySetAcrylicBackdrop())
                {
                    tbCurrentBackdrop.Text = "Built-in Acrylic";
                    currentBackdrop = type;
                }
                else
                {
                    // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                    tbChangeStatus.Text += "  Acrylic isn't supported. Switching to default color.";
                }
            }
            if (type == BackdropType.None && ThemeComboBox.SelectedIndex != 0)
            {
                ((ScrollViewer)Content).Background = new SolidColorBrush(ThemeComboBox.SelectedIndex == 1 ? Colors.White : Colors.Black);
            }
            else
            {
                ((ScrollViewer)Content).Background = new SolidColorBrush(Colors.Transparent);
            }

            this.SetTitleBarBackdrop(SystemBackdrop);

            // Announce visual change to automation.
            UIHelper.AnnounceActionForAccessibility(btnChangeBackdrop, $"Background changed to {tbCurrentBackdrop.Text}", "BackgroundChangedNotificationActivityId");
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (MicaController.IsSupported())
            {
                SystemBackdrop = new MicaBackdrop { Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base }; ;
                return true;
            }

            return false; // Mica is not supported on this system
        }

        bool TrySetAcrylicBackdrop()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                SystemBackdrop = new DesktopAcrylicBackdrop();
                return true;
            }

            return false; // Acrylic is not supported on this system
        }

        void ChangeBackdropButton_Click(object sender, RoutedEventArgs e)
        {
            var newType = currentBackdrop switch
            {
                BackdropType.Mica => BackdropType.MicaAlt,
                BackdropType.MicaAlt => BackdropType.Acrylic,
                BackdropType.Acrylic => BackdropType.None,
                _ => BackdropType.Mica,
            };

            if(!AllowedBackdrops.Any(b => b == newType))
            {
                currentBackdrop = newType;
                ChangeBackdropButton_Click(sender, e);
                return;
            }

            SetBackdrop(newType);
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((FrameworkElement)Content).RequestedTheme = ThemeComboBox.SelectedIndex switch
            {
                1 => ElementTheme.Light,
                2 => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
            this.SetTitleBarTheme();

            if (currentBackdrop == BackdropType.None && ThemeComboBox.SelectedIndex != 0)
            {
                ((ScrollViewer)Content).Background = new SolidColorBrush(ThemeComboBox.SelectedIndex == 1 ? Colors.White : Colors.Black);
            }
            else
            {
                ((ScrollViewer)Content).Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void CustomTitleBarSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ExtendsContentIntoTitleBar = CustomTitleBarSwitch.IsOn;
            if (!ExtendsContentIntoTitleBar)
            {
                AppWindow.TitleBar.ResetToDefault();
                this.SetTitleBarBackdrop(SystemBackdrop);
                this.SetTitleBarTheme();
            }
            else
                AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        }
    }
}
