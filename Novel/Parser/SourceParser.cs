using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeebLib.Novel.Parser
{
    internal class SourceParser
    {
        /// <summary>
        /// Parses the source of a novel
        /// </summary>
        /// <param name="source"></param>
        /// <param name="html"></param>
        /// <param name="novelText"></param>
        /// <returns>returns true if novel was sucessfully parsed</returns>
        internal static bool Parse(NovelUtil.NovelSources source, HtmlDocument html, out string novelText)
        {
            novelText = "";
            switch (source)
            {
                case NovelUtil.NovelSources.FreeWebNovel:
                    return FreeWebNovelParse(html, out novelText);
                case NovelUtil.NovelSources.FullNovel:
                    return FullNovelParse(html, out novelText);
                case NovelUtil.NovelSources.NovelTrench:
                    return NovelTrenchParse(html, out novelText);
                default:
                    return false;
            }
        }

        private static bool FreeWebNovelParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    //Get title
                    novelText += html.DocumentNode.SelectSingleNode("//h1[@class='tit']");

                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='txt ']"))
                    {
                        novelText += node.InnerText;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else return false;
            return true;
        }

        private static bool FullNovelParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    HtmlNode contentNode = html.DocumentNode.SelectSingleNode("//div[@id='chr-content']");
                    novelText = HttpUtility.HtmlDecode(contentNode.SelectSingleNode("//a[@class='novel-title']").InnerText);
                    novelText += "\n";
                    foreach (HtmlNode node in contentNode.SelectNodes("//p"))
                    {
                        novelText += HttpUtility.HtmlDecode(node.InnerText);
                        novelText += "\n";
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else return false;
            return true;
        }

        private static bool NovelTrenchParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='text-left']"))
                    {
                        novelText += HttpUtility.HtmlDecode(node.InnerText);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else return false;
            return true;
        }
    }
}
