using System.Security;

namespace StandUpTimer.ViewModels
{
    internal class LoginViewModel
    {
        public string Title { get { return "Login"; } }
        public string Username { get; set; }
        public SecureString Password { get; set; }
    }
}