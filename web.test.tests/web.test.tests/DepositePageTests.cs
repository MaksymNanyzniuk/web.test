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
            Decimal[] amount_array = { 0m, 1m, 100.54m, 100000m, 100001m };
            Decimal[] rate_array = { 0m, 1m, 13.5m, 100m, 101m };
            Decimal[] term_array = { 0m, 1m, 7.7m, 0m, 0m };
           
            var year_map = new Dictionary<String, int>();
            year_map.Add("d360", 360);
            year_map.Add("d365", 365);

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

                term_array [term_array.Length - 2]= year_length;
                term_array[term_array.Length - 1] = year_length + 1;

                //Act
                driver.FindElement(By.Id(financial_year_Id)).Click();

                for (int i = 0; i < amount_array.Length; i++)
                {
                    driver.FindElement(By.Id("amount")).Clear();
                    driver.FindElement(By.Id("amount")).SendKeys(amount_array[i].ToString());
                    for (int j = 0; j < rate_array.Length; j++)
                    {
                        driver.FindElement(By.Id("percent")).Clear();
                        driver.FindElement(By.Id("percent")).SendKeys(rate_array[j].ToString());
                        for (int k = 0; k < term_array.Length; k++)
                        {
                            driver.FindElement(By.Id("term")).Clear();
                            driver.FindElement(By.Id("term")).SendKeys(term_array[k].ToString());
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

                            act_interest = Convert.ToDecimal(driver.FindElement(By.Id("interest")).GetAttribute("value"));
                            act_income = Convert.ToDecimal(driver.FindElement(By.Id("income")).GetAttribute("value"));

                            act_interest_array[n] = act_interest;
                            act_income_array[n] = act_income;

                            Console.WriteLine("n=" + n + ". amount=" + amount_array[i] + "; rate=" + rate_array[j] + "; term=" + term_array[k] + ";");
                            Console.WriteLine("exp_interest=" + exp_interest + "; act_interest=" + act_interest + "; exp_income= " + exp_income + "; act_income= " + act_income);

                            n++;
                        }
                    }
                }
            }

            //Assert
            Assert.AreEqual(exp_interest_array, act_interest_array);
            Assert.AreEqual(exp_income_array, act_income_array);
        }
        

        [Test]
        public void End_Date()
        {
            //Arrange
            int start_day = 15;
            int start_month = 7;
            int start_year = 2019;
            int[] term_array = { 0, 1, 16, 17, 169, 170, 229, 365 };

            //Act
            SelectElement year_select = new SelectElement(driver.FindElement(By.Id("year")));
            year_select.SelectByText(start_year.ToString());

            SelectElement month_select = new SelectElement(driver.FindElement(By.Id("month")));
            month_select.SelectByIndex(start_month-1);

            SelectElement day_select = new SelectElement(driver.FindElement(By.Id("day")));
            day_select.SelectByText(start_day.ToString());
            
            //driver.FindElement(By.XPath("//*[@id='month']/option[" + start_month + "]")).Click();

            DateTime start_date = new DateTime(start_year, start_month, start_day);

            String[] exp_end_date_array = new String[term_array.Length];
            String[] act_end_date_array = new string[term_array.Length];

            IWebElement term_field = driver.FindElement(By.Id("term"));

            for (int i = 0; i < term_array.Length; i++)
            {
                exp_end_date_array[i] = start_date.AddDays(term_array[i]).ToString("dd/MM/yyyy");

                term_field.Clear();
                term_field.SendKeys(term_array[i].ToString());

                act_end_date_array[i] = driver.FindElement(By.Id("endDate")).GetAttribute("value");
            }

            //Assert
            Assert.AreEqual(exp_end_date_array, act_end_date_array);
        }


        [Test]
        public void Financial_Year_IsClicked_360()
        {
            //Arrange
            String radio_button_id = "d360";


            //Act
            driver.FindElement(By.Id(radio_button_id)).Click();


            //Assert
            Assert.AreEqual("true", driver.FindElement(By.Id(radio_button_id)).GetAttribute("checked"));
        }


        [Test]
        public void Financial_Year_IsClicked_365()
        {
            //Arrange
            String radio_button_id = "d365";


            //Act
            driver.FindElement(By.Id(radio_button_id)).Click();


            //Assert
            Assert.AreEqual("true", driver.FindElement(By.Id(radio_button_id)).GetAttribute("checked"));
        }


        [Test]
        public void Months_Order()
        {
            //Arrange
            string[] exp_month_names = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
            

            //Act
            SelectElement months_select = new SelectElement(driver.FindElement(By.Id("month")));
            IList<IWebElement> months_list = months_select.Options;

            int i = 0;
            string[] act_months_names = new string[months_list.Count];
            foreach (IWebElement el in months_list)
            {
                act_months_names[i] = el.Text;
                i++;
            }


            //Assert
            Assert.AreEqual(exp_month_names, act_months_names);
        }


        [Test]
        public void Days_per_Month_common_year()
        {
            //Arrange
            int year = 2010;
            int[] exp_days_number = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            //Act
            SelectElement year_select = new SelectElement(driver.FindElement(By.Id("year")));
            year_select.SelectByText(year.ToString());

            SelectElement months_select = new SelectElement(driver.FindElement(By.Id("month")));
            IList<IWebElement> months_list = months_select.Options;

            int i = 0;
            int[] act_days_number = new int[12];
            foreach (IWebElement el in months_list)
            {
                el.Click();
                SelectElement day = new SelectElement(driver.FindElement(By.Id("day")));
                act_days_number[i] = day.Options.Count;
                i++;
            }

            //Assert
            Assert.AreEqual(exp_days_number, act_days_number);
        }


        [Test]
        public void Days_per_Month_leap_year()
        {
            //Arrange
            int year = 2012;
            int[] exp_days_number = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            //Act
            SelectElement year_select = new SelectElement(driver.FindElement(By.Id("year")));
            year_select.SelectByText(year.ToString());

            SelectElement months_select = new SelectElement(driver.FindElement(By.Id("month")));
            IList<IWebElement> months_list = months_select.Options;

            int i = 0;
            int[] act_days_number = new int[12];
            foreach (IWebElement el in months_list)
            {
                el.Click();
                SelectElement day = new SelectElement(driver.FindElement(By.Id("day")));
                act_days_number[i] = day.Options.Count;
                i++;
            }

            //Assert
            Assert.AreEqual(exp_days_number, act_days_number);
        }

    }
}
