using OpenQA.Selenium;
using TestStack.Seleno.PageObjects;

namespace StandUpTimer.Specs.PageObjects
{
    public class RegisterPage : Page<RegisterModel>
    {
        public HomePage RegisterUser(RegisterModel registerModel)
        {
            Input.Model(registerModel);

            return Navigate.To<HomePage>(By.CssSelector("input[type=submit]"));
        }
    }
}