using AutoItX3Lib;
using GiftWizards.Pages;
using GiftWizards.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace GiftWizards
{
    [TestClass]
    public class GiftWizardTest
    {
        IWebDriver driver;
        GiftWizardHomePage ghomePage;
        SeleniumUtility utility;

        [SetUp]
        //public void Setup()
        //{
        //    driver = utility.SetUp("Chrome", "https://stage2.netflorist.co.za/");
        //}

        [TestMethod]
        public void TestGiftWizard()
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("no-sandbox");
                ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(driverService, options, TimeSpan.FromMinutes(3));

                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl("https://stage2.netflorist.co.za/");
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);

                GiftWizardHomePage ghomePage = new GiftWizardHomePage(driver);

                //Occassion

                //List<string> ids = new List<string>() {"30970","28704","30898","31123","28982","  ","29088", "29107", "28064","31142", "bramley",
                //                                      "27924","30912","28941","27924","31281","31103","30721","30568",
                //                                      "29982","33423","29534","37826","68127","28536","30200","65182","31507","26917","33732","31448" };

                List<string> ids = new List<string>() { "28704" };
                foreach (string id in ids)
                {

                    ghomePage.OccasionClick();
                    ghomePage.SelectSuburbById(id);
                    Thread.Sleep(1000);

                    ghomePage.SelectDate(new DateTime(2023, 7, 17));

                    ghomePage.FindItNow();

                    int totalPages = GetTotalPages();
                    Console.WriteLine(totalPages);
                   // test.Log(Status.Info, "totalPages");
                    int currentPage = 1;
                    while (currentPage <= totalPages)
                    {
                        List<IWebElement> Productlist = driver.FindElements(By.XPath("//div[@class ='ProductBox']")).ToList();

                        //testing each product
                        string productCode;


                        for (int i = 0; i < Productlist.Count; i++)
                        {
                            IWebElement product = Productlist[i];
                            try
                            {
                                //Out of stock(sold out)
                                IWebElement deliveryElement = product.FindElement(By.XPath("//div[@class='NextDeliveryPhrase']"));
                                string deliveryStatus = deliveryElement.Text;


                                IWebElement nameOfProduct = driver.FindElement(By.XPath(".//a[2]"));

                                if (deliveryStatus.Equals("Out of stock"))
                                {
                                    Console.WriteLine("Product is out of stock " + nameOfProduct.Text + deliveryStatus);
                                   // test.Log(Status.Info, "Product is out of stock " + nameOfProduct.Text + deliveryStatus);
                                    continue;

                                }
                            }
                            catch (StaleElementReferenceException)
                            {
                                //
                            }
                            product.Click();

                            //Add to basket

                            IWebElement ProductName = driver.FindElement(By.Id("productName"));
                            string ProductText = ProductName.Text;
                            Console.Write(ProductText + "=");
                            //test.Log(Status.Info, ProductName.Text + "=");

                            IWebElement element = driver.FindElement(By.XPath("//span[@id='CurrentProductCode']"));
                            productCode = element.Text;
                            Console.WriteLine(productCode);
                          //  test.Log(Status.Info, productCode);

                            if (productCode.StartsWith("PER"))
                            {
                                if (IsTextFieldPresent() && !IsImageFieldPresent())
                                {
                                    Console.WriteLine("Inside text ");
                                    PersonalizedTextProduct();

                                }
                                else if (IsTextFieldPresent())
                                {
                                    Console.WriteLine("inside image");
                                    PersonalizedImageAndTextProduct();
                                    // PreviewAndConfirm();
                                }
                                else if (!IsTextFieldPresent() && IsImageFieldPresent())
                                {
                                    PersonalizedOnlyImageProduct();
                                }

                            }
                            ghomePage.addBasket();

                            //Recipient Info

                            ghomePage.FillInfo("xxx", "xxx", "0712345678", "xxx", "xxx", id);
                            //test.Log(Status.Pass, "Recipient Information filled");

                            try
                            {

                                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@class='popDeliveryDetails']")));

                                IWebElement UnavailablePopUp = driver.FindElement(By.XPath("//*[@class='popDeliveryDetails']"));
                                string popText = UnavailablePopUp.Text;
                                //Console.WriteLine(popText);

                                if (popText.Contains("Not Available"))
                                {
                                    IWebElement ProductText1 = driver.FindElement(By.XPath("//*[@id='pddErrorPrdName']"));
                                    IWebElement ProductLocation = UnavailablePopUp.FindElement(By.XPath("//*[@id='pddErrorArea']/strong[1]"));

                                    string prodLocationText = ProductLocation.Text;
                                    UnavailablePopUp.FindElement(By.XPath("//*[@id='closeDeliverypop']")).Click();
                                    Thread.Sleep(1500);

                                    Console.WriteLine("Product is Unavailable: " + prodLocationText);
                                }
                            }
                            catch (NoSuchElementException)
                            {
                                Console.WriteLine("Popup did not appear");
                            }
                            ghomePage.SelectDateFromCalendar("17"); //provide date 
                            ghomePage.NextDeliveryType();

                            driver.Navigate().Back();
                            Thread.Sleep(1000);
                            Productlist = driver.FindElements(By.XPath("//div[@class ='ProductBox']")).ToList();
                        }

                        if (currentPage < totalPages)
                        {
                            GoToNextPage();
                            currentPage++;
                        }
                    }
                }
            }

            catch (Exception)
            {
                string screenshotPath = @"D:\Priyanka\AutomationProject\GiftWizards\GiftWizards\Screenshots\screenshot.png";
                SeleniumUtility.TakeScreenshot(driver, screenshotPath);
                //test.Log(Status.Fail, "Test Failed",
                //    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                //test.Log(Status.Fail);

                throw;

            }
        }


        //check image is present 
        public bool IsImageFieldPresent()
        {
            // Find elements that match the XPath expression for the image field
            //List<IWebElement> imageFields = driver.FindElements(By.XPath("//a[@title='Upload']")).ToList();

            ReadOnlyCollection<IWebElement> imageFields = driver.FindElements(By.XPath("//a[@title='Upload']"));
            if (imageFields != null && imageFields.Count > 0)
            {
                // Return true if any image field elements are found
                return true;
            }

            return false;
        }

        //checks textfield is present 
        public bool IsTextFieldPresent()
        {
            // Find elements that match the XPath expression for the text field
            List<IWebElement> isTextFields = driver.FindElements(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")).ToList();


            // Return true if any text field elements are found
            return isTextFields.Count > 0;
        }


        //selecting date 
        public void SelectDate(DateTime date)
        {
            IWebElement dateText = driver.FindElement(By.Id("txtSelectDate"));
            dateText.Click();

            // Calculate the row and column values based on the provided date
            int row = (date.Day + 6) / 7;
            int col = ((date.Day - 1) % 7) + 1;

            By dateLocator = By.XPath($"//div[@id='ui-datepicker-div']/table/tbody/tr[{row}]/td[{col}]");

            IWebElement selectedDate = driver.FindElement(dateLocator);
            selectedDate.Click();
        }



        //preview and confirm
        public void PreviewAndConfirm()
        {
            IWebElement buttonPreview = null;
            try
            {
                buttonPreview = driver.FindElement(By.Id("btnPreview"));
            }
            catch (NoSuchElementException)
            {
                // Preview button is not present
            }

            if (buttonPreview != null && buttonPreview.Displayed)
            {
                buttonPreview.Click();
                Thread.Sleep(1000);
                buttonPreview.SendKeys(Keys.Escape);
                Console.WriteLine("Click");
            }

            IWebElement confirm = driver.FindElement(By.Id("chkNFPersonalisedTermsAndCond"));
            confirm.Click();
            Console.WriteLine("Performing preview and confirm action");
        }

        public void PersonalizedTextProduct()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement personalizedButton = null;

            personalizedButton = driver.FindElement(By.XPath("//*[contains(text(), 'Personalise this item')]"));

            wait.Until(ExpectedConditions.ElementToBeClickable(personalizedButton));

            string personalizedButtonText = personalizedButton.Text.Trim();

            if (!personalizedButton.Enabled)
            {
                try
                {
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("ul.menuL1")));
                }
                catch (WebDriverTimeoutException)
                {
                    // Handle timeout exception if necessary
                }

                try
                {
                    Actions actions = new Actions(driver);
                    actions.MoveToElement(personalizedButton).Perform();
                    personalizedButton.Click();
                }
                catch (ElementClickInterceptedException)
                {
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                    jsExecutor.ExecuteScript("arguments[0].click();", personalizedButton);
                }

                try
                {
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")));
                }
                catch (WebDriverTimeoutException)
                {
                    // Handle timeout exception if necessary
                }
            }

            List<IWebElement> textFields = driver.FindElements(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")).ToList();

            foreach (IWebElement textField in textFields)
            {
                textField.Clear();
                textField.SendKeys("X");
            }

            PreviewAndConfirm();
        }


        //product with text and image
        public void PersonalizedImageAndTextProduct()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement personalizedButton = null;

            personalizedButton = driver.FindElement(By.XPath("//*[contains(text(), 'Personalise this item')]"));

            wait.Until(ExpectedConditions.ElementToBeClickable(personalizedButton));

            string personalizedButtonText = personalizedButton.Text.Trim();


            if (personalizedButton.Enabled)
            {
                personalizedButton.Click();
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")));
            }

            List<IWebElement> textFields = driver.FindElements(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")).ToList();

            foreach (IWebElement textField in textFields)
            {
                textField.Clear();
                textField.SendKeys("X");
            }
            string imagePath = @"D:\Priyanka\Pictures\Sample.png";

            List<IWebElement> UploadImage = driver.FindElements(By.XPath("//a[@title='Upload']")).ToList();

            if (UploadImage.Count > 0)
            {
                for (int i = 0; i < UploadImage.Count && i < imagePath.Length; i++)
                {
                    UploadImage[i].Click();

                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("UploadImage"))).Click();

                    // IWebElement fileInput = driver.FindElement(By.XPath("//input[@type='file']"));

                    Thread.Sleep(1000);

                    AutoItX3 autoIt = new AutoItX3();
                    autoIt.WinWait("Open", "", 10);
                    autoIt.ControlSetText("Open", "", "Edit1", imagePath);
                    autoIt.ControlClick("Open", "", "Button1");
                    Thread.Sleep(1000);

                    try
                    {
                        IWebElement previewImage = driver.FindElement(By.XPath("//*[@id='preview']/div/div[1]"));
                        IWebElement uploadButton = driver.FindElement(By.Id("uploadbutton"));

                        previewImage.Click();
                        uploadButton.Click();
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Upload elements not found");
                    }

                    //wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[3]/div[2]/div[5]/div/div/a")));

                    IWebElement successPop = null;

                    try
                    {
                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        var loaderMask = By.CssSelector(".loaderMask");
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(loaderMask));
                        successPop = driver.FindElement(By.XPath("//html/body/div[3]/div[2]/div[5]/div/div/a"));
                    }
                    catch (NoSuchElementException)
                    {
                        // Success pop button is not present
                    }

                    if (successPop != null && successPop.Displayed)
                    {
                        successPop.Click();
                        Thread.Sleep(1000);
                    }
                }
                PreviewAndConfirm();
            }
        }

        public void PersonalizedOnlyImageProduct()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement personalizedButton = driver.FindElement(By.CssSelector(".headRibbon"));
            string personalizedButtonText = personalizedButton.Text.Trim();

            if (personalizedButtonText != "Personalise this item")  // Modify this condition based on the actual text of the active button
            {
                personalizedButton.Click();
            }

            string imagePath = @"D:\Priyanka\Pictures\Sample.png";
            List<IWebElement> UploadImage = driver.FindElements(By.XPath("//a[@title='Upload']")).ToList();
            if (UploadImage.Count > 0)
            {

                for (int i = 0; i < UploadImage.Count && i < imagePath.Length; i++)
                {
                    UploadImage[i].Click();

                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("UploadImage"))).Click();

                    AutoItX3 autoIt = new AutoItX3();
                    autoIt.WinWait("Open", "", 10);
                    autoIt.ControlSetText("Open", "", "Edit1", imagePath);
                    autoIt.ControlClick("Open", "", "Button1");
                    Thread.Sleep(1000);
                    try
                    {
                        IWebElement previewImage = driver.FindElement(By.XPath("//*[@id='preview']/div/div[1]"));
                        IWebElement uploadButton = driver.FindElement(By.Id("uploadbutton"));

                        previewImage.Click();
                        uploadButton.Click();
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Upload elements not found");
                    }

                    //wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[3]/div[2]/div[5]/div/div/a")));

                    IWebElement successPop = null;

                    try
                    {
                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        var loaderMask = By.CssSelector(".loaderMask");
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(loaderMask));
                        successPop = driver.FindElement(By.XPath("//html/body/div[3]/div[2]/div[5]/div/div/a"));
                    }
                    catch (NoSuchElementException)
                    {
                        // Success pop button is not present
                    }

                    if (successPop != null && successPop.Displayed)
                    {
                        successPop.Click();
                        Thread.Sleep(1000);
                    }

                }
                PreviewAndConfirm();
            }
        }

        //Number of pages 
        private int GetTotalPages()
        {
            IWebElement paginationElement = driver.FindElement(By.XPath("//*[@id='ctl00_PagingBarTop_PagerTopBar']/div[1]"));
            IReadOnlyCollection<IWebElement> pageLinks = paginationElement.FindElements(By.TagName("a"));
            return pageLinks.Count;
        }

        //Next Page
        private void GoToNextPage()
        {
            IWebElement nextButton = driver.FindElement(By.XPath("//*[@id='ctl00_MainContent_ctl00_LinkNext']"));
            nextButton.Click();

            // Wait for the next page to load completely
            Thread.Sleep(2000);
        }




    }
    }

