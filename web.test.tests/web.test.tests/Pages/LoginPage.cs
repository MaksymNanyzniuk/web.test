using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace web.test.tests.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public IWebElement LoginField
        {
            get
            {
                return driver.FindElement(By.Id("login"));
            }
        }

        public IWebElement PasswordField => driver.FindElement(By.Id("password"));
        public IWebElement LoginButton => driver.FindElement(By.Id("loginBtn"));
        public IWebElement RemindButton
        {
            get
            {
                try
                {
                    new WebDriverWait(driver, TimeSpan.FromMilliseconds(2000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("remindBtn")));
                }
                catch { }
                return driver.FindElement(By.Id("remindBtn"));
            }
        }

        public void Login(string login, string password)
        {
            LoginField.SendKeys(login);
            PasswordField.SendKeys(password);
            LoginButton.Click();
        }

        internal DepositPage Login()
        {
            Login("test", "newyork1");
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(DepositPage.amount_field_id)));
            return new DepositPage(driver);
        }

        public string ErrorMessage => driver.FindElement(By.Id("errorMessage")).Text;
    }
}
