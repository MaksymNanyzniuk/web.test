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
        public Decimal InterestDec => Convert.ToDecimal(driver.FindElement(By.Id("interest")).GetAttribute("value"), CultureInfo.InvariantCulture);
        public Decimal Income => Convert.ToDecimal(driver.FindElement(By.Id("income")).GetAttribute("value"), CultureInfo.InvariantCulture);
        public String EndDate => driver.FindElement(By.Id("endDate")).GetAttribute("value");
        public IWebElement Settings => driver.FindElement(By.XPath("//div[text()='Settings']"));
        public String CurrencySign => driver.FindElement(By.Id("currency")).Text;

        public void OpenSettingsPage()
        {
            Settings.Click();
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//div[text()='Logout']")));
        }

        public void SetTerm(int term)
        {
            this.TermField.Clear();
            foreach (char ch in term.ToString().ToCharArray())
            {
                this.TermField.SendKeys(ch.ToString());
                Thread.Sleep(100);
            }
        }
    }
}
