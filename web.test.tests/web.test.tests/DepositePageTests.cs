using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace web.test.tests
{
    class DepositePageTests
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
        }

        [TearDown]
        public void AfterEachTest()
        {
            driver.Quit();
        }


        [Test]
        public void Income_And_Interest()
        {
            //Arrange
            Decimal amount = 1234m; // range [0; 100,000]
            Decimal rate = 13.17m; // range [0; 100] 
            int term = 364; // range [0; 360/365]
           
            Decimal exp_interest = Math.Round (amount * rate * term / 100 / 365, 2);
            Decimal exp_income = amount + exp_interest;

            //Act
            driver.FindElement(By.Id("d365")).Click(); // available IDs: "d360", "d365"
            driver.FindElement(By.Id("amount")).SendKeys("" + amount);
            driver.FindElement(By.Id("percent")).SendKeys("" + rate);
            driver.FindElement(By.Id("term")).SendKeys("" + term);

            Decimal act_interest = Convert.ToDecimal(driver.FindElement(By.Id("interest")).GetAttribute("value"));
            Decimal act_income = Convert.ToDecimal(driver.FindElement(By.Id("income")).GetAttribute("value"));

            //Assert
            Assert.AreEqual(exp_interest, act_interest);
            Assert.AreEqual(exp_income, act_income);
        }


        [Test]
        public void End_Date()
        {
            //Arrange
            int start_day = 1;
            int start_month = 1;
            int start_year = 2020;
            int term = 364; // range [0; 360/365]

            DateTime start_date = new DateTime(start_year, start_month, start_day);

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
            String exp_end_date = start_date.AddDays(term).ToString().Split(new char[] { ' ' })[0];


            //Act
            driver.FindElement(By.Id("d365")).Click(); // available IDs: "d360", "d365"

            driver.FindElement(By.Id("term")).SendKeys("" + term);
            
            driver.FindElement(By.XPath("//*[@id='day']/option[" + start_day + "]")).Click();
            driver.FindElement(By.XPath("//*[@id='month']/option[" + start_month + "]")).Click();
            driver.FindElement(By.XPath("//*[@id='year']/option[@value='" + start_year + "']")).Click();

            String act_end_date = driver.FindElement(By.Id("endDate")).GetAttribute("value");


            //Assert
            Assert.AreEqual(exp_end_date, act_end_date);
        }


        [Test]
        public void Financial_Year_IsClicked()
        {
            //Arrange
            String radio_button_id = "d365"; // available IDs: "d360", "d365"


            //Act
            driver.FindElement(By.Id(radio_button_id)).Click();


            //Assert
            Assert.AreEqual("true", driver.FindElement(By.Id(radio_button_id)).GetAttribute("checked"));
        }


        [Test]
        public void Selected_Month_Name()
        {
            //Arrange
            string[] month_name = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

            for (int i = 1; i <= 12; i++)
            {
                //Act
                driver.FindElement(By.XPath("//*[@id='month']/option[" + i + "]")).Click();

                //Assert
                Assert.AreEqual(month_name[i - 1], driver.FindElement(By.Id("month")).GetAttribute("value"));
            }
        }
    }
}
