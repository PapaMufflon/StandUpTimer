using System;
using System.Security;
using System.Windows.Controls;
using System.Windows.Input;
using StandUpTimer.Common;

namespace StandUpTimer.ViewModels
{
    internal class LoginViewModel : IDialogViewModel
    {
        public event EventHandler<RequestCloseEventArgs> RequestClose;

        public string Title { get { return "Login"; } }
        public string Username { get; set; }
        public SecureString Password { get; set; }

        private ICommand loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                return loginCommand ?? (loginCommand = new RelayCommand(passwordBox =>
                {
                    Password = ((PasswordBox)passwordBox).SecurePassword;

                    OnRequestClose(new RequestCloseEventArgs { DialogResult = true });
                }));
            }
        }

        protected virtual void OnRequestClose(RequestCloseEventArgs e)
        {
            var handler = RequestClose;
            if (handler != null) handler(this, e);
        }
    }
}