using HtmlAgilityPack;
using PdfSharpCore.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.NovelExceptions;
using static WeebLib.Utility.WeebLibUtil;

namespace WeebLib.Novel.Retrieval
{
    //TODO: CREATE FULL SEARCH(ALL SOURCES)
    //TODO: ADD FREEWEBNOVEL IMAGES
    //TODO: SET FREE AS DEFAULT SOURCE
    public class NovelSearcher : ISearcher
    {
        /// <summary>
        /// number of results fetched
        /// </summary>
        int numberOfResults;
        public override bool Search(int start, string title, string source = "")
        {
            switch (source)
            {
                case "freewebnovel":
                    return SearchFreeWebNovel(start, title);
                case "fullnovel":
                    return SearchFullNovel(start, title);
                case "noveltrench":
                    return SearchNovelTrench(start, title);
                case "":
                    return SearchAll(start, title);
                default:
                    return false;
            }
        }

        private bool SearchAll(int start, string title)
        {
            return SearchFreeWebNovel(start, title) && SearchFullNovel(start, title);
        }

        /// <summary>
        /// Searches for a novel on FreeWebNovel
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchFreeWebNovel(int startChapter, string novelname)
        {
            numberOfResults = 0;
            var url = "https://freewebnovel.com/search/";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            httpRequest.Method = "POST";
            //mimck chrome
            httpRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            httpRequest.ContentType = "application/x-www-form-urlencoded";

            var data = $"searchkey={novelname}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            HtmlDocument html = new HtmlDocument();
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
                List<string> image = new List<string>();
                List<string> sources = new List<string>();
                List<int> latest = new List<int>();

                //get total amount of results
                try
                {
                    numberOfResults = int.Parse(html.DocumentNode.SelectSingleNode("//em[@class='num']").InnerText);
                }
                catch (NullReferenceException)
                {
                    throw new SearchException("Search query must be at least 3 characters long");
                }

                var latestChapters = html.DocumentNode.SelectNodes("//span[@class='s1']");
                foreach(var chap in latestChapters)
                {
                    string chapTxt = Regex.Match(chap.InnerText, @"\d+").Value;
                    latest.Add(int.Parse(chapTxt));
                }
                
                foreach (HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='pic']"))
                {
                    var childNode = node.ChildNodes[1];
                    var imgLink = childNode.SelectSingleNode("img").Attributes[0].Value;
                    image.Add(imgLink);
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
                                throw new LinkException("Error extracting link from FreeWebNovel");
                            }
                        }
                        string output = NovelUtil.sourceToStringWithCasing(NovelUtil.NovelSources.FreeWebNovel);
                        sources.Add(output);
                    }
                }
                for (int i = 0; i < numberOfResults; ++i)
                {
                    SearchType se = new SearchType(name[i], link[i], latest[i], sources[i], image[i]);
                    bool had = results.Any(s => se.name == s.name);
                    if(!had) results.Add(se);
                }
            }
            else
            {
                //ask for new input
            }
            return true;
        }

        /// <summary>
        /// Searches for a novel on FullNove
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchFullNovel(int startChapter, string novelname)
        {
            numberOfResults = 0;
            string srchqry = novelname.Replace(" ", "+");
            string url = $"https://full-novel.com/search?keyword={srchqry}";

            var html = Request(url);
            if (html.DocumentNode != null)
            {
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> image = new List<string>();
                List<string> sources = new List<string>();
                List<int> latest = new List<int>();
                string nextPage = "";

                //Get next page
                if (html.DocumentNode.SelectSingleNode("//li[@class='next ']")!= null)
                {
                    var nextPageNode = html.DocumentNode.SelectSingleNode("//li[@class='next ']").SelectSingleNode("a");
                    nextPage = nextPageNode.GetAttributeValue("href", string.Empty);
                }
                else
                {
                    nextPage = "WILL BE DEAD";
                }


                while (nextPage != "DEAD")
                {
                    var latestChapters = html.DocumentNode.SelectNodes("//span[@class='chr-text']");
                    foreach (var chap in latestChapters)
                    {
                        string chapTxt = Regex.Match(chap.InnerText, @"\d+").Value;
                        latest.Add(int.Parse(chapTxt));
                    }

                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='col-xs-3']")) {
                        var childNode = node.ChildNodes[1];
                        var imgLink = childNode.SelectSingleNode("img").Attributes[0].Value;
                        image.Add(imgLink);
                    }
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//h3[@class='novel-title']"))
                    {
                        name.Add(node.InnerText);
                        var newNodes = node.SelectNodes("a");
                        foreach (var innerNode in newNodes)
                        {
                            string pageLink = innerNode.GetAttributeValue("href", string.Empty);

                            if (pageLink != string.Empty)
                            {
                                pageLink = pageLink.Replace("/nb/", "/ajax/chapter-archive?novelId=");
                                link.Add(pageLink);
                            }
                            else
                            {
                                throw new LinkException("Error extracting link from FullNovel");
                            }
                        }
                        string output = NovelUtil.sourceToStringWithCasing(NovelUtil.NovelSources.FullNovel);
                        sources.Add(output);
                        ++numberOfResults;
                    }

                    HtmlNode tmpNode = null;

                    try
                    {
                        tmpNode = html.DocumentNode.SelectSingleNode("//li[@class='next ']");
                    }
                    catch (Exception)
                    {
                        nextPage = "DEAD";
                    }

                    if (nextPage == "WILL BE DEAD")
                    {
                        nextPage = "DEAD";
                    }
                    else if (tmpNode == null)
                    {
                        nextPage = "DEAD";
                    }
                    else
                    {
                        nextPage = tmpNode.SelectSingleNode("a").GetAttributeValue("href", string.Empty);
                        nextPage = nextPage.Replace("&#x3D;", "=");
                        nextPage = nextPage.Replace("&amp;", "&");
                        html = Request(nextPage);
                    }
                    for (int i = 0; i < numberOfResults; ++i)
                    {
                        SearchType se = new SearchType(name[i], link[i], latest[i], sources[i], image[i]);
                        bool had = results.Any(s => se.name == s.name);
                        if (!had) results.Add(se);
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Searches for a novel on NovelTrench
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchNovelTrench(int startChapter, string novelname)
        {
            string srchqry = novelname.Replace(" ", "+");
            var url = $"https://noveltrench.com/?s={srchqry}&post_type=wp-manga&op=&author=&artist=&release=&adult=";

            var html = Request(url);

            if (html.DocumentNode != null)
            {
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> sources = new List<string>();

                //get total amount of results
                string tmpString = html.DocumentNode.SelectSingleNode("//h1[@class='h4']").InnerText;
                tmpString = Regex.Match(tmpString, @"\d+").Value;
                numberOfResults = int.Parse(tmpString);



                //get next page
                string nextPage = html.DocumentNode.SelectSingleNode("//div[@class='nav-previous float-left']").InnerText;

                while (nextPage != "DEAD")
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//h3[@class='h4']"))
                    {
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
                                throw new LinkException("Error extracting link from NovelTrench");
                            }
                        }
                        string output = NovelUtil.sourceToStringWithCasing(NovelUtil.NovelSources.NovelTrench);
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
                        html = Request(nextPage);
                        tmpNode = tmpNode.SelectSingleNode("a");
                    }
                    else nextPage = "DEAD";
                }
                for (int i = 0; i < numberOfResults; ++i)
                {
                    //results.Add(new SearchType(name[i], link[i], sources[i]));
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        internal List<NovelData> QueryFullNovelAndGetChapters(ref NovelData novel, int first)
        {
            List<NovelData> novels = new List<NovelData>();

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(novel.initalLink);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            httpRequest.Method = "GET";
            //mimick chrome
            httpRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string results = string.Empty;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                results = streamReader.ReadToEnd();
            }
            var html = new HtmlDocument();
            html.LoadHtml(results);


            foreach (HtmlNode node in html.DocumentNode.SelectNodes("//span[@class='nchr-text']"))
            {
                var parent = node.ParentNode;
                var link = parent.GetAttributeValue("href", string.Empty);
                int num = int.Parse(Regex.Match(node.InnerHtml, @"\d+").Value);

                if (novels.Count == 0 || link != novels[num - 2].initalLink ||  num <= novel.latestChapter)
                {
                    if (num >= first && num <= novel.totalChapters)
                    {
                        novels.Add(new NovelData(novel.name, num, novel.latestChapter,link, novel.source));
                    }
                }
                else break;
            }
            return novels;
        }

        /// <summary>
        /// Get all novel chapters
        /// </summary>
        /// <param name="novel"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        internal List<NovelData> GetNovelChapters(ref NovelData novel, int first)
        {
            List<NovelData> novels = new List<NovelData>();
            for (int i = first; i <= novel.totalChapters; ++i)
            {
                //get current chapter link
                //replace numbers with more than one digit with ""
                string curchapter = Regex.Replace(novel.initalLink, "[0-9]{2,}", $"");
                if(novels.Count <= 1 || curchapter != novels[i -2].initalLink || i <= novel.latestChapter)
                {
                    //replace remaing last digit
                    //TODO: CHECK FOR POSSIBLE CHAPTER INDEX -1 LOGIC ERROR
                    curchapter = Regex.Replace(curchapter, "[0-9]", $"{i}");

                    novels.Add(new NovelData(novel.name, i, novel.latestChapter, curchapter, novel.source));
                }
                else break;
            }
            return novels;
        }
    }
}