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
using System.ComponentModel;

namespace WeebLib.Manga.Retrieval
{
    public class MangaSearcher : ISearcher
    {
        int numberOfResults;
        public override bool Search(int start, string title, string source = "")
        {
            switch (source)
            {
                case "kissmanga":
                    return SearchKissManga(start, title);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Searches for a manga on KissManga
        /// </summary>
        /// <param name="startChapter"></param>
        /// <param name="novelname"></param>
        /// <returns>True if content was retrived successfully</returns>
        private bool SearchKissManga(int startChapter, string novelname)
        {
            string srchqry = novelname.Replace(" ", "+");
            string url = $"https://kissmanga.org/manga_list?page=1&action=search&q={srchqry}";
            int pageNum = 1;

            var html = Request(url);
            if (html.DocumentNode != null)
            {
                List<string> name = new();
                List<string> link = new();
                List<string> sources = new();
                List<double> latest = new();

                string nextPage = $"https://kissmanga.org/manga_list?page={pageNum}&action=search&q={srchqry}";
                
                int count = 0;
                
                foreach(HtmlNode node in html.DocumentNode.SelectNodes("//a[@class='item_movies_link']"))
                {
                    var href = node.Attributes["href"].Value;
                    href = $"https://kissmanga.org{href}";
                    link.Add(href);
                    
                    var title = node.InnerText;

                    var chapter = GetKissMangaLatestChapter(href);
                    if (chapter != "0" && chapter != "")
                    {
                        latest.Add(double.Parse(chapter));
                        link.Add(href);
                        name.Add(title);
                        sources.Add(MangaUtil.sourceToStringWithCasing(MangaUtil.MangaSources.KissManga));


                        ++numberOfResults;
                    }
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

        private string GetKissMangaLatestChapter(string url)
        {
            var html = Request(url);
            var node = html.DocumentNode.SelectSingleNode("//h3");
            var latest = node.ChildNodes[1].InnerText;
            latest = latest.Substring(latest.IndexOf("Ch") + 3);
            //get last floating point number
            latest = Regex.Match(latest, @"\d*\.\d+|\d+\.\d*|\d+(?=\D*$)").Value;
            return latest;
        }

        internal List<MangaData> QueryKissMangaAndGetChapters(ref MangaData manga, int targetChapter)
        {
            List<MangaData> mangas = new();
            var html = Request(manga.initalLink);

            var outer = html.DocumentNode.SelectNodes("//div[@class='barContent episodeList full']");
            foreach (HtmlNode node in outer)
            {
                foreach (HtmlNode chapter in node.SelectNodes(".//h3"))
                {
                    var a = chapter.SelectSingleNode(".//a");
                    var href = a.Attributes["href"].Value;
                    href = $"https://kissmanga.org{href}";
                    var chapterNumber = href;
                    var temp = chapterNumber;
                    chapterNumber = Regex.Match(chapterNumber, @"\d*\.\d+|\d+\.\d*|\d+(?=\D*$)").Value;
                    if (double.Parse(chapterNumber) <= targetChapter)
                    {
                        mangas.Add(new MangaData(manga.name, targetChapter, double.Parse(chapterNumber), href, manga.source));
                    }
                }
                return mangas;
            }
            return null;

        }
    }
}
