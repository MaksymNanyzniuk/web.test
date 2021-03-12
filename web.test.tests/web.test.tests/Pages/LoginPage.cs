using OpenQA.Selenium;

namespace web.test.tests.Pages
{
    public class LoginPage
    {
        private IWebDriver _driver;

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement LoginField
        {
            get
            {
                return _driver.FindElement(By.Id("loginBtn"));
            }
        }

        public IWebElement PasswordField => _driver.FindElement(By.Id("password"));
        public IWebElement LoginButton => _driver.FindElement(By.Id("loginBtn"));

        public void Login(string login, string password)
        {
            LoginField.SendKeys(login);
            PasswordField.SendKeys(password);
            LoginButton.Click();
        }

        public string ErrorMessage => _driver.FindElement(By.Id("errorMessage")).Text;

        //driver.FindElement(By.Id("password")).SendKeys("newyork1");

        //driver.FindElement(By.Id("loginBtn")).Click();

    }
}
