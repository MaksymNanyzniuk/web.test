using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

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
                return _driver.FindElement(By.Id("login"));
            }
        }

        public IWebElement PasswordField => _driver.FindElement(By.Id("password"));
        public IWebElement LoginButton => _driver.FindElement(By.Id("loginBtn"));
        public IWebElement RemindButton
        {
            get
            {
                try
                {
                    new WebDriverWait(_driver, TimeSpan.FromMilliseconds(2000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("remindBtn")));
                }
                catch { }
                return _driver.FindElement(By.Id("remindBtn"));
            }
        }

        public void Login(string login, string password)
        {
            LoginField.SendKeys(login);
            PasswordField.SendKeys(password);
            LoginButton.Click();
        }
        public string ErrorMessage => _driver.FindElement(By.Id("errorMessage")).Text;
    }
}
