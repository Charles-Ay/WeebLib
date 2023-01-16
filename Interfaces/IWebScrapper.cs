using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Interfaces
{
    public abstract class IWebScrapper<T>
    {
        /// <summary>
        /// Scrape the data from the source
        /// </summary>
        /// <param name="data">Pages to scrape</param>
        /// <param name="dir">Where to output results</param>
        /// <param name="outputToFile">Output results to file</param>
        /// <returns>text if output to file is false</returns>
        public abstract string Scrape(List<T> data, string dir, bool outputToFile = true);

        /// <summary>
        /// Get the html from the page
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected HtmlAgilityPack.HtmlDocument GetSite(string url)
        {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete

            //Make the request look like a browser
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
