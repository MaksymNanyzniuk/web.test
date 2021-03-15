using System;
using System.Globalization;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using web.test.tests.Pages;

namespace web.test.tests.Tests
{
    class SettingsPageTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            driver.Url = "http://localhost:64177/Login";

            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login("test", "newyork1");

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
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.CancelBtn.Click();

            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
        }


        [Test]
        public void SaveBtn_Closes_Settings()
        {
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.Save();

            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
        }


        [Test]
        public void Logout_Exits()
        {
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.Logout.Click();

            LoginPage loginPage = new LoginPage(driver);
            
            Assert.IsTrue(loginPage.RemindButton.Displayed);
        }


        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Date_Format_Applied(int date_format_index)
        {
            //Arrange
            int start_day = 1;
            int start_month = 1;
            int start_year = 2020;
            int term = 12;

            //Act
            DateTime start_date = new DateTime(start_year, start_month, start_day);

            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectDateFormat.SelectByIndex(date_format_index);
            String exp_end_date = start_date.AddDays(term).ToString(settingsPage.SelectedDateFormat, CultureInfo.InvariantCulture);

            settingsPage.Save();

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


        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Number_Format_Applied(int number_format_index)
        {
            //Arrange
            Decimal amount = 12345m;
            int rate = 74;
            int term = 211;

            String financial_year = "365 days";
            int year_length = 365;

            //Act
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectNumberFormat.SelectByIndex(number_format_index);
            String selected_number_format_setting = settingsPage.SelectedNumberFormat;

            settingsPage.Save();

            driver.FindElement(By.XPath($"//td[text()='{financial_year}']/input")).Click();

            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys(amount.ToString());

            driver.FindElement(By.Id("percent")).Clear();
            driver.FindElement(By.Id("percent")).SendKeys(rate.ToString());

            driver.FindElement(By.Id("term")).Clear();
            driver.FindElement(By.Id("term")).SendKeys(term.ToString());

            NumberFormatInfo nfi = new CultureInfo("en-US", true).NumberFormat;
            switch (selected_number_format_setting)
            {
                case "123,456,789.00":
                    break;
                case "123.456.789,00":
                    nfi.NumberGroupSeparator = ".";
                    nfi.NumberDecimalSeparator = ",";
                    break;
                case "123 456 789.00":
                    nfi.NumberGroupSeparator = " ";
                    break;
                case "123 456 789,00":
                    nfi.NumberGroupSeparator = " ";
                    nfi.NumberDecimalSeparator = ",";
                    break;
                default:
                    Assert.AreEqual("", "Unexpected number format selected in the dropdown.");
                    break;
            }

            String exp_interest = Math.Round(amount * rate * term / 100 / year_length, 2).ToString("N", nfi);

            String act_interest = driver.FindElement(By.Id("interest")).GetAttribute("value");

            //Assert
            Assert.AreEqual(exp_interest, act_interest);
        }


        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Currency_Applied(int default_currency_index)
        {
            //Act
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectDefaultCurrency.SelectByIndex(default_currency_index);
            String selected_currency_setting = settingsPage.SelectedCurrency;

            settingsPage.Save();

            String exp_currency_sign = "";
            switch (selected_currency_setting)
            {
                case "$ - US dollar":
                    exp_currency_sign = "$";
                    break;
                case "€ - euro":
                    exp_currency_sign = "€";
                    break;
                case "£ - Great Britain Pound":
                    exp_currency_sign = "£";
                    break;
                default:
                    Assert.AreEqual("", "Unexpected currency selected in the dropdown.");
                    break;
            }

            String act_currency_sign = driver.FindElement(By.Id("currency")).Text;

            //Assert
            Assert.AreEqual(exp_currency_sign, act_currency_sign);
        }


        [TestCase("Date format:")]
        [TestCase("Number format:")]
        [TestCase("Default currency:")]
        public void Field_Captions_th(String caption)
        {
            //Assert
            Assert.True(driver.FindElement(By.XPath($"//th[text() = '{caption}']")).Displayed);
        }
    }
}
