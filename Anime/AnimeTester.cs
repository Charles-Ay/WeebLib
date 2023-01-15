using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V106.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Anime
{
    public class AnimeTester
    {
        string link;

        public AnimeTester(string link)
        {
            this.link = link;
            dostuff();
        }
        
        private void dostuff()
        {
            string json = "";
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Only a test!");
                wc.Headers.Add("User-Agent: Other");
                json = wc.DownloadString(link);
            }
            Root searchStuff = JsonConvert.DeserializeObject<Root>(json);

            string qr = $"https://allanime.site/anime/{searchStuff.data.shows.edges[0]._id}";
            //string qr2= $"https://allanime.site/watch/{searchStuff.data.shows.edges[0]._id}";
            string actualLink = "";
            {
                var html2 = GetSite(qr);
                var nodes = html2.DocumentNode.SelectNodes("//link[@data-n-head='ssr']");
                HtmlNode f;

                foreach(var node in nodes)
                {
                    if (node.HasAttributes)
                    {
                        foreach(var attr in node.Attributes)
                        {
                            if (attr.Value == "canonical")
                            {
                                actualLink = node.Attributes["href"].Value;
                                goto MAIN;
                            }
                        }
                    }
                }

            }
        MAIN:
            string tmp = actualLink.Replace("https://allanime.site/anime/", "https://allanime.site/watch/");
            string qr2 = tmp + "/episode-1-sub";
            var html = GetSite(qr2);
            var doc = html.DocumentNode.InnerHtml;
            var ind = doc.IndexOf("downloadUrl") + "downloadUrl".Length + 1;
            string data = "";
            string sub = doc.Substring(ind);
            foreach (char c in sub)
            {
                if (c != '"') data += c;
                else
                {
                    if (data.Length > 0) break;
                }
            }
            data = data.Replace(@"\\", @"\");
            data = data.Replace("\\u002F", "/");
            var dwn = MockWithChrome(data, searchStuff.data.shows.edges[0]._id);


        }

        private HtmlDocument MockWithChrome(string url, string fname)
        {

            var downloadsPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\" + fname+"_1_sub.mp4";
            for (var i = 0; i < 30; i++)
            {
                if (File.Exists(downloadsPath)) { break; }
                //Thread.Sleep(1000);
            }



            //create object of chrome options
            ChromeOptions options = new ChromeOptions();

            //add the headless argument
            options.AddArgument("headless");
            var driver = new ChromeDriver(options);
            //navigate to the url
            driver.Navigate().GoToUrl(url);

            driver.FindElement(By.XPath("//div[@class='download-wrapper']")).Click();

            var length = new FileInfo(downloadsPath).Length;
            for (var i = 0; i < 30; i++)
            {
                Thread.Sleep(1000);
                var newLength = new FileInfo(downloadsPath).Length;
                if (newLength == length && length != 0) { break; }
                length = newLength;
            }
            //get the page source
            var pageSource = driver.PageSource;

            //create a new HtmlDocument
            var doc = new HtmlDocument();

            //load the page source into the HtmlDocument
            doc.LoadHtml(pageSource);

            //return the HtmlDocument
            return doc;



            //using (var driver = new ChromeDriver(options))
            //{

            //}
            //    var driver = new ChromeDriver(options);
            //    driver.Navigate().GoToUrl(url);
            //    var doc = new HtmlDocument();
            //    doc.LoadHtml(driver.PageSource);
            //    driver.Quit();
            //    return doc;
        }

        protected HtmlAgilityPack.HtmlDocument GetSite(string url)
        {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            httpRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            HtmlAgilityPack.HtmlDocument html = new HtmlDocument();
            html.OptionFixNestedTags = true;

            var response = httpRequest.GetResponse();
            if (((HttpWebResponse)response).StatusDescription == "OK")
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    html.LoadHtml(reader.ReadToEnd());
                }
            }
            return html;
        }
    }
}
