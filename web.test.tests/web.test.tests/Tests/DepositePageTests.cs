using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using web.test.tests.Pages;

namespace web.test.tests.Tests
{
    class DepositePageTests
    {
        private IWebDriver driver;
        const String fin_year_360 = "360 days";
        const String fin_year_365 = "365 days";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Url = "http://localhost:64177/Login";

            new LoginPage(driver).Login("test", "newyork1");

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
            Decimal[] amount_array = { 0m, 1m, 100.54m, 100000m, 100001m };
            Decimal[] rate_array = { 0m, 1m, 13.5m, 100m, 101m };
            Decimal[] term_array = { 0m, 1m, 7.7m, 0m, 0m };

            var year_map = new Dictionary<String, int>();
            year_map.Add(fin_year_360, 360);
            year_map.Add(fin_year_365, 365);

            //Act
          
            int n = 0;

            String financial_year_Id;
            int year_length;

            Decimal exp_interest;
            Decimal exp_income;

            Decimal act_interest;
            Decimal act_income;

            int array_size = amount_array.Length * rate_array.Length * term_array.Length * year_map.Count;

            Decimal[] exp_interest_array = new Decimal[array_size];
            Decimal[] exp_income_array = new Decimal[array_size];

            Decimal[] act_interest_array = new Decimal[array_size];
            Decimal[] act_income_array = new Decimal[array_size];

            foreach (var pair in year_map)
            {
                financial_year_Id = pair.Key;
                year_length = pair.Value;

                term_array[term_array.Length - 2] = year_length;
                term_array[term_array.Length - 1] = year_length + 1;

                //Act
                driver.FindElement(By.XPath($"//*[text()='{financial_year_Id}']/input")).Click();

                for (int i = 0; i < amount_array.Length; i++)
                {
                    driver.FindElement(By.Id("amount")).Clear();
                    driver.FindElement(By.Id("amount")).SendKeys(amount_array[i].ToString(CultureInfo.InvariantCulture));
                    for (int j = 0; j < rate_array.Length; j++)
                    {
                        driver.FindElement(By.Id("percent")).Clear();
                        driver.FindElement(By.Id("percent")).SendKeys(rate_array[j].ToString(CultureInfo.InvariantCulture));
                        for (int k = 0; k < term_array.Length; k++)
                        {
                            driver.FindElement(By.Id("term")).Clear();
                            driver.FindElement(By.Id("term")).SendKeys(term_array[k].ToString(CultureInfo.InvariantCulture));

                            if (amount_array[i] > 100000)
                            {
                                exp_interest = 0;
                                exp_income = 0;
                            }
                            else if (rate_array[j] > 100 | term_array[k] > year_length)
                            {
                                exp_interest = 0;
                                exp_income = amount_array[i] + exp_interest;
                            }
                            else
                            {
                                exp_interest = Math.Round(amount_array[i] * rate_array[j] * term_array[k] / 100 / year_length, 2);
                                exp_income = amount_array[i] + exp_interest;
                            }

                            exp_interest_array[n] = exp_interest;
                            exp_income_array[n] = exp_income;

                            act_interest = Convert.ToDecimal(driver.FindElement(By.Id("interest")).GetAttribute("value"), CultureInfo.InvariantCulture);
                            act_income = Convert.ToDecimal(driver.FindElement(By.Id("income")).GetAttribute("value"), CultureInfo.InvariantCulture);

                            act_interest_array[n] = act_interest;
                            act_income_array[n] = act_income;

                            Console.WriteLine("n=" + n + ". amount=" + amount_array[i] + "; rate=" + rate_array[j] + "; term=" + term_array[k] + ";");
                            Console.WriteLine("exp_interest=" + exp_interest + "; act_interest=" + act_interest + "; exp_income= " + exp_income + "; act_income= " + act_income);

                            n++;

                            //Assert
                            Assert.AreEqual("true", driver.FindElement(By.XPath($"//*[text()='{financial_year_Id}']/input")).GetAttribute("checked"));
                        }
                    }
                }
            }

            //Assert
            Assert.AreEqual(exp_interest_array, act_interest_array);
            Assert.AreEqual(exp_income_array, act_income_array);
        }


