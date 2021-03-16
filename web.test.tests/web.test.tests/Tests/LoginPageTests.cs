using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using web.test.tests.Pages;

namespace web.test.tests.Tests
{
    public class LoginPageTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            driver.Url = "http://localhost:64177/Login";
        }

        [TearDown]
        public void AfterEachTest()
        {
            driver.Quit();
        }


        [TestCase("", "")]
        [TestCase("", "wrong")]
        [TestCase("", "newyork1")]
        [TestCase("wrong", "")]
        [TestCase("test", "")]
        public void Empty_Login_OR_Password(String login, String password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(login, password);
            Assert.AreEqual("User name and password cannot be empty!", loginPage.ErrorMessage);
        }

        [TestCase("wrong", "wrong")]
        [TestCase("wrong", "newyork1")]
        [TestCase("test", "wrong")]
        public void Wrong_Login_OR_Password(String login, String password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(login, password);
            Assert.AreEqual("Incorrect user name or password!", loginPage.ErrorMessage);
        }
        
        [Test]
        public void Correct_Login_and_Correct_Password()
        {
            new LoginPage(driver).Login("test", "newyork1");
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(DepositPage.amount_field_id)));
            Assert.IsTrue(new DepositPage(driver).AmountField.Displayed);
        }

        [Test]
        public void Button_Remind_Is_Present()
        {
            Assert.IsTrue(new LoginPage(driver).RemindButton.Displayed);
        }

        [Test]
        public void Button_Labels()
        {
            LoginPage loginPage = new LoginPage(driver);
            Assert.AreEqual("Login", loginPage.LoginButton.Text);
            Assert.AreEqual("Remind password", loginPage.RemindButton.Text);
        }
    }
}