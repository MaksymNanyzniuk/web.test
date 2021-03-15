using System;
using System.Globalization;
using System.Threading;
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

            new LoginPage(driver).Login("test", "newyork1");

            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            new DepositPage(driver).Settings.Click();
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
            new SettingsPage(driver).CancelBtn.Click();

            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));

            Assert.IsTrue(new DepositPage(driver).AmountField.Displayed);
        }


        [Test]
        public void SaveBtn_Closes_Settings()
        {
            new SettingsPage(driver).Save();

            Assert.IsTrue(new DepositPage(driver).AmountField.Displayed);
        }


        [Test]
        public void Logout_Exits()
        {
            new SettingsPage(driver).Logout.Click();
            Assert.IsTrue(new LoginPage(driver).RemindButton.Displayed);
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
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectDateFormat.SelectByIndex(date_format_index);
            String exp_end_date = new DateTime(start_year, start_month, start_day).AddDays(term).ToString(settingsPage.SelectedDateFormat, CultureInfo.InvariantCulture);

            settingsPage.Save();

            DepositPage depositPage = new DepositPage(driver);
            depositPage.SelectStartYear.SelectByText(start_year.ToString());
            depositPage.SelectStartMonth.SelectByIndex(start_month - 1);
            depositPage.SelectStartDay.SelectByText(start_day.ToString());
            depositPage.TermField.Clear();
            depositPage.TermField.SendKeys(term.ToString());

            //Assert
            Assert.AreEqual(exp_end_date, depositPage.EndDate);
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

            //Act
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectNumberFormat.SelectByIndex(number_format_index);
            String selected_number_format_setting = settingsPage.SelectedNumberFormat;

            settingsPage.Save();

            DepositPage depositPage = new DepositPage(driver);
            depositPage.FinYear365.Click();
            int year_length = 365;
            depositPage.AmountField.Clear();
            depositPage.AmountField.SendKeys(amount.ToString());
            depositPage.RateField.Clear();
            depositPage.RateField.SendKeys(rate.ToString());
            depositPage.TermField.Clear();
            depositPage.TermField.SendKeys(term.ToString());

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

            //Assert
            Assert.AreEqual(exp_interest, depositPage.InterestStr);
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

            //Assert
            Assert.AreEqual(exp_currency_sign, new DepositPage(driver).CurrencySign);
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
