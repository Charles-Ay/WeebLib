using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeebLib.Interfaces;

namespace WeebLib.Novel.Parser
{
    internal class NovelSearch : ISearcher
    {
        /// <summary>
        /// The website links fetched
        /// </summary>
        public List<SearchType> results = new List<SearchType>();
        /// <summary>
        /// number of results fetched
        /// </summary>
        int numberOfResults;
        internal override bool Search(int start, string title, string source = "")
        {
            if (source == "freewebnovel")
            {
                return SearchFreeWebNovel(ref start, ref title);
            }
            else if (source == "noveltrench")
            {
                return SearchNovelTrench(ref start, ref title);
            }
            else
            {
                bool result = SearchFreeWebNovel(ref start, ref title);
                if (result == false) SearchNovelTrench(ref start, ref title);
                return result;
            }
        }


        /// <summary>
        /// Searches for a novel on FreeWebNovel
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns></returns>
        private bool SearchFreeWebNovel(ref int startChapter, ref string novelname)
        {
            var url = "https://freewebnovel.com/search/";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/x-www-form-urlencoded";

            var data = $"searchkey={novelname}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

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

            if (html.DocumentNode != null)
            {
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> sources = new List<string>();

                //get total amount of results
                try
                {
                    numberOfResults = Int32.Parse(html.DocumentNode.SelectSingleNode("//em[@class='num']").InnerText);
                }
                catch (NullReferenceException e)
                {
                    Logger.writeToLog("LOG TEST FOR NULL REF");
                }

                foreach (HtmlNode node in html.DocumentNode.SelectNodes("//h3[@class='tit']"))
                {
                    if (node.InnerText != " Genres" && node.InnerText != " Search Tips")
                    {
                        name.Add(node.InnerText);
                        var newNodes = node.SelectNodes("a");
                        foreach (var innerNode in newNodes)
                        {
                            string tmp = innerNode.GetAttributeValue("href", string.Empty);
                            if (tmp != string.Empty)
                            {
                                tmp = tmp.Replace(".html", "");
                                tmp = tmp.Remove(0, 1);
                                tmp = $"https://freewebnovel.com/{tmp}/chapter-{startChapter}.html";
                                link.Add(tmp);
                            }
                            else
                            {
                                //throw some error or return false
                            }
                        }
                        string output;
                        NovelUtil.htmlSupportedWebsites.TryGetValue("freewebnovel", out output);
                        sources.Add(output);
                    }
                }

                if (name.Count != numberOfResults || link.Count != numberOfResults || sources.Count != numberOfResults)
                {
                    //throw error
                }
                for (int i = 0; i < numberOfResults; ++i)
                {
                    results.Add(new SearchType(name[i], link[i], sources[i]));
                }
            }
            else
            {
                //ask for new input
            }
            return true;
        }

        /// <summary>
        /// Searches for a novel on NovelTrench
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private bool SearchNovelTrench(ref int startChapter, ref string novelname)
        {
            string srchqry = novelname.Replace(" ", "+");
            var url = $"https://noveltrench.com/?s={srchqry}&post_type=wp-manga&op=&author=&artist=&release=&adult=";

            var html = Request(ref url);

            if (html.DocumentNode != null)
            {
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> sources = new List<string>();

                //get total amount of results

                string tmpString = html.DocumentNode.SelectSingleNode("//h1[@class='h4']").InnerText;
                tmpString = Regex.Match(tmpString, @"\d+").Value;
                numberOfResults = Int32.Parse(tmpString);



                //get next page
                string nextPage = html.DocumentNode.SelectSingleNode("//div[@class='nav-previous float-left']").InnerText;

                while (nextPage != "DEAD")
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//h3[@class='h4']"))
                    {
                        //Console.WriteLine(node.InnerText);
                        name.Add(node.InnerText);
                        var newNodes = node.SelectNodes("a");
                        foreach (var innerNode in newNodes)
                        {
                            string tmp = innerNode.GetAttributeValue("href", string.Empty);
                            if (tmp != string.Empty)
                            {
                                tmp += $"chapter-{startChapter}";
                                link.Add(tmp);
                            }
                            else
                            {
                                //throw some error or return false
                            }
                        }
                        string output;
                        NovelUtil.htmlSupportedWebsites.TryGetValue("noveltrench", out output);
                        sources.Add(output);
                    }
                    var tmpNode = html.DocumentNode.SelectSingleNode("//div[@class='nav-previous float-left']");

                    if (tmpNode != null)
                    {
                        tmpNode = tmpNode.SelectSingleNode("a");
                        nextPage = tmpNode.GetAttributeValue("href", string.Empty);

                        //links for the page are weird, need to do some magic
                        int index = nextPage.IndexOf("wp-manga") + "wp-manga".Length;
                        if (index >= 0) nextPage = nextPage.Substring(0, index);
                        nextPage = nextPage.Replace("&#038;", "&");
                        html = Request(ref nextPage);
                        tmpNode = tmpNode.SelectSingleNode("a");
                    }
                    else nextPage = "DEAD";
                }

                if (name.Count != numberOfResults || link.Count != numberOfResults || sources.Count != numberOfResults)
                {
                    //throw error
                    throw new InvalidOperationException();
                }
                for (int i = 0; i < numberOfResults; ++i)
                {
                    results.Add(new SearchType(name[i], link[i], sources[i]));
                }
            }
            else
            {
                //ask for new input
            }
            return true;
        }


        /// <summary>
        /// Get all novel chapters
        /// </summary>
        /// <param name="novel"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        public List<NovelData> GetNovelChapters(ref NovelData novel, ref int first)
        {
            List<NovelData> novels = new List<NovelData>();
            for (int i = first; i <= novel.totalChapters; ++i)
            {
                //get current chapter link
                //replace numbers with more than one digit with ""
                string curchapter = Regex.Replace(novel.initalLink, "[0-9]{2,}", $"");
                //replace remaing last digit
                curchapter = Regex.Replace(curchapter, "[0-9]", $"{i}");

                novels.Add(new NovelData(novel.name, i, curchapter, novel.source));
            }
            return novels;
        }

        /// <summary>
        /// Hold the data for the search results
        /// </summary>
        public class SearchType
        {
            public SearchType(string name, string link, string source)
            {
                this.name = name;
                this.link = link;
                this.source = source;
            }
            public string name { get; private set; }
            public string link { get; private set; }
            public string source { get; private set; }
        }
    }
}