using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace web.test.tests
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


        [Test]
        public void Empty_Login_and_Empty_Password()
        {
            driver.FindElement(By.Id("loginBtn")).Click();
            
            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("User name and password cannot be empty!", error);
        }

        [Test]
        public void Empty_Login_and_Wrong_Password()
        {
            driver.FindElement(By.Id("password")).SendKeys("wrong");
            
            driver.FindElement(By.Id("loginBtn")).Click();
           
            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("User name and password cannot be empty!", error);
        }

        [Test]
        public void Empty_Login_and_Correct_Password()
        {
            driver.FindElement(By.Id("password")).SendKeys("newyork1");
            
            driver.FindElement(By.Id("loginBtn")).Click();
           
            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("User name and password cannot be empty!", error);
        }

        [Test]
        public void Wrong_Login_and_Empty_Password()
        {
            driver.FindElement(By.Id("login")).SendKeys("wrong");
           
            driver.FindElement(By.Id("loginBtn")).Click();

            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("User name and password cannot be empty!", error);
        }

        [Test]
        public void Wrong_Login_and_Wrong_Password()
        {
            string wrongUserName = "wrong";
            driver.FindElement(By.Id("login")).SendKeys(wrongUserName);
            driver.FindElement(By.Id("password")).SendKeys("wrong");
           
            driver.FindElement(By.Id("loginBtn")).Click();

            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("Incorrect user name!", error);
        }

        [Test]
        public void Wrong_Login_and_Correct_Password()
        {
            driver.FindElement(By.Id("login")).SendKeys("wrong");
            driver.FindElement(By.Id("password")).SendKeys("newyork1");

            driver.FindElement(By.Id("loginBtn")).Click();

            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("Incorrect user name!", error);
        }

        [Test]
        public void Correct_Login_and_Empty_Password()
        {
            driver.FindElement(By.Id("login")).SendKeys("test");
            driver.FindElement(By.Id("loginBtn")).Click();
           
            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("User name and password cannot be empty!", error);
        }

        [Test]
        public void Correct_Login_and_Wrong_Password()
        {
            driver.FindElement(By.Id("login")).SendKeys("test");
            driver.FindElement(By.Id("password")).SendKeys("wrong");
            driver.FindElement(By.Id("loginBtn")).Click();

            string error = driver.FindElement(By.Id("errorMessage")).Text;
            Assert.AreEqual("Incorrect password!", error);
        }

        [Test]
        public void Correct_Login_and_Correct_Password()
        {
            driver.FindElement(By.Id("login")).SendKeys("test");
            driver.FindElement(By.Id("password")).SendKeys("newyork1");
            driver.FindElement(By.Id("loginBtn")).Click();
           
            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
        }

        [Test]
        public void Button_Remind_Is_Present()
        {

            try
            {
                new WebDriverWait(driver, TimeSpan.FromMilliseconds(2000))
                .Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("remindBtn")));
            }
            catch { }

            Assert.IsTrue(driver.FindElement(By.Id("remindBtn")).Displayed);

        }
    }
}