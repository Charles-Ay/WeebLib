using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeebLib.Novel.Parser
{
    internal class NovelSourceParser
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

        /// <summary>
        /// Parses the source of a novel from freewebnovel.com
        /// </summary>
        /// <param name="html"></param>
        /// <param name="novelText">The text of the novel</param>
        /// <returns></returns>
        private static bool FreeWebNovelParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    //Get title
                    novelText += html.DocumentNode.SelectSingleNode("//h1[@class='tit']").InnerText + "\n";

                    //Parse each paragraph
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

        /// <summary>
        /// Parses the source of a novel from full-novel.com
        /// </summary>
        /// <param name="html"></param>
        /// <param name="novelText">The text of the novel</param>
        /// <returns></returns>
        private static bool FullNovelParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    //Get the main content
                    HtmlNode contentNode = html.DocumentNode.SelectSingleNode("//div[@id='chr-content']");

                    //Get title
                    novelText = HttpUtility.HtmlDecode(contentNode.SelectSingleNode("//a[@class='novel-title']").InnerText);
                    novelText += "\n";

                    //Parse each paragraph
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

        [Obsolete("NovelTrenchParse is obsolete, FullNovelParse or FreeWebNovelParse instead")]
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
