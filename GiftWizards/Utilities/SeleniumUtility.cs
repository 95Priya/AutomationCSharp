using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using OpenQA.Selenium.Support.UI;

namespace GiftWizards.Utilities
{
    public class SeleniumUtility
    {
        private readonly IWebDriver driver;

        public SeleniumUtility(IWebDriver driver)
        {
            this.driver = driver;
        }

        public IWebDriver SetUp(string browserName, string appUrl)
        {
            IWebDriver driver;

            if (browserName.Equals("Chrome"))
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("no-sandbox");
                ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(driverService, options, TimeSpan.FromMinutes(3));
            }
            else if (browserName.Equals("Firefox"))
            {
                new DriverManager().SetUpDriver(new FirefoxConfig());
                driver = new FirefoxDriver();
            }
            else
            {
                throw new ArgumentException("Unsupported browser name");
            }

            driver.Manage().Window.Maximize();
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl(appUrl);

            return driver;
        }
        public static void SelectFromDropDownByText(IWebElement element, string text)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByText(text);

        }

        public static void SelectFromDropDownByIndex(IWebElement element, int index)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByIndex(index);

        }

        public static void SelectFromDropDownByValue(IWebElement element, string value)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByValue(value);

        }

        public static void TakeScreenshot(IWebDriver driver, string filePath)
        {
            if (driver != null)
            {
                ITakesScreenshot screenshotDriver = driver as ITakesScreenshot;
                Screenshot screenshot = screenshotDriver.GetScreenshot();
                screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
            }

            else
            {
                Console.WriteLine("Driver instance is null. Skipping screenshot capture.");
            }
        }

        public static void AlertAccept(IWebDriver driver)
        {

            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
            }
            catch (NoAlertPresentException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        //Alert accept dismiss
        public static void AlertDismiss(IWebDriver driver)
        {

            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Dismiss();

            }
            catch (NoAlertPresentException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        public void cleanUp()
        {
            driver.Close();

        }
        //public static void SelectDateFromCalendar(string desiredDate)
        //{
        //    string dateXPath = $"//div[@id='pddDatePicker']//td/a[contains(text(), '{desiredDate}')]";

        //    try
        //    {
        //        IWebElement dateElement = driver.FindElement(By.XPath(dateXPath));
        //        dateElement.Click();
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        // Handle the case when the date element is not found
        //        // Console.WriteLine("Date element not found in the calendar.");
        //    }
        //    catch (StaleElementReferenceException)
        //    {
        //        IWebElement dateElement = driver.FindElement(By.XPath(dateXPath));
        //        dateElement.Click();
        //    }
        //}



    }
}
