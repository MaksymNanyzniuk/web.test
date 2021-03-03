using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace web.test.tests
{
    class SettingsPageTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            driver.Url = "http://localhost:64177/Login";

            driver.FindElement(By.Id("login")).SendKeys("test");
            driver.FindElement(By.Id("password")).SendKeys("newyork1");
            driver.FindElement(By.Id("loginBtn")).Click();

            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            driver.FindElement(By.XPath("//div[text()='Settings']")).Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//div[text()='Logout']")));
        }

        [TearDown]
        public void AfterEachTest()
        {
            driver.Quit();
        }


        [Test]
        public void CancelBtn_Closes_Settings()
        {
            //Act
            driver.FindElement(By.Id("cancel")).Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            //Assert
            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
        }


        [Test]
        public void SaveBtn_Closes_Settings()
        {
            //Act
            driver.FindElement(By.Id("save")).Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            //Assert
            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
        }


        [Test]
        public void Logout_Exits()
        {
            //Act
            driver.FindElement(By.XPath("//div[text()='Logout']")).Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("remindBtn")));

            //Assert
            Assert.IsTrue(driver.FindElement(By.Id("remindBtn")).Displayed);
        }
    }
}
