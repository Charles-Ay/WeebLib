using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using static WeebLib.Utility.WeebLibUtil;
using WeebLib.Novel.NovelExceptions;
using WeebLib.Novel;
using WeebLib.Manga.MangaExceptions;
using System.Text.RegularExpressions;

namespace WeebLib.Manga.Retrieval
{//TODO CHANGE MANGAINN TO BATO.TO
    public class MangaSearcher : ISearcher
    {
        int numberOfResults;
        public override bool Search(int start, string title, string source = "")
        {
            switch (source)
            {
                case "bato":
                    return SearchBato(start, title);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Searches for a manga on MangaInn
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchBato(int startChapter, string novelname)
        {
            string srchqry = novelname.Replace(" ", "+");
            string url = $"https://bato.to/search?word={srchqry}";
            int pageNum= 1;

            var html = Request(url);
            if (html.DocumentNode != null)
            {
                List<string> name = new List<string>();
                List<string> link = new List<string>();
                List<string> sources = new List<string>();
                string nextPage = $"https://bato.to/search?word={srchqry}&page={pageNum}";


                while (nextPage != "DEAD")
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//a[@class='item-title']"))
                    {
                        var parentNode = node.ParentNode;
                        //We only want english manga
                        if (parentNode.SelectSingleNode("em") is null)
                        {
                            name.Add(node.InnerText);
                            link.Add("https://bato.to" + node.Attributes["href"].Value);
                            sources.Add(MangaUtil.sourceToStringWithCasing(MangaUtil.MangaSources.Bato));
                            ++numberOfResults;
                        }
                    }

                    try
                    {
                        ++pageNum;
                        nextPage = $"https://bato.to/search?word={srchqry}&page={pageNum}";
                        var prevHtml = html;
                        html = Request(nextPage);
                        //See if the pages are the same because if they are then there are no more pages
                        if (html.DocumentNode.InnerText == prevHtml.DocumentNode.InnerText) throw new LastPageException();
                    }
                    catch (LastPageException)
                    {
                        nextPage = "DEAD";
                    }
                    for (int i = 0; i < numberOfResults; ++i)
                    {
                        results.Add(new SearchType(name[i], link[i], sources[i]));
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        internal List<MangaData> QueryBatoAndGetChapters(ref MangaData novel, int first)
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


            foreach (HtmlNode node in html.DocumentNode.SelectNodes("//a[@class='visited chapt']"))
            {
                var link = "https://bato.to" + node.GetAttributeValue("href", string.Empty);
                int num = int.Parse(Regex.Match(node.InnerHtml, @"\d+").Value);
                if (num >= first && num <= novel.totalChapters)
                {
                    novels.Add(new(novel.name, num, link, novel.source));
                }
            }
            return novels;
        }
    }
}
