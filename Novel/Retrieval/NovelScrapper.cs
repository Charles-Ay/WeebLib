using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.NovelExceptions;
using WeebLib.Novel.Parser;
using WeebLib.Utility;

namespace WeebLib.Novel.Retrieval
{
    internal class NovelScrapper : IWebScrapper<NovelData>
    {
        protected string? novelText;
        public override int Scrape(List<NovelData> data, string dir)
        {
            bool returnedValue = false;
            int amount = 0;

            //Check if the "Novels" Dir exist. If not make a new one
            if (!Directory.Exists(Path.Combine(dir, "Novels"))) Directory.CreateDirectory(Path.Combine(dir, "Novels"));
            dir = Path.Combine(dir, "Novels");

            foreach (NovelData novel in data)
            {
                SourceParser sourceParser = new SourceParser();
                
                var html = base.GetSite(novel.initalLink);
                if (novel.initalLink.Contains("freewebnovel")) returnedValue = SourceParser.Parse(NovelUtil.NovelSources.FreeWebNovel, html, out novelText);
                else if (novel.initalLink.Contains("fullnovel") || novel.initalLink.Contains("full-novel")) returnedValue = SourceParser.Parse(NovelUtil.NovelSources.FullNovel, html, out novelText);
                else if (novel.initalLink.Contains("noveltrench")) returnedValue = SourceParser.Parse(NovelUtil.NovelSources.NovelTrench, html, out novelText);

                if (returnedValue == false)
                {
                    throw new NovelContentException("Unable to parse novel content", WeebLibUtil.ContentType.Novel);
                }
                else if (novelText == "")
                {
                    throw new NovelContentException("Empty novel content", WeebLibUtil.ContentType.Novel);
                }

                //Total chapters becomes current chapter
                var novelDir = Path.Combine(dir, NovelUtil.RemoveSpecialCharacters(novel.name));
                if (!Directory.Exists(novel.name))
                {
                    Directory.CreateDirectory(novelDir);
                }
                
                string fileName = Path.Combine(novelDir, $"{NovelUtil.RemoveSpecialCharacters(novel.name)} - Chapter {novel.totalChapters}.txt");
                if (!File.Exists(fileName)) File.Create(fileName).Dispose();

                //FileStream writitng due to Numeric character reference
                //https://en.wikipedia.org/wiki/Numeric_character_reference
                FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate);
                StreamWriter streamWriter = new StreamWriter(stream);
                streamWriter.WriteLine(novelText);
                streamWriter.Close();
                stream.Close();
                
                //add to amount of chapters found
                ++amount;
            }
            return amount;
        }
    }
}
