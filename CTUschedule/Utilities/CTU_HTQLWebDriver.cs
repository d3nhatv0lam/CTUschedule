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
using System.Threading;
using System.IO;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.DevTools.V138.Network;
using OpenQA.Selenium.Internal;

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

        private event EventHandler _webLogedOut;

        public event EventHandler WebLoggedOut
        {
           add
            {
                _webLogedOut += value;
            }
            remove
            {
                _webLogedOut -= value;
            }
        }

        public void OnLoggedOut()
        {
            _webLogedOut(this, new EventArgs());
        }


        public CTU_HTQLWebDriver()
        {
            if (Instance == null)
                Instance = this;
            AssignWebDriver();
        }


        private void AssignWebDriver()
        {
            options = new ChromeOptions();
            //options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-application-cache");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(chromeDriverService, options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void CloseWeb()
        {
            driver.Quit();
            driver.Dispose();
            driver = null;
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

        public void NavigateToSignin()
        {
                driver.Navigate().GoToUrl(HTQLWebDriver.SignInPage);
                //GetAndShowCapchaImage();
        }

        public bool SignIn(string UserName, string Password)
        {
            try
            {
                // Write account, password
                var username = driver.FindElement(By.Id("usernameUserInput"));
                var password = driver.FindElement(By.Id("password"));
                var loginButton = driver.FindElement(By.Id("sign-in-button"));

                username.SendKeys(UserName);
                password.SendKeys(Password);
                loginButton.Click();

                // check login fail
                try
                {
                    Thread.Sleep(500);
                    var usernameErrorText = driver.FindElement(By.Id("usernameErrorText"));
                    if (usernameErrorText.Displayed) return false;
                    var passwordErrorText = driver.FindElement(By.Id("passwordErrorText"));
                    if (passwordErrorText.Displayed) return false;
                    var signinFail = driver.FindElement(By.Id("error-msg"));
                    if (signinFail.Displayed) return false;
                    //driver.SwitchTo().Alert().Accept();
                    Thread.Sleep(2000);
                    return true;
                }
                catch (Exception e)
                {
                    // no alert
                    // nhảy qua trang khác nên lỗi k tìm được
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

    public class HTQL_CourseCatalog : IDisposable
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
            if (Instance == null)
                Instance = this;
            this.HTQLWebDriver = CTU_HTQLWebDriver.Instance;
            actions = new Actions(driver);

            var devTools = driver as IDevTools;
            var session = devTools.GetDevToolsSession();
            network = new NetworkAdapter(session);

            InitDevtool();
        }

        private async void InitDevtool()
        {
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

        private void ForceNavigateToCourseCatalog()
        {
            //truy cập Trang đăng ký học phần
            driver.Navigate().GoToUrl(HTQLWebDriver.MainPage);
            try
            {
                var dkmh = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"page-body\"]/div[1]/table/tbody/tr[1]/td[2]/div/table/tbody/tr[1]/td[2]/div/span/img")));
                dkmh.Click();
                // chờ 3s để load trang
                Thread.Sleep(3000);
                // nảy tới trang catalog
                driver.Navigate().GoToUrl(HTQLWebDriver.CourseCatalogPage);
            } catch
            {
                // không truy cập được vào trang main => văng về login
                HTQLWebDriver.OnLoggedOut();
            }
             //var dkmh = driver.FindElement(By.XPath("//*[@id=\"page-body\"]/div[1]/table/tbody/tr[1]/td[2]/div/table/tbody/tr[1]/td[2]/div/span/img"));
            
        }

        public bool IsDriveUrlCatalogPage()
        {
            return driver.Url == HTQLWebDriver.CourseCatalogPage;
        }

        public void NavigateToCourseCatalog()
        {
            // thử refesh lại
            driver.Navigate().Refresh();
            // giả định đang ở web List HP
            try
            {
                // check has popup -> đã logout
                var logoutPopup = driver.FindElement(By.XPath("/html/body/div[3]/div/div[2]/div/div[2]/div/div/div/div[2]/button"));
                // chờ 5 giây để văng ra
                Thread.Sleep(5000);
                HTQLWebDriver.OnLoggedOut();
            }
            catch
            {
                // không có popup log out
                // nếu đang ở web => còn dùng được
                if (driver.Url == HTQLWebDriver.CourseCatalogPage) return;
                // giả định đang ở web sign-in
                if (driver.Url == HTQLWebDriver.SignInPage)
                {
                    HTQLWebDriver.OnLoggedOut();
                    return;
                }
                ForceNavigateToCourseCatalog();
            }
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
                    var SearchBox = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("rc_select_2")));
                    SearchBox.Click();
                    ClearAllText();
                    SearchBox.SendKeys(searchText);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        public void Search(string searchText)
        {
            try
            {
                var SearchBox = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("rc_select_2")));
                SearchBox.Click();
                ClearAllText();
                //Enter MaHocPhan     
                SearchBox.SendKeys(searchText);


                var SearchButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("span[role=\"img\"][aria-label=\"search\"].anticon-search")));
                wait.Until(driver => SearchBox.GetAttribute("value") == searchText);
                SearchButton.Click();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Có lỗi xảy ra: " + ex.Message);
                driver.Navigate().Refresh();
            }

        }

        public void Dispose()
        {
           if (network is IDisposable disposable) disposable.Dispose();
        }
    }
}
