using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.Parser;

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
                if (novel.initalLink.Contains("freewebnovel")) returnedValue = sourceParser.Parse(NovelUtil.NovelSources.FreeWebNovel, html, out novelText);
                else if (novel.initalLink.Contains("noveltrench")) returnedValue = sourceParser.Parse(NovelUtil.NovelSources.NovelTrench, html, out novelText);

                if (novelText == "")
                {
                    // TODO: throw some internal error
                }
                if (returnedValue == false)
                {
                    //TODO: MAKE THIS A MORE SPECIFIC ERROR(CUSTOM EXCEPTION???)
                    throw new InvalidOperationException();
                }

                //Total chapters becomes current chapter
                string fileName = Path.Combine(dir, $"{novel.name} - Chapter {novel.totalChapters}.txt");
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
