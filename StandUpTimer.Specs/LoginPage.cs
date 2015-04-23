using OpenQA.Selenium;
using TestStack.Seleno.PageObjects;

namespace StandUpTimer.Specs
{
    public class LoginPage : Page<LoginModel>
    {
        public HomePage Login(LoginModel loginModel)
        {
            Input.Model(loginModel);

            return Navigate.To<HomePage>(By.CssSelector("input[type=submit]"));
        }
    }
}