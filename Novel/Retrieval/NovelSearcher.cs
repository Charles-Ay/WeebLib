using HtmlAgilityPack;
using PdfSharpCore.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.NovelExceptions;
using static WeebLib.Utility.WeebLibUtil;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WeebLib.Novel.Retrieval
{
    public class NovelSearcher : ISearcher
    {
        /// <summary>
        /// number of results fetched
        /// </summary>
        int numberOfResults;
        public override bool Search(int start, string title, string source = "")
        {
            Clear();
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
                    return SearchAll(start, title);
            }
        }

        /// <summary>
        /// Searches all sources for a novel
        /// </summary>
        /// <param name="start"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool SearchAll(int start, string title)
        {
            return SearchFreeWebNovel(start, title);
            //TODO: Uncomment when fullnovel is fixed
            //bool searchFull = SearchFullNovel(start, title);
            //if (searchFull || searchFree) return true;
            //return false;
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

            HtmlDocument html = new HtmlDocument();
            html.OptionFixNestedTags = true;

            // Create a cookie container to maintain cookies between requests
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            using (var client = new HttpClient(handler))
            {
                // Step 1: Visit the search page first to get cookies/cf_clearance
                var initialRequest = new HttpRequestMessage(HttpMethod.Get, "https://freewebnovel.com/search");
                initialRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36");
                var initialResponse = client.SendAsync(initialRequest).GetAwaiter().GetResult();
                initialResponse.EnsureSuccessStatusCode();

                // Now make the actual search request with cookies
                var searchRequest = new HttpRequestMessage(HttpMethod.Post, url);

                // Add all the headers the browser sends
                searchRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36");
                searchRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                searchRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                searchRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                searchRequest.Headers.Add("Cache-Control", "max-age=0");
                searchRequest.Headers.Add("Origin", "https://freewebnovel.com");
                searchRequest.Headers.Add("Referer", "https://freewebnovel.com/search");
                searchRequest.Headers.Add("Sec-Fetch-Dest", "document");
                searchRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
                searchRequest.Headers.Add("Sec-Fetch-Site", "same-origin");
                searchRequest.Headers.Add("Sec-Fetch-User", "?1");
                searchRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
                searchRequest.Headers.Add("sec-ch-ua", "\"Not:A-Brand\";v=\"99\", \"Google Chrome\";v=\"145\", \"Chromium\";v=\"145\"");
                searchRequest.Headers.Add("sec-ch-ua-mobile", "?0");
                searchRequest.Headers.Add("sec-ch-ua-platform", "\"Windows\"");

                var payload = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("searchkey", novelname)
                });
                searchRequest.Content = payload;

                var response = client.SendAsync(searchRequest).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    html.LoadHtml(body);
                }
            }

            if (ShouldRetryWithSelenium(html, novelname))
            {
                html = RequestFreeWebNovelSearchWithSelenium(url, novelname);
            }

            if (html.DocumentNode != null)
            {
                //Get total amount of results (fallback to actual result count)
                try
                {
                    var numNode = html.DocumentNode.SelectSingleNode("//em[@class='num']");
                    if (numNode != null)
                    {
                        string numTxt = Regex.Match(numNode.InnerText, @"\d+").Value;
                        if (!string.IsNullOrWhiteSpace(numTxt)) numberOfResults = int.Parse(numTxt);
                    }
                }
                catch (Exception)
                {
                    throw new SearchException("Search query must be at least 3 characters long");
                }

                var resultNodes = html.DocumentNode.SelectNodes("//div[contains(@class,'ul-list1')]//div[contains(@class,'li-row')]");
                if (resultNodes == null) return true;

                if (numberOfResults == 0) numberOfResults = resultNodes.Count;

                foreach (var node in resultNodes)
                {
                    var titleNode = node.SelectSingleNode(".//h3[contains(@class,'tit')]/a");
                    if (titleNode == null) continue;

                    string novelName = HtmlEntity.DeEntitize(titleNode.InnerText).Trim();
                    if (string.IsNullOrWhiteSpace(novelName)) continue;

                    string novelHref = titleNode.GetAttributeValue("href", string.Empty);
                    if (string.IsNullOrWhiteSpace(novelHref)) continue;

                    string novelUrl = novelHref.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? novelHref
                        : $"https://freewebnovel.com{(novelHref.StartsWith("/") ? "" : "/")}{novelHref}";

                    string startLink = $"{novelUrl}/chapter-{startChapter}";

                    var latestNode = node.SelectSingleNode(".//a[contains(@class,'chapter')]//span[contains(@class,'s1')]")
                        ?? node.SelectSingleNode(".//span[contains(@class,'s1')]");
                    int latestChapter = 0;
                    if (latestNode != null)
                    {
                        string chapTxt = Regex.Match(latestNode.InnerText, @"\d+").Value;
                        if (!string.IsNullOrWhiteSpace(chapTxt)) latestChapter = int.Parse(chapTxt);
                    }

                    var imgNode = node.SelectSingleNode(".//div[contains(@class,'pic')]//img");
                    string imgLink = imgNode?.GetAttributeValue("src", string.Empty) ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(imgLink) && !imgLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        imgLink = $"https://freewebnovel.com{(imgLink.StartsWith("/") ? "" : "/")}{imgLink}";
                    }

                    string output = NovelUtil.sourceToStringWithCasing(NovelUtil.NovelSources.FreeWebNovel);

                    SearchType se = new SearchType(novelName, startLink, latestChapter, output, imgLink);
                    bool had = results.Any(s => se.name == s.name);
                    if (!had) results.Add(se);
                }
            }
            else
            {
                return true;
            }
            return true;
        }

        private bool ShouldRetryWithSelenium(HtmlDocument html, string novelname)
        {
            if (html?.DocumentNode == null) return true;

            var titleNode = html.DocumentNode.SelectSingleNode("//title");
            if (titleNode != null)
            {
                var title = HtmlEntity.DeEntitize(titleNode.InnerText).Trim();
                if (title.Contains("You searched for", StringComparison.OrdinalIgnoreCase) &&
                    !title.Contains(novelname, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            var numNode = html.DocumentNode.SelectSingleNode("//em[@class='num']");
            if (numNode != null)
            {
                string numTxt = Regex.Match(numNode.InnerText, @"\d+").Value;
                if (numTxt == "0") return true;
            }

            var resultNodes = html.DocumentNode.SelectNodes("//div[contains(@class,'ul-list1')]//div[contains(@class,'li-row')]");
            return resultNodes == null || resultNodes.Count == 0;
        }

        private HtmlDocument RequestFreeWebNovelSearchWithSelenium(string url, string novelname)
        {
            var html = new HtmlDocument();
            html.OptionFixNestedTags = true;

            var options = new ChromeOptions();
            options.AddArgument("headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");

            using (var driver = new ChromeDriver(options))
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                driver.Navigate().GoToUrl(url);

                IWebElement input = null;
                var inputs = driver.FindElements(By.Name("searchkey"));
                foreach (var el in inputs)
                {
                    if (el.Displayed && el.Enabled)
                    {
                        input = el;
                        break;
                    }
                }
                if (input == null)
                {
                    input = driver.FindElement(By.Id("searchkey"));
                }

                var js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].value = arguments[1];", input, novelname);

                // Submit the desktop or mobile search form via JS to avoid non-interactable errors
                js.ExecuteScript(@"
                    var f = document.getElementById('form') || document.getElementById('m_form');
                    if (f) f.submit();
                ");

                System.Threading.Thread.Sleep(1000);

                var pageSource = driver.PageSource;
                html.LoadHtml(pageSource);
            }

            return html;
        }

        //TODO: Improve search
        /// <summary>
        /// Searches for a novel on FullNovel. This search and is not as accurate as FreeWebNovel
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchFullNovel(int startChapter, string novelname)
        {
            numberOfResults = 0;
            string srchqry = novelname.Replace(" ", "+");
            //base url
            string url = $"https://full-novel.com/search?keyword={srchqry}";

            var html = Request(url);
            if (html.DocumentNode != null)
            {
                //Lists to hold the search query data
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> image = new List<string>();
                List<string> sources = new List<string>();
                List<int> latest = new List<int>();
                string nextPage = "";

                //Get next page
                if (html.DocumentNode.SelectSingleNode("//li[@class='next ']")!= null)
                {
                    //Get the link to the next page
                    var nextPageNode = html.DocumentNode.SelectSingleNode("//li[@class='next ']").SelectSingleNode("a");
                    nextPage = nextPageNode.GetAttributeValue("href", string.Empty);
                }
                else
                {
                    nextPage = "WILL BE DEAD";
                }

                while (nextPage != "DEAD")
                {
                    //Get the latest chapter of each novel
                    var latestChapters = html.DocumentNode.SelectNodes("//span[@class='chr-text']");
                    try
                    {
                        foreach (var chap in latestChapters)
                        {
                            string chapTxt = Regex.Match(chap.InnerText, @"\d+").Value;
                            if (chapTxt != "")
                            {
                                latest.Add(int.Parse(chapTxt));
                            }
                            //If the inner text doesn't contain a number, it instead uses the outer HTML of the parent node to extract the number.
                            else
                            {
                                chapTxt = Regex.Match(chap.ParentNode.OuterHtml, @"\d+").Value;
                                latest.Add(int.Parse(chapTxt));
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {
                        return false;
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
        [Obsolete("NovelTrench is no longer supported")]
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

        //TODO: Fix this when fixing fullnovel
        /// <summary>
        /// Searches for a novel on NovelFull
        /// </summary>
        /// <param name="novel"></param>
        /// <param name="first">first chapter to search for</param>
        /// <returns></returns>
        internal List<NovelData> QueryFullNovelAndGetChapters(ref NovelData novel, int first)
        {
            List<NovelData> novels = new List<NovelData>();

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var httpRequest = (HttpWebRequest)WebRequest.Create(novel.initalLink);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            httpRequest.Method = "GET";
            //mimick chrome


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string results = string.Empty;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                results = streamReader.ReadToEnd();
            }
            var html = new HtmlDocument();
            html.LoadHtml(results);

            int nextNum = 0;
            //used to keep track of the number of missing chapters in the novel.
            int gap = 2;
            foreach (HtmlNode node in html.DocumentNode.SelectNodes("//span[@class='nchr-text']"))
            {
                var parent = node.ParentNode;
                var link = parent.GetAttributeValue("href", string.Empty);
                int num = int.Parse(Regex.Match(node.InnerHtml, @"\d+").Value);

                //Hack: sometimes a chapter is missing
                //If the next chapter number is greater than the previous chapter number, it means that there is a missing chapter.
                if (nextNum > 0 && num > nextNum)
                {
                    //Just use the text from the previous chapter
                    while(nextNum != num)
                    {
                        novels.Add(new NovelData(novel.name, nextNum, novel.latestChapter, link, novel.source));
                        ++nextNum;
                    }
                    ++gap;
                }

                //checks if the novels list is empty or if the link of the current chapter is not the same
                //as the previous chapter's link in the novels list by using the gap variable
                if (novels.Count == 0 ||  num <= novel.latestChapter)
                {
                    if (num >= first && num <= novel.totalChapters)
                    {
                        novels.Add(new NovelData(novel.name, num, novel.latestChapter,link, novel.source));
                    }
                }
                if(num >= novel.latestChapter) break;
                nextNum = num + 1;
            }
            return novels;
        }

        /// <summary>
        /// Get all novel chapters
        /// </summary>
        /// <param name="novel"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        internal List<NovelData> QueryFreeWebNovelAndGetChapters(ref NovelData novel, int first)
        {
            List<NovelData> novels = new List<NovelData>();
            //HACK: FOR WHEN SOMEONE WANTS TO DOWNLOAD A SINGLE CHAPTER OTHER THAN FIRST
            int total = novel.totalChapters;
            if (first > novel.totalChapters) total += first;
            
            for (int i = first; i <= total; ++i)
            {
                //get current chapter link
                //replace numbers with more than one digit with ""
                string curchapter = Regex.Replace(novel.initalLink, "[0-9]{2,}", $"");
                if(novels.Count <= 1 || curchapter != novels[i -2].initalLink || i <= novel.latestChapter)
                {
                    //replace remaing last digit
                    curchapter = Regex.Replace(curchapter, "[0-9]", $"{i}");

                    novels.Add(new NovelData(novel.name, i, novel.latestChapter, curchapter, novel.source));
                }
                else break;
            }
            return novels;
        }
    }
}
