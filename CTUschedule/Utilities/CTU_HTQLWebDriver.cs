using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication.ExtendedProtection;
using OpenQA.Selenium.DevTools;
using Newtonsoft.Json;
using OpenQA.Selenium.DevTools.V85.Network;
using System.Threading;
using System.IO;
using OpenQA.Selenium.Interactions;

namespace Utilities
{
    
    public class CTU_HTQLWebDriver
    {
        public static CTU_HTQLWebDriver Instance;

        public string MainPage = "https://dkmh.ctu.edu.vn/htql/sinhvien/hindex.php";
        public string CourseCatalogPage = "https://dkmhfe.ctu.edu.vn/dangkyhocphan/sinhvien/danhmuchocphan";
        public string SignInPage = "https://htql.ctu.edu.vn/htql/login.php";

        private ChromeOptions options;
        public IWebDriver driver;
        public WebDriverWait wait;


        public CTU_HTQLWebDriver()
        {
            Instance = this;
            AssignWebDriver();
        }


        private void AssignWebDriver()
        {
            options = new ChromeOptions();
            //options.AddArgument("--headless");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(chromeDriverService, options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void CloseWeb()
        {
            driver.Quit();
        }
    }

    public class HTQL_Signin
    {
        private CTU_HTQLWebDriver HTQLWebDriver = CTU_HTQLWebDriver.Instance;

        public string CapchaPath = System.IO.Path.GetTempPath() + "capcha.png";

        private IWebDriver driver
        {
            get => HTQLWebDriver.driver;
        }

        private WebDriverWait wait
        {
            get => HTQLWebDriver.wait;
        }

        public HTQL_Signin()
        {
            
        }

        public void NavigateToSignin()
        {
                driver.Navigate().GoToUrl(HTQLWebDriver.SignInPage);
                GetAndShowCapchaImage();
        }

        public bool SignIn(string UserName, string Password, string Capcha)
        {
            try
            {
                // Write account, password
                var username = driver.FindElement(By.Id("txtDinhDanh"));
                var password = driver.FindElement(By.Id("txtMatKhau"));
                var capcha = driver.FindElement(By.Id("txtMaBaoVe"));
                var loginButton = driver.FindElement(By.Name("login"));

                username.SendKeys(UserName);
                password.SendKeys(Password);
                capcha.SendKeys(Capcha);
                loginButton.Click();

                // check login fail
                try
                {
                    Thread.Sleep(300);
                    driver.SwitchTo().Alert().Accept();
                    return false;
                }
                catch (NoAlertPresentException e)
                {
                    // no alert
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Có lỗi xảy ra: " + ex.Message);
            }
            return false;
        }

        public void ImageViewer(string Path)
        {
            if (!System.IO.File.Exists(Path)) return;
            Process.Start("explorer.exe", Path);
        }

        public void GetAndShowCapchaImage()
        {
            try
            {
                Thread.Sleep(300);
                var capchaImg = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("verify_code")));
                //var capchaImg = driver.FindElement(By.Id("verify_code"));
                Screenshot screenShot = ((ITakesScreenshot)capchaImg).GetScreenshot();
                screenShot.SaveAsFile(CapchaPath);
                //ImageViewer(CapchaPath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
           
        }
    }

    public class HTQL_CourseCatalog
    {
        public static HTQL_CourseCatalog Instance;

        private CTU_HTQLWebDriver HTQLWebDriver;

        public IWebDriver driver
        {
            get => HTQLWebDriver.driver;
        }

        private WebDriverWait wait
        {
            get => HTQLWebDriver.wait;
        }

        public NetworkAdapter network { get; set; }

        private Actions actions;



        public HTQL_CourseCatalog()
        {
            this.HTQLWebDriver = CTU_HTQLWebDriver.Instance;
            actions = new Actions(driver);
            InitDevtool();
        }

        private async void InitDevtool()
        {
            var devTools = driver as IDevTools;
            var session = devTools.GetDevToolsSession();
            network = new NetworkAdapter(session);
            var enableNetworkTask = network.Enable(new EnableCommandSettings());
            await enableNetworkTask;

            //network.ResponseReceived += async (sender, e) =>
            //{
            //    // Kiểm tra nếu nội dung phản hồi là JSON
            //    if (e.Response.MimeType == "application/json")
            //    {
            //        try
            //        {
            //            var responseBody = await network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = e.RequestId });
            //            //Console.WriteLine($"Response URL: {e.Response.Url}");
            //            //Console.WriteLine($"Response Status: {e.Response.Status}");
            //            //Console.WriteLine($"Response Headers: {string.Join(", ", e.Response.Headers)}");
            //            //Console.WriteLine($"Response Body: {responseBody.Body}");
            //            //File.WriteAllText($@"E:\hello.json", responseBody.Body);
            //            ResponseData = responseBody.Body;

            //            //Debug.WriteLine(ResponseData);
            //        }
            //        catch (Exception ex)
            //        {
            //            //Debug.WriteLine(ex.Message);
            //        }
            //    }
            //};
        }

        public void NavigateToCourseCatalog()
        {
            //truy cập Trang đăng ký học phần
            driver.Navigate().GoToUrl(HTQLWebDriver.MainPage);
            var dkmh = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"page-body\"]/div[1]/table/tbody/tr[1]/td[2]/div/table/tbody/tr[1]/td[2]/div/span/img")));
            //var dkmh = driver.FindElement(By.XPath("//*[@id=\"page-body\"]/div[1]/table/tbody/tr[1]/td[2]/div/table/tbody/tr[1]/td[2]/div/span/img"));
            dkmh.Click();
            // chờ 3s để load trang
            Thread.Sleep(3000);
            // nảy tới trang catalog
            driver.Navigate().GoToUrl(HTQLWebDriver.CourseCatalogPage);
        }

        public void ClearAllText()
        {
            actions.KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).Build().Perform();
            actions.SendKeys(Keys.Delete).Build().Perform();
        }

        public async void QuickSearch(string searchText)
        {
            await Task.Run(() =>
            {
                try
                {
                    var SearchBox = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("rc_select_2")));
                    ClearAllText();
                    SearchBox.SendKeys(searchText);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
            
        }

        public async void Search(string searchText)
        {
            try
            {
                var SearchBox = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("rc_select_2")));
                SearchBox.Click();
                ClearAllText();
                //Enter MaHocPhan     
                SearchBox.SendKeys(searchText);


                var SearchButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"root\"]/div/div/main/div[3]/div[1]/div[1]/div/div[3]/span")));
      
                SearchButton.Click();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("Có lỗi xảy ra: " + ex.Message);
                driver.Navigate().Refresh();
            }

        }
    }
}
