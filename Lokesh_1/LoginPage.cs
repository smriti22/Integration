using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit;
using OpenQA.Selenium.Firefox;
using System.IO;



namespace Automate
{
    class LoginPage 

    {
        IWebDriver driver;
        public string UserLogin(string username, string password,IWebDriver driver)
        {

            string file;
            file = "<tr><td colspan=\"2\"><center><B> The user login test" + "</B></center></td></tr>";
            Thread.Sleep(4000);
           //WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(10));
            //wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[data-uitest=login-link]")));
            
            driver.FindElement(By.CssSelector("a[data-uitest=login-link]")).Click();
            Thread.Sleep(3000);
            try
            {
                //---------------------------Valid User(both username and password are provided correctly)-----------------------------------------------
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    try
                    {

                        driver.SwitchTo().Frame("legoid-iframe");
                        Thread.Sleep(3000);
                        driver.FindElement(By.CssSelector("#fieldUsername")).SendKeys(username);
                        driver.FindElement(By.Id("fieldPassword")).SendKeys(password);
                        driver.FindElement(By.Id("buttonSubmitLogin")).Click();
                        Thread.Sleep(3000);
                        driver.Navigate().Refresh();
                        IWebElement anchor = driver.FindElement(By.CssSelector("a[data-uitest=edit-profile-link]"));
                        string innerText = anchor.Text;
                        Assert.AreEqual(innerText, username.ToUpper());
                        Console.WriteLine("\n\t Test Case Passed : Successfully Logged In");
                        file = file + "<tr><td>Test Case 1 :User successfully logged in</td><td>Pass</td></tr>";
                    }
                    catch
                    {
                        //Console.WriteLine("\n\t Test Case Failed : Wrong Username or Password");
                        file = file + "<tr><td>Test Case 1 : User successfully logged in</td><td>Fail</td>";
                    }

                }
                if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                {
                    //----------------------------Invalid user1(password feild if left empty)-------------------------


                    driver.SwitchTo().Frame("legoid-iframe");
                    Thread.Sleep(3000);

                    driver.FindElement(By.CssSelector("#fieldUsername")).SendKeys(username);
                    driver.FindElement(By.Id("fieldPassword")).SendKeys(password);
                    driver.FindElement(By.Id("buttonSubmitLogin")).Click();
                    IWebElement Error = driver.FindElement(By.CssSelector("p[for=fieldPassword]"));
                    if (Error.Displayed)
                    {
                        //Console.WriteLine("\n\tPassword is Required..................................................... !!! ");
                        file = file + "<tr><td>Test Case 1 :User successfully logged ind</td><td>Failed</td>";

                    }

                }

                if (string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    //----------------------------Invalid user2(Username feild if left empty)-----------------------

                    driver.SwitchTo().Frame("legoid-iframe");
                    Thread.Sleep(3000);

                    driver.FindElement(By.CssSelector("#fieldUsername")).SendKeys(username);
                    driver.FindElement(By.Id("fieldPassword")).SendKeys(password);
                    driver.FindElement(By.Id("buttonSubmitLogin")).Click();
                    IWebElement Error = driver.FindElement(By.CssSelector("p[for=fieldUsername]"));
                    if (Error.Displayed)
                    {
                        //  Console.WriteLine("\n\tUsername is Required...................................................... !!!");
                        file = file + "<tr><td>Test Case 1 :User successfully logged in</td><td>Failed";


                    }

                }

                if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                {
                    //----------------------------Invalid user3(Username feild & Password Field if left empty)-----------

                    driver.SwitchTo().Frame("legoid-iframe");
                    Thread.Sleep(3000);
                    driver.FindElement(By.CssSelector("#fieldUsername")).SendKeys(username);
                    driver.FindElement(By.Id("fieldPassword")).SendKeys(password);
                    driver.FindElement(By.Id("buttonSubmitLogin")).Click();
                    IWebElement ErrorUsername = driver.FindElement(By.CssSelector("p[for=fieldUsername]"));
                    IWebElement ErrorPassword = driver.FindElement(By.CssSelector("p[for=fieldUsername]"));
                    if (ErrorUsername.Displayed && ErrorPassword.Displayed)
                    {
                        // Console.WriteLine("\n\tUsername  And Password is Required........................................ !!!");
                        file = file + "<tr><td>Test Case 1 :User successfully logged in</td><td>Failed";

                    }

                }
            }

            catch
            {
                // Console.WriteLine("\n\t Test Case Failed : User login Failed");
            }
            finally
            {
                driver.Navigate().Refresh();
                //file = file + "</table></center></body><html>";
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\UserLoginTestReport.html";
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(file);
                sw.Close();
                fs.Close();
                //driver.Navigate().GoToUrl(path);
                //Thread.Sleep(1000);
            }
            return file;
        }
    }
}
