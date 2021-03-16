using System;
using System.Globalization;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace web.test.tests.Pages
{
    class SettingsPage
    {
        IWebDriver _driver;

        public SettingsPage(IWebDriver driver)
        {
            _driver = driver;
        }

        const String XPathNumberFormatSelect = "//th[text()='Number format:']/following-sibling::td/select";
        const String XPathCurrencySelect = "//th[text()='Default currency:']/following-sibling::td/select";
        const String XPathDateFormatSelect = "//th[text()='Date format:']/following-sibling::td/select";

        public static String LogoutId = "//div[text()='Logout']";

        public static String DefaultDateFormat = "dd/MM/yyyy";
        public static String DefaultNumberFormat = "123,456,789.00";
        public static String DefaultCurrencyFormat = "$ - US dollar";

        public IWebElement CancelBtn => _driver.FindElement(By.Id("cancel"));
        public IWebElement Logout => _driver.FindElement(By.XPath(LogoutId));

        public SelectElement SelectDateFormat => new SelectElement(_driver.FindElement(By.XPath(XPathDateFormatSelect)));
        public SelectElement SelectNumberFormat => new SelectElement(_driver.FindElement(By.XPath(XPathNumberFormatSelect)));
        public SelectElement SelectDefaultCurrency => new SelectElement(_driver.FindElement(By.XPath(XPathCurrencySelect)));

        public void Save()
        {
            _driver.FindElement(By.Id("save")).Click();
            new WebDriverWait(_driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.AlertIsPresent());
            IAlert alert = _driver.SwitchTo().Alert();
            if (alert.Text == "Changes are saved!") alert.Accept();
            new WebDriverWait(_driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(DepositPage.amount_field_id)));
        }

        public DepositPage Cancel()
        {
            CancelBtn.Click();
            new WebDriverWait(_driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(DepositPage.amount_field_id)));
            return new DepositPage(_driver);
        }

        public String SelectedDateFormat => SelectDateFormat.SelectedOption.Text;
        public String SelectedNumberFormat => SelectNumberFormat.SelectedOption.Text;
        public String SelectedCurrency => SelectDefaultCurrency.SelectedOption.Text;

        public void ResetToDefault()
        {
            SelectDateFormat.SelectByText(DefaultDateFormat);
            SelectNumberFormat.SelectByText(DefaultNumberFormat);
            SelectDefaultCurrency.SelectByText(DefaultCurrencyFormat);
            Save();
        }

        public NumberFormatInfo GetSelectedNFI(String format)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", true).NumberFormat;
            switch (format)
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
                    throw new Exception($"Not valid number format value: {format}");
            }
            return nfi;
        }

        public String GetExpCurrencySign(String format)
        {
            String str = "";
            switch (format)
            {
                case "$ - US dollar":
                    str = "$";
                    break;
                case "€ - euro":
                    str = "€";
                    break;
                case "£ - Great Britain Pound":
                    str = "£";
                    break;
                default:
                    throw new Exception($"Not valid currency format value: {format}");
            }
            return str;
        }
    }
}
