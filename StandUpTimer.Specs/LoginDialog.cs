using TestStack.White.InputDevices;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
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

        public void LogIn(string userName, string password)
        {
            loginWindow.Get<TextBox>("UserNameTextBox").Text = userName;

            loginWindow.Get(SearchCriteria.ByAutomationId("PasswordBox")).Focus();
            Keyboard.Instance.Enter(password);

            loginWindow.Get<Button>("LoginButton").Click();
        }
    }
}