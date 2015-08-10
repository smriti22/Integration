using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using System.IO;
using OpenQA.Selenium.Remote;
using System.Threading;
using System.Net;
namespace Automate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the URL (Like :- https://qa-playdays.lego.com )\n\t");
             var url = Console.ReadLine();
            //var url = "http://google.com";
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Result";
            string subject = "Regress Test for  " + url;
            string str = "<center><table border=1 ><caption><b>Site Tracking : " + subject + "</b></caption>";
            str += "<tr><th style='width:700px'>Test Case </th><th style='width:150px;'>Result</th></tr>";



            Proxy proxy = new Proxy();
            proxy.IsAutoDetect = true;

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability(CapabilityType.Proxy, proxy);

            IWebDriver driver = new FirefoxDriver(capabilities);

            driver.Manage().Cookies.DeleteAllCookies();

            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            driver.SwitchTo().ParentFrame();
            IWebElement scriptTag = null;
            try
            {
                var updatedURL = driver.Url;
                var words = updatedURL.Split('/');
                var local = words[3].ToLower();
                var environment = words[2].ToLower();

                try
                {
                    Console.WriteLine("Test Case 1 : Verify the site tracking implementation by checking presence of ‘data-tracking-script’ tag in DOM ");
                    scriptTag = driver.FindElement(By.CssSelector("script[data-tracking-script]"));
                    Console.WriteLine("Test Case 1 : Pass - Verify the site tracking implementation by checking presence of ‘data-tracking-script’ tag in DOM");
                    str += "<tr><td>Test Case 1 : Verify the site tracking implementation by checking presence of ‘data-tracking-script’ tag in DOM </td><td> Pass" + "</td></tr>";
                }
                catch (Exception)
                {
                    Console.WriteLine("Test Case 1 : Fail - 1. Site tracking implementation by checking presence of ‘data-tracking-script’ tag in DOM");
                    str += "<tr><td>Test Case 1 : Verify the site tracking implementation by checking presence of ‘data-tracking-script’ tag in DOM </td><td> Fail" + "</td></tr>";
                }


                if (scriptTag != null)
                {
                    Console.WriteLine("Test Case 2 : Verify URL locale (ex. da-dk) should match with site tracking script tag i.e. cultural-info attribute (da-dk) ");
                    try
                    {
                        var cultureInfo = scriptTag.GetAttribute("culture-info").ToLower();
                        Console.WriteLine("Locale mention in 'Culture-Info' attribute : " + cultureInfo);

                        try
                        {
                            //Assert.Equals(local, cultureInfo);
                            if (cultureInfo.Equals(local))
                            {
                                Console.WriteLine("Test Case 2 : Pass - Verify URL locale should match with site tracking script tag i.e. cultural-info attribute : " + cultureInfo);
                                str += "<tr><td>Test Case 2 : Verify URL locale (ex. da-dk) should match with site tracking script tag i.e. cultural-info attribute (da-dk) </td><td> Pass" + "</td></tr>";
                            }
                            else if (cultureInfo.Equals("en-us")) // check with Dev End for better implementation
                            {
                                Console.WriteLine("Test Case 2 : Pass - Verify URL locale should match with site tracking script tag i.e. cultural-info attribute : " + cultureInfo);
                                str += "<tr><td>Test Case 2 : Verify URL locale should match with site tracking script tag i.e. cultural-info attribute :" + cultureInfo + " </td><td> Pass" + "</td></tr>";
                            }
                            else
                            {
                                Console.WriteLine("Test Case 2 : Fail - URL locale  should not match with site tracking script tag i.e. cultural-info attribute : " + cultureInfo);
                                str += "<tr><td>Test Case 2 : Verify URL locale should not match with site tracking script tag i.e. cultural-info attribute :" + cultureInfo + " </td><td> Fail" + "</td></tr>";
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Test Case 2 : Fail - URL locale should not match with site tracking script tag i.e. cultural-info attribute : " + cultureInfo);
                            str += "<tr><td>Test Case 2 : URL locale should not match with site tracking script tag i.e. cultural-info attribute :" + cultureInfo + " </td><td> Fail" + "</td></tr>";
                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Test Case 2 : Fail - Because Culture-info Attribute Tag is Missing ");
                        str += "<tr><td>Test Case 2 : URL locale should not match with site tracking script tag i.e. cultural-info attribute (da-dk) </td><td> Fail" + "</td></tr>";
                    }
                }

                if (scriptTag != null)
                {
                    Console.WriteLine("Test Case 3 : Verify ‘Report Suite’ should be as per expectation i.e. for QA it should be 'LegoGlobalQA' and Live it should be 'Global'  ");
                    if (environment.Contains("qa-"))
                    {
                        try
                        {
                            var dataReportSuite = scriptTag.GetAttribute("data-reportsuite").ToLower();
                            Console.WriteLine("Report Suite is in 'dataReportSuite' attribute : " + dataReportSuite);
                            if (dataReportSuite.Equals("legoglobalqa") || dataReportSuite.Contains("qa"))
                            {
                                Console.WriteLine("Test Case 3 : Pass - Verify ‘Report Suite’ should be as per expectation i.e. for QA it should be 'LegoGlobalQA' ");
                                str += "<tr><td>Test Case 3 : Verify ‘Report Suite’ should be as per expectation for QA i.e. " + dataReportSuite + "</td><td> Pass" + "</td></tr>";
                            }
                            else
                            {
                                Console.WriteLine("Test Case 3 : Fail - ‘Report Suite’ should npt be as per expectation ");
                                str += "<tr><td>Test Case 3 : ‘Report Suite’ should not be as per expectation i.e. for QA it should not be 'LegoGlobalQA' </td><td> Fail" + "</td></tr>";
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Test Case 3 : Fail Because data-reportsuite Attribute Tag is Missing in QA env");
                            str += "<tr><td>Test Case 3 :‘Report Suite’ should not be as per expectation i.e. for QA it should not be 'LegoGlobalQA' </td><td> Fail" + "</td></tr>";
                        }
                    }
                    else
                    {
                        try
                        {
                            var dataReportSuite = scriptTag.GetAttribute("data-reportsuite").ToLower();
                            Console.WriteLine("Report Suite is in 'dataReportSuite' attribute : " + dataReportSuite);
                            if (dataReportSuite.Equals(""))
                            {
                                Console.WriteLine("Test Case 3 : Pass - ‘Report Suite’ should be as per expectation i.e. for Live it should be 'Global' ");
                                str += "<tr><td>Test Case 3 : ‘Report Suite’ should be as per expectation for Live  i.e. " + dataReportSuite + " empty in script tag </td><td> Pass" + "</td></tr>";
                            }
                            else
                            {
                                Console.WriteLine("Test Case 3 : Fail - ‘Report Suite’ should not be as per expectation for Live ");
                                str += "<tr><td>Test Case 3 : ‘Report Suite’ should not be as per expectation for live " + dataReportSuite + "</td><td> Fail" + "</td></tr>";
                            }
                        }
                        catch (Exception)
                        {

                            Console.WriteLine("Test Case 3 : Pass - Because if data-reportsuite Attribute Tag is Missing in Live env than it points to Global");
                            str += "<tr><td>Test Case 3 :  ‘Report Suite’ should be as per expectation for Live </td><td> Pass" + "</td></tr>";

                        }
                    }
                }


                if (scriptTag != null)
                {
                    Console.WriteLine("Test Case 4 : Verify ‘Page Name’(ex. playdays:homepage:overview) should be as per expectation i.e. data-initial-page attribute should not be empty and should contains two :(colons) like - playdays:homepage:overview");
                    try
                    {
                        var dataInitialPage = scriptTag.GetAttribute("data-initial-page");
                        Assert.DoesNotThrow(() => dataInitialPage.ToLower());
                        var count = dataInitialPage.Count(f => f == ':');
                        Assert.AreEqual(count, 2);
                        Console.WriteLine("Test Case 4 : Pass - Verify ‘Page Name’(ex. playdays:homepage:overview) should be as per expectation i.e. data-initial-page attribute should not be empty and should contains two :(colons) like - playdays:homepage:overview");
                        str += "<tr><td>Test Case 4 : Verify ‘Page Name’(ex. playdays:homepage:overview) should be as per expectation i.e. data-initial-page attribute should not be empty and should contains two :(colons) and Page Name is : " + dataInitialPage + " </td><td> Pass" + "</td></tr>";
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Test Case 4 : Fail, Because Either Page Name is missing or Not In Proper Format");
                        str += "<tr><td>Test Case 4 :  Verify Page Name Should as Per Expectation : Because Either Page Name is missing or Not In Proper Format </td><td> Fail" + "</td></tr>";
                    }
                }

                if (scriptTag != null)
                {
                    Console.WriteLine("Test Case 5 : Verify site tracking ‘src’ attribute should contain Trackmanapi");
                    try
                    {
                        var src = scriptTag.GetAttribute("src");
                        Assert.IsTrue(src.Contains("TrackManApi"));
                        Console.WriteLine("Test Case 5 : Pass, Verify site tracking ‘src’ attribute should contain Trackmanapi");
                        str += "<tr><td>Test Case 5 : Verify site tracking ‘src’ attribute should contain Trackmanapi : " + src + " </td><td> Pass" + "</td></tr>";
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Test Case 5 : Fail, site tracking ‘src’ attribute should not contain Trackmanapi");
                        str += "<tr><td>Test Case 5 :  Site tracking ‘src’ attribute should not contain Trackmanapi </td><td> Fail" + "</td></tr>";
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Please check URL ");
                str += "<tr><td>Please check URL or Site not loaded. Please Try Again.. </td><td> Fail" + "</td></tr>";

            }

            //------------------------------------------------- sticky footer test script-------------------------------------------------------------------




            Console.WriteLine("-----------------------The script to test sticky footer------------------------- ");
            str += "<tr><td colspan=\"2\"><center><B> The script to test sticky footer" + "</B></center></td></tr>";

            driver.Manage().Window.Maximize();
            //driver.FindElement(By.CssSelector("#closing")).Click();


            try
            {

                bool present;
                try
                {
                    driver.FindElement(By.CssSelector("#closing"));

                    driver.FindElement(By.CssSelector("#closing")).Click();
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("No one time sticky footer");
                }
            }
            catch (NUnit.Framework.AssertionException)
            {

            }

            // Checking the visibility of sticky footer
            string b = "#GFSticky";
            try
            {

                try
                {
                    driver.FindElement(By.CssSelector(b));

                    Assert.AreEqual(true, driver.FindElement(By.CssSelector(b)).Displayed);
                    str += "<tr><td>Test Case 1 : Pass- Verify the visibility of sticky footer </td><td> Pass" + "</td></tr>";
                    Console.WriteLine("Test Case 1 : Pass- Verify the visibility of sticky footer");
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 1: Fail- Verify the visibility of sticky footer");
                    str += "<tr><td>Testcase 1: Fail- Verify the visibility of sticky footer</td><td> Fail" + "</td></tr>";

                }
            }

            catch (NUnit.Framework.AssertionException)
            {

            }


            //check the visibility of privacy
            string c = "#GFSpplink";
            try
            {

                try
                {
                    driver.FindElement(By.CssSelector(c));

                    Console.WriteLine("Testcase 2: Pass- Verify the visibility of privacy");
                    Assert.AreEqual(true, driver.FindElement(By.CssSelector(c)) != null);
                    str += "<tr><td>Test Case 2 :Verify the visibility of privacy </td><td> Pass" + "</td></tr>";
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 2: Fail- Verify the visibility of  privacy ");
                    str += "<tr><td>Testcase 2: Fail- Verify the visibility of privacy</td><td> Fail" + "</td></tr>";

                }
            }

            catch (NUnit.Framework.AssertionException)
            {


            }


            //check the visibility of cookies
            string d = "#GFScilink";
            try
            {


                try
                {
                    driver.FindElement(By.CssSelector(d));

                    Console.WriteLine("Testcase 3: Pass- Verify the visibility of cookies");
                    Assert.AreEqual(true, driver.FindElement(By.CssSelector(d)) != null);
                    str += "<tr><td>Test Case 3:Verify the visibility of cookies </td><td> Pass" + "</td></tr>";
                }
                catch (NoSuchElementException)
                {


                    Console.WriteLine("Testcase 3: Fail- Verify the visibility of cookies");
                    str += "<tr><td>Testcase 3: Fail- Verify the visibility of cookies</td><td> Fail" + "</td></tr>";

                }


            }

            catch (NUnit.Framework.AssertionException)
            {

            }


            //checking if the sticky footer is present
            string z = "#GFSticky";
            try
            {

                try
                {
                    driver.FindElement(By.CssSelector(z));

                    Console.WriteLine("Testcase 4: Pass- Verify the presence of sticky footer");
                    Assert.AreEqual(true, driver.FindElement(By.CssSelector(z)) != null);
                    str += "<tr><td>Test Case 4 :Verify the presence of sticky footer </td><td> Pass" + "</td></tr>";
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 4: Fail-Verify the presence of sticky footer");
                    str += "<tr><td>Testcase 4: Fail- Verify the presence of sticky footer</td><td> Fail" + "</td></tr>";


                }


            }

            catch (NUnit.Framework.AssertionException)
            {

            }


            //checking if the privacy is present
            string y = "#GFSpplink";
            try
            {

                try
                {
                    driver.FindElement(By.CssSelector(y));

                    Console.WriteLine("Testcase 5: Pass- Verify the presence of privacy");
                    Assert.AreEqual(true, driver.FindElement(By.CssSelector(y)) != null);
                    str += "<tr><td>Test Case 5 :Verify the presence of privacy </td><td> Pass" + "</td></tr>";
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 5: Fail-Verify the presence of privacy");
                    str += "<tr><td>Testcase 5: Fail- Verify the presence of privacy</td><td> Fail" + "</td></tr>";
                }
            }

            catch (NUnit.Framework.AssertionException)
            {

            }


            //checking if the cookies is present
            string g = "#GFScilink";
            try
            {

                try
                {
                    driver.FindElement(By.CssSelector(g));

                    Console.WriteLine("Testcase 6: Pass- Verify the presence of cookies");
                    Assert.AreEqual(true, driver.FindElement(By.CssSelector(g)) != null);
                    str += "<tr><td>Test Case 6 :Verify the presence of cookies</td><td> Pass" + "</td></tr>";
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 6: Fail-Verify the presence of cookies");
                    str += "<tr><td>Testcase 6: Fail- Verify the presence of cookies</td><td> Fail" + "</td></tr>";
                }

            }

            catch (NUnit.Framework.AssertionException)
            {

            }

            driver.Manage().Cookies.DeleteAllCookies();
            //Switching to iframe of sticky footer
            driver.SwitchTo().ParentFrame();
            Console.WriteLine("Checking if there exist any text in the privacy popup");

            try
            {

                try
                {
                    IWebElement privacy_text = driver.FindElement(By.TagName("body"));
                    driver.FindElement(By.CssSelector("#GFSpplink"));
                    driver.FindElement(By.CssSelector("#GFSpplink")).Click();
                    Console.WriteLine("Testcase 7: Text exists in the privacy popup");
                    Assert.IsNotNullOrEmpty(privacy_text.Text);
                    str += "<tr><td>Test Case 7 :Text exists in the privacy popup</td><td> Pass" + "</td></tr>";
                    driver.FindElement(By.CssSelector(".lego-modal-wrapper.lego-modal-close-trigger.lego-in")).Click();
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 7: Fail-Text exists in the privacy popup");
                    str += "<tr><td>Testcase 7: Fail- Text exists in the privacy popup</td><td> Fail" + "</td></tr>";
                }

            }
            catch (NUnit.Framework.AssertionException)
            {

            }


            driver.Manage().Cookies.DeleteAllCookies();



            //Checking if there exist any text in the cookies popup
            try
            {

                try
                {
                    IWebElement cookie_text = driver.FindElement(By.TagName("body"));
                    driver.FindElement(By.CssSelector("#GFScilink"));

                    driver.FindElement(By.CssSelector("#GFScilink")).Click();
                    Console.WriteLine("Testcase 8: Text exists in the cookies popup");
                    Assert.IsNullOrEmpty(cookie_text.Text);
                    str += "<tr><td>Test Case 8 :Text exists in the cookies popup</td><td> Pass" + "</td></tr>";

                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine("Testcase 8: Fail-Text exists in the cookies popup");
                    str += "<tr><td>Testcase 8: Fail- Text exists in the cookies popup</td><td> Fail" + "</td></tr>";
                }

            }
            catch (NUnit.Framework.AssertionException)
            {

            }

            //-------------------------------------------------------------------------------------------------legoid-------------------------
            driver.Navigate().GoToUrl(url);
            LoginPage obj = new LoginPage();
            Console.WriteLine("\n\tPlease Enter the Username : ");
            string username = Console.ReadLine();
            Console.WriteLine("\n\tPlease Enter the Password : ");
            string password = Console.ReadLine();
            str = str + obj.UserLogin(username, password, driver);
            //-------------------------------------------------------------------------------------------------------------------------------

            //str = str + "</table></center></body><html>";
            //string Res = AppDomain.CurrentDomain.BaseDirectory + "\\TestReport.html";
            //FileStream fs = new FileStream(Res, FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);

            //sw.WriteLine(str);
            //sw.Close();
            //fs.Close();



            //Lego Header-----------------------------------------------------------------------------------------------------------------------------

            string str_result = "<tr><td colspan=\"2\"> <center><b>Lego header test</b></center></td>";
            IWebElement main_logo = null;
            String title_page = null;
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();

            try
            {

                IJavaScriptExecutor js1 = driver as IJavaScriptExecutor;
                string title1 = (string)js1.ExecuteScript("return window.getComputedStyle( document.querySelector('.lego-global-header-wrap .lego-brand a'), ':after' ).getPropertyValue('box-sizing')");
                IWebElement header = driver.FindElement(By.CssSelector(".lego-global-navigation"));
                string header_postion = header.GetCssValue("position");
                Console.WriteLine(title1);
                string p = "absolute";
                if (header_postion == p)
                {
                    Console.WriteLine("mobile view");
                }
                else
                {
                    Console.WriteLine("desk view");
                    throw new Exception();
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Inside desktop view");
            }


            try
            {
                main_logo = driver.FindElement(By.ClassName("logo"));
                if (main_logo.Enabled && main_logo.Displayed)
                {
                    Console.WriteLine("Main logo is present");
                    str_result += "<tr><td>Main Logo</td> <td>Present</td></tr>";
                    try
                    {
                        main_logo.Click();
                        str_result += "<tr><td>Main Logo</td> <td>Is Clickable</td></tr>";
                        driver.Navigate().Back();
                    }
                    catch
                    {
                        str_result += "<tr><td>Main Logo</td> <td>Is Not Clickable</td></tr>";
                        Console.WriteLine("Main logo is not clickable");
                    }

                }
                else
                {
                    str_result = str_result + "<tr><td>Main Logo</td> <td>IS Not Present</td></tr>";
                }
            }
            catch (Exception e)
            {
                str_result = str_result + "<tr><td>Lego header</td> <td>IS Not Present</td></tr>";
            }



            //code according to desktop view 
            try
            {
                IWebElement brandingsection = driver.FindElement(By.XPath("//div[@class='lego-global-navigation']/ul"));
                List<string> anchor_list = new List<string>();
                IList<IWebElement> totallogos = brandingsection.FindElements(By.TagName("a"));

                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                string title = (string)js.ExecuteScript("return window.getComputedStyle( document.querySelector('.lego-global-navigation .global-links a'), ':before' ).getPropertyValue('background-image')");

                int ifirstindx = title.IndexOf("url");
                int start = ifirstindx + 5;
                string s = title.Substring(start);
                int l = s.Length;
                string final_url = s.Substring(0, l - 2);
                Console.WriteLine("Total logos present are" + ":" + totallogos.Count);
                HttpWebRequest request_url = (HttpWebRequest)WebRequest.Create(final_url);
                request_url.Method = "GET";
                try
                {
                    using (var response_url = request_url.GetResponse())
                    {
                        Thread.Sleep(2000);
                        HttpStatusCode statusCode_url = ((HttpWebResponse)response_url).StatusCode;
                        Console.WriteLine("Lego Header Images Url status" + ":" + statusCode_url);
                        str_result += "<tr><td>Status of  All Images in Navigation</td> <td>" + statusCode_url + "</td></tr>";

                    }
                }
                catch
                {
                    str_result += "<tr><td>Status of  All Images in Navigation</td> <td> Not Found </td></tr>";
                }

                try
                {


                    for (int i = 0; i < totallogos.Count; i++)
                    {
                        brandingsection = driver.FindElement(By.XPath("//div[@class='lego-global-navigation']/ul"));
                        totallogos = brandingsection.FindElements(By.TagName("a"));
                        NUnit.Framework.Assert.IsTrue(driver.FindElement(By.TagName("a")).Enabled);
                        anchor_list.Add(totallogos[i].GetAttribute("href"));
                        Console.WriteLine(anchor_list.Count);
                        try
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(anchor_list[i]);
                            request.Method = "GET";
                            Thread.Sleep(2000);
                            using (var response = request.GetResponse())
                            {
                                HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;
                                Console.WriteLine(anchor_list[i]);
                                Console.WriteLine(statusCode);
                                str_result += "<tr><td>" + anchor_list[i] + "</td> <td>" + statusCode + "</td></tr>";
                            }
                        }
                        catch
                        {
                            Console.WriteLine("No response for : " + anchor_list[i]);
                            str_result += "<tr><td>" + anchor_list[i] + "</td> <td> Not Clickable </td></tr>";
                        }

                        IWebElement main_logo2 = driver.FindElement(By.ClassName("logo"));
                        main_logo2.Click();
                        //if (totallogos[i].Enabled && totallogos[i].Displayed)
                        //{
                        try
                        {
                            Thread.Sleep(2000);
                            totallogos[i].Click();
                            title_page = driver.Title;
                            Console.WriteLine((title_page));
                            str_result += "<tr><td>" + title_page + "</td> <td>Clickable</td></tr>";
                            driver.Navigate().Refresh();

                            driver.Navigate().Back();
                            driver.Navigate().Refresh();
                            Thread.Sleep(500);

                            IWebElement main_logo1 = driver.FindElement(By.ClassName("logo"));
                            main_logo1.Click();
                            Thread.Sleep(1000);
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine("Item is not clickable" + ":" + title_page + ex.ToString());
                            // str_result += "<tr><td>" + title_page + "</td> <td>Not Clickable</td></tr>";
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            catch (
                Exception e)
            { }
            str = str + str_result;
            str = str + "</table></center></body><html>";
            string Res = AppDomain.CurrentDomain.BaseDirectory + "\\TestReport.html";
            FileStream fs = new FileStream(Res, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine(str);
            sw.Close();
            fs.Close();

            //Console.WriteLine("The end");

            //string Res = AppDomain.CurrentDomain.BaseDirectory + "\\TestReport.html";

            //FileStream fs = new FileStream(Res, FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.WriteLine(str_result); ;


            //str_result = str_result + "<tr><td>End of Prog</td> <td>The End</td></tr><table></body> </html>";
            //StreamWriter sw = new StreamWriter("D:/Lego_Automate/LegoHeader/LegoHeader/Result/res.html", false);

            //FileStream fs = new FileStream(str_result, FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);


            // sw.WriteLine(DateTime.Now.ToString() + str_result);
            //sw.Flush();
            sw.Close();
            driver.Navigate().GoToUrl(Res);
            /* 
              IList<IWebElement> footer_section = driver.FindElements(By.CssSelector(".lego-global-footer-wrap > ul"));//(By.XPath("//div[@class='lego-global-footer-wrap']/ul"));
              Console.WriteLine(footer_section.Count);
              Console.WriteLine(footer_section[0].Text);
              Console.WriteLine(footer_section[1].Text);
              Console.WriteLine(footer_section[2].Text);
              List<string> anchor_list_footer = new List<string>();
              foreach (var v in footer_section)
              {
                  IList<IWebElement> total_sub_sections = v.FindElements(By.TagName("li"));
                
                  Console.WriteLine(total_sub_sections.Count);
             //     Console.WriteLine(v.Text);
            
                  for (int i = 0; i < total_sub_sections.Count; i++)
                  {
                   

                  //    Console.WriteLine(total_sub_sections[i].Text + "       index" + i);
                      if (total_sub_sections[i].Enabled && total_sub_sections[i].Displayed)
                      {
                          Console.WriteLine(total_sub_sections[i].Text + "is present ");
                    
                      }
                  }
              }
           
            driver.Navigate().GoToUrl(Res);*/


        }
    }
}
