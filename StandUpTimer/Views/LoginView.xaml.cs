using System.Diagnostics;
using System.Windows.Navigation;
using StandUpTimer.Properties;

namespace StandUpTimer.Views
{
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Settings.Default.BaseUrl + e.Uri.OriginalString));
            e.Handled = true;
        }
    }
}