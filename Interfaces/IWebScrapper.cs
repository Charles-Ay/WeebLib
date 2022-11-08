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
        /// <param name="dir">where to output results</param>
        /// <returns>number of sites retrived</returns>
        public abstract int Scrape(List<T> data, string dir);

        /// <summary>
        /// Pings the website and gets the the raw html from the source
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        protected HtmlAgilityPack.HtmlDocument GetSite(string url)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

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
