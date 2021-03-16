using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace web.test.tests.Pages
{
    class DepositPage
    {
        private IWebDriver driver;
        public static String amount_field_id = "amount";

        public DepositPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public IWebElement AmountField => driver.FindElement(By.Id("amount"));
        public IWebElement RateField => driver.FindElement(By.Id("percent"));
        public IWebElement TermField => driver.FindElement(By.Id("term"));

        public String AmountValue => AmountField.GetAttribute("value");
        public String RateValue => RateField.GetAttribute("value");
        public String TermValue => TermField.GetAttribute("value");

        public SelectElement SelectStartDay => new SelectElement(driver.FindElement(By.Id("day")));
        public SelectElement SelectStartMonth => new SelectElement(driver.FindElement(By.Id("month")));
        public SelectElement SelectStartYear => new SelectElement(driver.FindElement(By.Id("year")));

        public String StartDay => SelectStartDay.SelectedOption.Text;
        public String StartMonth => SelectStartMonth.SelectedOption.Text;
        public String StartYear => SelectStartYear.SelectedOption.Text;

        public IWebElement FinYear360 => driver.FindElement(By.XPath($"//*[text()='360 days']/input"));
        public IWebElement FinYear365 => driver.FindElement(By.XPath($"//*[text()='365 days']/input"));

        public String InterestStr => driver.FindElement(By.Id("interest")).GetAttribute("value");
        public Decimal InterestDec => Convert.ToDecimal(InterestStr, CultureInfo.InvariantCulture);
        public String IncomeStr => driver.FindElement(By.Id("income")).GetAttribute("value");
        public Decimal IncomeDec => Convert.ToDecimal(IncomeStr, CultureInfo.InvariantCulture);

        public String EndDate => driver.FindElement(By.Id("endDate")).GetAttribute("value");
        public IWebElement Settings => driver.FindElement(By.XPath("//div[text()='Settings']"));
        public String CurrencySign => driver.FindElement(By.Id("currency")).Text;

        public DateTime StartDate
        {
            get
            {
                String startDay = SelectStartDay.SelectedOption.Text;
                String startMonth = SelectStartMonth.SelectedOption.Text;
                String startYear = SelectStartYear.SelectedOption.Text;
                return DateTime.Parse($"{startYear} {startMonth} {startDay}");
            }
            set
            {
                SelectStartYear.SelectByValue(value.Year.ToString());
                SelectStartMonth.SelectByIndex(value.Month - 1);
                SelectStartDay.SelectByValue(value.Day.ToString());
            }
        }

        public SettingsPage OpenSettingsPage()
        {
            Settings.Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(SettingsPage.LogoutId)));
            return new SettingsPage(driver);
        }

        private void SetValue(IWebElement element, Decimal value)
        {
            element.Clear();
            foreach (char ch in value.ToString())
            {
                element.SendKeys(ch.ToString());
                Thread.Sleep(100);
            }
        }

        public void SetAmount(Decimal value)
        {
            SetValue(AmountField, value);
        }

        public void SetRate(Decimal value)
        {
            SetValue(RateField, value);
        }

        public void SetTerm(Decimal value)
        {
            SetValue(TermField, value);
        }

        public void SetAmountRateTerm(Decimal amount, Decimal rate, Decimal term)
        {
            SetValue(AmountField, amount);
            SetValue(RateField, rate);
            SetValue(TermField, term);
        }

        public void SelectFinYear(int year)
        {
            switch (year)
            {
                case 360:
                    FinYear360.Click();
                    break;
                case 365:
                    FinYear365.Click();
                    break;
                default:
                    throw new Exception($"Not valid financial year value: {year}");
            }
        }
    }
}
