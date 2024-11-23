using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinUIGallery.TabViewPages
{
    public sealed partial class MyTabContentControl : UserControl
    {
        public bool IsInProgress
        {
            get { return (bool)GetValue(IsInProgressProperty); }
            set { SetValue(IsInProgressProperty, value); }
        }

        public static readonly DependencyProperty IsInProgressProperty = DependencyProperty.Register("IsInProgress", typeof(bool), typeof(MyTabContentControl), new PropertyMetadata(false));

        public MyTabContentControl()
        {
            this.InitializeComponent();
        }
    }
}
