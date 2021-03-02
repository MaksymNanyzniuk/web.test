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
            int start_day = 1;
            int start_month = 1;
            int start_year = 2020;
            int term = 364; // range [0; 360/365]

            DateTime start_date = new DateTime(start_year, start_month, start_day);

            String exp_end_date = start_date.AddDays(term).ToString("dd/MM/yyyy");

            //Act
            driver.FindElement(By.Id("d365")).Click(); // available IDs: "d360", "d365"

            driver.FindElement(By.Id("term")).SendKeys("" + term);
            
            driver.FindElement(By.XPath("//*[@id='day']/option[" + start_day + "]")).Click();
            driver.FindElement(By.XPath("//*[@id='month']/option[" + start_month + "]")).Click();

            //driver.FindElement(By.XPath("//*[@id='year']/option[@value='" + start_year + "']")).Click();
            IWebElement year = driver.FindElement(By.Id("year"));
            SelectElement year_select = new SelectElement(year);
            year_select.SelectByText(start_year.ToString());

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
    }
}
