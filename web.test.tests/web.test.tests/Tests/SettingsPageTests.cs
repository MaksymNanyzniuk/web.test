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

            new LoginPage(driver).Login().OpenSettingsPage();
        }

        [TearDown]
        public void AfterEachTest()
        {
            driver.Quit();
        }


        [Test]
        public void CancelBtn_Closes_Settings()
        {
            Assert.IsTrue(new SettingsPage(driver).Cancel().AmountField.Displayed);
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


        [TestCase("dd/MM/yyyy", 12)]
        [TestCase("dd-MM-yyyy", 12)]
        [TestCase("MM/dd/yyyy", 12)]
        [TestCase("MM dd yyyy", 12)]
        public void Date_Format_Applied(String date_format, int term)
        {
            //Arrange
            DateTime startDate = new DateTime(2020, 1, 1);

            //Act
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectDateFormat.SelectByText(date_format);
            settingsPage.Save();

            DepositPage depositPage = new DepositPage(driver);
            depositPage.StartDate = startDate;
            depositPage.SetTerm(term);

            //Assert
            Assert.AreEqual(startDate.AddDays(term).ToString(date_format, CultureInfo.InvariantCulture), depositPage.EndDate);
        }


        [TestCase("123,456,789.00", 365)]
        [TestCase("123.456.789,00", 365)]
        [TestCase("123 456 789.00", 365)]
        [TestCase("123 456 789,00", 365)]
        public void Number_Format_Applied(String number_format, int fin_year)
        {
            //Arrange
            Decimal amount = 12345m;
            int rate = 74;
            int term = 211;
            int year_length = 365;

            //Act
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectNumberFormat.SelectByText(number_format);
            settingsPage.Save();

            DepositPage depositPage = new DepositPage(driver);
            depositPage.SelectFinYear(fin_year);
            depositPage.SetAmountRateTerm(amount, rate, term);

            //Assert
            Assert.AreEqual(Math.Round(amount*rate*term/100/year_length, 2).ToString("N", settingsPage.GetSelectedNFI(number_format)), depositPage.InterestStr);
        }


        [TestCase("$ - US dollar")]
        [TestCase("€ - euro")]
        [TestCase("£ - Great Britain Pound")]
        public void Currency_Applied(String currency_format)
        {
            //Act
            SettingsPage settingsPage = new SettingsPage(driver);
            settingsPage.SelectDefaultCurrency.SelectByText(currency_format);
            settingsPage.Save();

            //Assert
            Assert.AreEqual(settingsPage.GetExpCurrencySign(currency_format), new DepositPage(driver).CurrencySign);
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
