using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;

namespace StandUpTimer.Specs
{
    internal class LoginDialog
    {
        private readonly Window loginWindow;

        public LoginDialog(Window loginWindow)
        {
            this.loginWindow = loginWindow;
        }

        public Hyperlink RegisterLink
        {
            get { return loginWindow.Get<Hyperlink>("Register"); }
        }

        public void TakeScreenshot(string fileName)
        {
            loginWindow.TakeScreenshot(fileName);
        }
    }
}