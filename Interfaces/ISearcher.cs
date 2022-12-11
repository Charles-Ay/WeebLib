using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Novel.NovelExceptions;
using static WeebLib.Utility.WeebLibUtil;

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

        protected HtmlDocument Request(string url)
        {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            httpRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            HtmlAgilityPack.HtmlDocument html = new HtmlDocument();
            html.OptionFixNestedTags = true;

            var response = httpRequest.GetResponse();
            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
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

        public List<SearchType> getResults()
        {
            if (results.Count > 0) return results;
            else throw new SearchException("No results found");
        }
    }
}
