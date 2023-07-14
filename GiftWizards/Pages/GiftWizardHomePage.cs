using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GiftWizards.Pages
{
    public class GiftWizardHomePage
    {
        private IWebDriver driver;
        public GiftWizardHomePage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.Id, Using = "txtOcc")]
        private IWebElement Occasion;

        public void OccasionClick()
        {
            Occasion.Click();
            List<IWebElement> OccasionList = driver.FindElements(By.XPath("//div[@class='occasionList']/div")).ToList();
            int OccassionListCount = OccasionList.Count;


            foreach (IWebElement occasions in OccasionList)
            {
                Console.WriteLine(occasions.Text);

            }
            OccasionList[1].Click();
        }

        //Select suburb

        [FindsBy(How = How.Id, Using = "txtPlace")]
        private IWebElement SuburbField;

        [FindsBy(How = How.Id, Using = "txtSuburb")]
        private IWebElement SuburbText;

        [FindsBy(How = How.XPath, Using = "/html/body/ul/li[1]")]
        private IWebElement SuburbText2;

        [FindsBy(How = How.XPath, Using = "//div[@class='addressList clearFix']/label[1]")]
        private IWebElement AddList;

        [FindsBy(How = How.Id, Using = "saveSuburb")]
        private IWebElement SaveSuburb;

        public void SelectSuburbById(string id)
        {
            SuburbField.Click();
            SuburbText.Clear();
            SuburbText.SendKeys(id);
            SuburbText2.Click();

            //SuburbText.SendKeys(Keys.Down);
            //SuburbText.SendKeys(Keys.Enter);
            // WaitUntilElementVisible(AddList);
            AddList.Click();
            SaveSuburb.Click();
        }


        //DateSelect
        [FindsBy(How = How.Id, Using = "txtSelectDate")]
        private IWebElement DateField;

        public void dateClick()
        {
            DateField.Click();
        }


        public void SelectDate(DateTime date)
        {


            // Click on the date field to open the datepicker
            IWebElement dateField = driver.FindElement(By.Id("txtSelectDate"));
            dateField.Click();

            // Find all the date elements in the datepicker
            IReadOnlyCollection<IWebElement> dateElements = driver.FindElements(By.XPath("//div[@id='ui-datepicker-div']//td[@data-handler='selectDay']"));

            // Iterate through the date elements to find the desired date
            foreach (IWebElement dateElement in dateElements)
            {
                if (dateElement.Text == date.Day.ToString())
                {
                    dateElement.Click();
                    break;
                }
            }
        }
        //FindNow

        [FindsBy(How = How.Id, Using = "_btnApply")]
        private IWebElement FindNow;
        public void FindItNow()
        {
            FindNow.Click();
            Thread.Sleep(1000);
        }

        //ProductList

        [FindsBy(How = How.XPath, Using = "//div[@class ='ProductBox']")]
        private IWebElement Productlist;

        [FindsBy(How = How.Id, Using = "productName")]
        private IWebElement ProductName;

        [FindsBy(How = How.XPath, Using = "//span[@id='CurrentProductCode']")]
        private IWebElement element;




        //AddBasket

        [FindsBy(How = How.Id, Using = "ctl00_MainContent_Link")]
        private IWebElement AddToBasket;

        public void addBasket()
        {
            AddToBasket.Click();
            Thread.Sleep(1000);
        }


        // receipients info

        [FindsBy(How = How.Id, Using = "fName")]
        private IWebElement FirstName;

        [FindsBy(How = How.Id, Using = "lName")]
        private IWebElement LastName;

        [FindsBy(How = How.Id, Using = "telNo")]
        private IWebElement Phone;

        [FindsBy(How = How.Id, Using = "strtNameNo")]
        private IWebElement Street;

        [FindsBy(How = How.Id, Using = "ddlAddressType")]
        private IWebElement Address;



        [FindsBy(How = How.Id, Using = "aprtBldng")]
        private IWebElement Apartment;

        [FindsBy(How = How.Id, Using = "suburb")]
        private IWebElement Suburb2;



        [FindsBy(How = How.XPath, Using = "//button[@class='homeSprite pddButton btnNext']")]
        private IWebElement NextDelDate;

        public void FillInfo(string fName, string lName, string phoneNumber, string streetName, string apartmentBuilding, string suburb)
        {
            FirstName.SendKeys(fName);
            LastName.SendKeys(lName);
            Phone.SendKeys(phoneNumber);
            Street.SendKeys(streetName);

            Address.Click();
            SelectElement AddSelect = new SelectElement(Address);
            AddSelect.SelectByText("Residence");
            Apartment.SendKeys(apartmentBuilding);
            Suburb2.SendKeys(suburb);
            List<IWebElement> SuburbsList2 = driver.FindElements(By.XPath("/html/body/ul/li")).ToList();

            SuburbsList2[0].Click();

            NextDelDate.Click();
            Thread.Sleep(1000);
        }


        [FindsBy(How = How.CssSelector, Using = ".pddButton.btnNext")]
        private IWebElement NextDelType;

        public void NextDeliveryType()
        {
            try
            {
                NextDelType.Click();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());

                string alertText = alert.Text;
                Console.WriteLine("Alert for product: " + alertText);

                alert.Accept();
                Thread.Sleep(1000);
            }

            catch (WebDriverTimeoutException)
            {
                // Console.WriteLine("Timeout waiting for the alert to appear.");
            }
            catch (NoAlertPresentException)
            {
                Console.WriteLine("No Alert");
            }


        }


        //Personalised button
        [FindsBy(How = How.CssSelector, Using = ".headRibbon")]
        private IWebElement personalizedButton;


        //Confirm

        [FindsBy(How = How.Id, Using = "chkNFPersonalisedTermsAndCond")]
        private IWebElement Confirm;

        public void PersonalizedClick()
        {
            personalizedButton.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")));

            List<IWebElement> textFields = driver.FindElements(By.XPath("//div[@class='collapsibleContent']/div[@class='clearFix']/div/input[@type='text']")).ToList();
            foreach (IWebElement textField in textFields)
            {
                // Clear the existing text and send new input	
                textField.Clear();
                textField.SendKeys("X");
                Thread.Sleep(500);
            }
        }

        public void ConfirmClick()
        {
            Confirm.Click();
        }

        public void SelectDateFromCalendar(string desiredDate)
        {
            string dateXPath = $"//div[@id='pddDatePicker']//td/a[contains(text(), '{desiredDate}')]";

            try
            {
                IWebElement dateElement = driver.FindElement(By.XPath(dateXPath));
                dateElement.Click();
            }
            catch (NoSuchElementException)
            {
                // Handle the case when the date element is not found
                // Console.WriteLine("Date element not found in the calendar.");
            }
            catch (StaleElementReferenceException)
            {

                IWebElement dateElement = driver.FindElement(By.XPath(dateXPath));
                dateElement.Click();

            }
        }
    }

}

