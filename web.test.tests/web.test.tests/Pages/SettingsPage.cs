using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IWebElement CancelBtn => _driver.FindElement(By.Id("cancel"));
        public IWebElement Logout => _driver.FindElement(By.XPath("//div[text()='Logout']"));

        public SelectElement DateFormatSelect => new SelectElement(_driver.FindElement(By.XPath("//th[text()='Date format:']/following-sibling::td/select")));
        public SelectElement NumberFormatSelect => new SelectElement(_driver.FindElement(By.XPath(XPathNumberFormatSelect)));
        public SelectElement DefaultCurrencySelect => new SelectElement(_driver.FindElement(By.XPath(XPathCurrencySelect)));

        public void ClickSave()
        {
            _driver.FindElement(By.Id("save")).Click();
            IAlert alert =_driver.SwitchTo().Alert();
            Assert.AreEqual("Changes are saved!", alert.Text);
            alert.Accept();
        }

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
