using System;
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

        public IWebElement CancelBtn => _driver.FindElement(By.Id("cancel"));
        public IWebElement Logout => _driver.FindElement(By.XPath("//div[text()='Logout']"));

        public SelectElement SelectDateFormat => new SelectElement(_driver.FindElement(By.XPath(XPathDateFormatSelect)));
        public SelectElement SelectNumberFormat => new SelectElement(_driver.FindElement(By.XPath(XPathNumberFormatSelect)));
        public SelectElement SelectDefaultCurrency => new SelectElement(_driver.FindElement(By.XPath(XPathCurrencySelect)));

        public void Save()
        {
            _driver.FindElement(By.Id("save")).Click();
            new WebDriverWait(_driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.AlertIsPresent());
            IAlert alert = _driver.SwitchTo().Alert();
            Assert.AreEqual("Changes are saved!", alert.Text);
            alert.Accept();
            new WebDriverWait(_driver, TimeSpan.FromMilliseconds(10000)).Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("amount")));
        }

public string SelectedDateFormat => SelectDateFormat.SelectedOption.Text;
        public String GetSelectedNumberFormatText()
        {
            return _driver.FindElement(By.XPath(XPathNumberFormatSelect)).GetAttribute("value");
        }

        public String GetSelectedCurrencyText()
        {
            return _driver.FindElement(By.XPath(XPathCurrencySelect)).GetAttribute("value");
        }
    }
}
