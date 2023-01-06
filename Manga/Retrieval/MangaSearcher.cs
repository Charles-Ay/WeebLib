using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.NovelExceptions;
using WeebLib.Novel;
using WeebLib.Manga.MangaExceptions;
using System.Text.RegularExpressions;
using static WeebLib.Utility.WeebLibUtil;
using System.Xml.Linq;

namespace WeebLib.Manga.Retrieval
{
    public class MangaSearcher : ISearcher
    {
        int numberOfResults;
        public override bool Search(int start, string title, string source = "")
        {
            switch (source)
            {
                case "mangasee":
                    return SearchMangaSee(start, title);
                default:
                    return false;
            }
        }

        ///// <summary>
        ///// Searches for a manga on FanFox
        ///// </summary>
        ///// <param name="startChapter"></param>
        ///// <param name="novelname"></param>
        ///// <returns>True if content was retrived successfully</returns>
        //private bool SearchMangaSee(int startChapter, string novelname)
        //{
        //    string srchqry = novelname.Replace(" ", "+");
        //    string url = $"https://fanfox.net/search?title={srchqry}";
        //    int pageNum= 1;

        //    var html = Request(url);
        //    if (html.DocumentNode != null)
        //    {
        //        List<string> name = new List<string>();
        //        List<string> link = new List<string>();
        //        List<string> sources = new List<string>();
        //        List<int> latest = new List<int>();

        //        string nextPage = $"https://fanfox.net/search?page={pageNum}&title={srchqry}#searchlt";

        //        while (nextPage != "DEAD")
        //        {
        //            try
        //            {
        //                int count = 0;
        //                var outerNode = html.DocumentNode.SelectSingleNode("//ul[@class='manga-list-4-list line']");

        //                foreach (HtmlNode node in outerNode.SelectNodes(".//li"))
        //                {
        //                    var a = node.SelectSingleNode(".//a");
        //                    var href = a.Attributes["href"].Value;
        //                    href = $"https://fanfox.net{href}";
        //                    var title = a.Attributes["title"].Value;
        //                    var latestChapterNode = node.SelectNodes("//p[@class='manga-list-4-item-tip']")[1];
        //                    var latestChapter = latestChapterNode.InnerText.Substring(latestChapterNode.InnerText.LastIndexOf("Ch.") + 3);
        //                    latestChapter = Regex.Match(latestChapter, @"\d+").Value;
        //                    name.Add(title);
        //                    link.Add(href);
        //                    sources.Add(MangaUtil.sourceToStringWithCasing(MangaUtil.MangaSources.MangaSee));
        //                    latest.Add(int.Parse(latestChapter));
        //                    ++numberOfResults;
        //                    Console.WriteLine($"Found {title} with latest chapter {latestChapter}");
        //                }
        //            }
        //            catch (NullReferenceException)
        //            {
        //                //if last page was dead than we are done
        //                nextPage = "DEAD";
        //            }
        //            if (nextPage != "DEAD")
        //            {
        //                ++pageNum;
        //                nextPage = $"https://fanfox.net/search?page={pageNum}&title={srchqry}#searchlt";
        //                html = Request(nextPage);
        //            }

        //        }

        //        for (int i = 0; i < numberOfResults; ++i)
        //        {
        //            results.Add(new SearchType(name[i], link[i], latest[i], sources[i]));
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// Searches for a manga on MangaSee
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchMangaSee(int startChapter, string novelname)
        {
            string srchqry = novelname.Replace(" ", "+");
            string url = $"https://mangasee123.com/search/?name={srchqry}";
            int pageNum = 1;

            var html = Request(url);
            if (html.DocumentNode != null)
            {
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> sources = new List<string>();
                List<int> latest = new List<int>();

                string nextPage = $"https://fanfox.net/search?page={pageNum}&title={srchqry}#searchlt";

                int count = 0;
                
                foreach(HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='col-md-10 col-8']"))
                {
                    var a = node.SelectSingleNode("//a[@class='SeriesName ng-binding']");
                    var href = a.Attributes["href"].Value;
                    href = $"https://fanfox.net{href}";
                    link.Add(href);
                    
                    var title = a.InnerText;
                    name.Add(title);

                    sources.Add(MangaUtil.sourceToStringWithCasing(MangaUtil.MangaSources.MangaSee));
                    latest.Add(int.Parse(GetMangaSeeLatestChapter(href)));
                    ++numberOfResults;
                }
                
                for (int i = 0; i < numberOfResults; ++i)
                {
                    results.Add(new SearchType(name[i], link[i], latest[i], sources[i]));
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private string GetMangaSeeLatestChapter(string url)
        {
            var html = Request(url);
            var node = html.DocumentNode.SelectSingleNode("//div[@class='list-group top-10 bottom-5 ng-scope']");
            var a = node.SelectSingleNode("//a[@class='list-group-item ChapterLink ng-scope']");
            string latest = a.SelectSingleNode("//div[@class='ng-binding']").InnerText;
            latest = Regex.Match(latest, @"\d+").Value;
            return latest;
        }

        internal List<MangaData> QueryMangaSeeANdGetChapters(ref MangaData novel, int first)
        {
            List<MangaData> novels = new();

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


            foreach (HtmlNode node in html.DocumentNode.SelectNodes("//ul[@class='detail-main-list']"))
            {
                foreach (HtmlNode chapter in node.SelectNodes(".//li"))
                {
                    var a = chapter.SelectSingleNode(".//a");
                    var href = a.Attributes["href"].Value;
                    href = $"https://fanfox.net{href}";
                    var chapterNumTxt = a.SelectSingleNode(".//div").SelectSingleNode("p[@class='title3']").InnerText;
                    var chapterNumber = chapterNumTxt.Substring(chapterNumTxt.LastIndexOf("Ch.") + 3);
                    chapterNumber = Regex.Match(chapterNumber, @"\d+").Value;
                    if (double.Parse(chapterNumber) >= first)
                    {
                        novels.Add(new MangaData(novel.name, first, double.Parse(chapterNumber), href, novel.source));
                    }
                }
            }

            return novels;
        }
    }
}