        [TestCase(0, 360)]
        [TestCase(1, 360)]
        [TestCase(16, 360)]
        [TestCase(17, 360)]
        [TestCase(169, 360)]
        [TestCase(170, 360)]
        [TestCase(229, 360)]
        [TestCase(360, 360)]
        [TestCase(365, 365)]
        public void End_Date(int term, int fin_year)
        {
            //Arrange
            int start_day = 15;
            int start_month = 7;
            int start_year = 2019;

            //Act
            DepositPage depositPage = new DepositPage(driver);
            depositPage.OpenSettingsPage();

            SettingsPage settingsPage = new SettingsPage(driver);
            String exp_end_date = new DateTime(start_year, start_month, start_day).AddDays(term).ToString(settingsPage.SelectedDateFormat, CultureInfo.InvariantCulture);
            settingsPage.CancelBtn.Click();

            if (fin_year == 360)
            { 
                depositPage.FinYear360.Click(); 
            }
            else if (fin_year == 365)
            {
                depositPage.FinYear365.Click();
            }
            else 
            {
                Assert.AreEqual("", "Inproper fin_year value in input data");
            }

            depositPage.SelectStartYear.SelectByText(start_year.ToString());
            depositPage.SelectStartMonth.SelectByIndex(start_month-1);
            depositPage.SelectStartDay.SelectByText(start_day.ToString());
            depositPage.SetTerm(term);

            //Assert
            Assert.AreEqual(exp_end_date, depositPage.EndDate);
        }


        [Test]
        public void Financial_Year_IsClicked_360()
        {
            //Act
            DepositPage depositPage = new DepositPage(driver);
            depositPage.FinYear360.Click();

            //Assert
            Assert.AreEqual("true", depositPage.FinYear360.GetAttribute("checked"));
        }


        [Test]
        public void Financial_Year_IsClicked_365()
        {
            //Act
            DepositPage depositPage = new DepositPage(driver);
            depositPage.FinYear365.Click();

            //Assert
            Assert.AreEqual("true", depositPage.FinYear365.GetAttribute("checked"));
        }


        [Test]
        public void Financial_Year_365_Selected_byDefault()
        {
            //Act
            DepositPage depositPage = new DepositPage(driver);
            
            //Assert
            Assert.AreEqual("true", depositPage.FinYear365.GetAttribute("checked"));
            Assert.AreEqual(null, depositPage.FinYear360.GetAttribute("checked"));
        }


        [Test]
        public void Financial_Year_OnlyOne_IsClicked()
        {
            //Act
            DepositPage depositPage = new DepositPage(driver);
            depositPage.FinYear365.Click();
            depositPage.FinYear360.Click();

            //Assert
            Assert.AreEqual(null, depositPage.FinYear365.GetAttribute("checked"));
            Assert.AreEqual("true", depositPage.FinYear360.GetAttribute("checked"));
        }

        [Test]
        public void Months_Order()
        {
            //Arrange
            string[] exp_month_names = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            //Act
            DepositPage depositPage = new DepositPage(driver);
            int i = 0;
            string[] act_months_names = new string[exp_month_names.Length];
            foreach (IWebElement el in depositPage.SelectStartMonth.Options)
            {
                act_months_names[i] = el.Text;
                i++;
            }

            //Assert
            Assert.AreEqual(exp_month_names, act_months_names);
        }


        [TestCase(2011)]
        [TestCase(2012)]
        public void Days_per_Month(int year)
        {
            //Arrange
            int[] exp_days_number = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }; ;
            if (year % 4 == 0) exp_days_number[1] = 29;
            
            //Act
            DepositPage depositPage = new DepositPage(driver);
            depositPage.SelectStartYear.SelectByText(year.ToString());

            SelectElement months_select = new SelectElement(driver.FindElement(By.Id("month")));
            IList<IWebElement> months_list = depositPage.SelectStartMonth.Options;

            int i = 0;
            int[] act_days_number = new int[12];
            foreach (IWebElement el in months_list)
            {
                el.Click();
                act_days_number[i] = depositPage.SelectStartDay.Options.Count;
                i++;
            }

            //Assert
            Assert.AreEqual(exp_days_number, act_days_number);
        }


        [TestCase("Deposit Amount: *")]
        [TestCase("Rate of Interest: *")]
        [TestCase("Investment Term: *")]
        [TestCase("Start Date:")]
        [TestCase("Financial Year:")]
        [TestCase("* - mandatory fields")]
        public void Field_Captions_td(String caption)
        {
            //Assert
            Assert.True(driver.FindElement(By.XPath($"//table//tr/td[text() = '{caption}']")).Displayed);
        }


        [TestCase("Income:")]
        [TestCase("Interest Earned:")]
        [TestCase("End Date:")]
        public void Field_Captions_th(String caption)
        {
            //Assert
            Assert.True(driver.FindElement(By.XPath($"//table//tr/th[text() = '{caption}']")).Displayed);
        }
    }
}