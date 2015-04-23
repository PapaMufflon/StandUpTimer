using OpenQA.Selenium;
using TestStack.Seleno.PageObjects;

namespace StandUpTimer.Specs
{
    public class HomePage : Page
    {
        public RegisterPage GoToRegisterPage()
        {
            return Navigate.To<RegisterPage>(By.Id("registerLink"));
        }

        public LoginPage GoToLoginPage()
        {
            return Navigate.To<LoginPage>(By.Id("loginLink"));
        }

        public Page GoToStatisticsPage()
        {
            return Navigate.To<Page>(By.LinkText("Statistics"));
        }
    }
}