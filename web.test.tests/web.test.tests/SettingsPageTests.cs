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


        [Test]
        public void Date_Format_Applied()
        {
            //Arrange
            String date_format_Settings = "MM/DD/YYYY";
            String date_format_DateTime = "MM/dd/yyyy";
            int start_day = 1;
            int start_month = 1;
            int start_year = 2020;
            int term = 12;

            //Act
            SelectElement date_format_select = new SelectElement(driver.FindElement(By.XPath("//tr[1]/td/select")));
            date_format_select.SelectByText(date_format_Settings);
            driver.FindElement(By.Id("save")).Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            DateTime start_date = new DateTime(start_year, start_month, start_day);
            String exp_end_date = start_date.AddDays(term).ToString(date_format_DateTime);

            SelectElement year_select = new SelectElement(driver.FindElement(By.Id("year")));
            year_select.SelectByText(start_year.ToString());

            SelectElement month_select = new SelectElement(driver.FindElement(By.Id("month")));
            month_select.SelectByIndex(start_month - 1);

            SelectElement day_select = new SelectElement(driver.FindElement(By.Id("day")));
            day_select.SelectByText(start_day.ToString());

            IWebElement term_field = driver.FindElement(By.Id("term"));
            term_field.Clear();
            term_field.SendKeys(term.ToString());

            String act_end_date = driver.FindElement(By.Id("endDate")).GetAttribute("value");

            //Assert
            Assert.AreEqual(exp_end_date, act_end_date);
        }
    }
}
