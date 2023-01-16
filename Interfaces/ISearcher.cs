using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Novel.NovelExceptions;
using static WeebLib.Utility.WeebLibUtil;
using OpenQA.Selenium.Chrome;


namespace WeebLib.Interfaces
{
    public abstract class ISearcher
    {
        /// <summary>
        /// The website links fetched
        /// </summary>
        protected List<SearchType> results = new List<SearchType>();
        
        /// <summary>
        /// Searches for a item
        /// </summary>
        /// <param name="start">The first chapter or episode to check</param>
        /// <param name="title"></param>
        /// <param name="source"></param>
        /// <returns>True if the object was found</returns>
        public abstract bool Search(int start, string title, string source = "");

        /// <summary>
        /// Mock a chrome browser with selenium
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Obsolete("MockWithChrome should typically not be used due to slow speed.")]
        private HtmlDocument MockWithChrome(string url) {
            //create object of chrome options
            ChromeOptions options = new ChromeOptions();

            //add the headless argument
            options.AddArgument("headless");

            using (var driver = new ChromeDriver(options))
            {
                //navigate to the url
                driver.Navigate().GoToUrl(url);

                //get the page source
                var pageSource = driver.PageSource;

                //create a new HtmlDocument
                var doc = new HtmlDocument();

                //load the page source into the HtmlDocument
                doc.LoadHtml(pageSource);

                //return the HtmlDocument
                return doc;
            }
        }

        /// <summary>
        /// Gets the html document from the url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mockBrowser"></param>
        /// <returns></returns>
        protected HtmlDocument Request(string url, bool mockBrowser = false)
        {
            if (mockBrowser) return MockWithChrome(url);
            
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete

            //Make the request look like a browser
            httpRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            
            HtmlAgilityPack.HtmlDocument html = new HtmlDocument();
            html.OptionFixNestedTags = true;

            WebResponse response = httpRequest.GetResponse();

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream, Encoding.Default);
                    // Read the content.
                    html.LoadHtml(reader.ReadToEnd());
                }
            }
            return html;
        }

        /// <summary>
        /// Get the search results
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SearchException"></exception>
        public List<SearchType> Results()
        {
            if (results.Count > 0) return results;
            else throw new SearchException("No results found");
        }

        /// <summary>
        /// Clear the search results
        /// </summary>
        protected void Clear()
        {
            results.Clear();
        }
    }
}
