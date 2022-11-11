using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Novel.NovelExceptions;

namespace WeebLib.Novel
{
    public class NovelUtil
    {
        public enum NovelSources
        {
            FreeWebNovel, FullNovel, NovelTrench
        }
        
        /// <summary>
        /// Turns source enum to string
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string sourceToString(NovelSources sources)
        {
            return htmlSupportedWebsites[sources.ToString()];
        }

        /// <summary>
        /// Turns source enum to string with proper caseing
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string sourceToStringWithCasing(NovelSources sources)
        {
            foreach (var key in htmlSupportedWebsites.Keys)
            {
                if (key == sources.ToString())
                    return htmlSupportedWebsites[key];
            }
            return string.Empty;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c== ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static Dictionary<string, string> htmlSupportedWebsites = new Dictionary<string, string>()
        {
            {"FreeWebNovel", "freewebnovel"}, {"FullNovel", "fullnovel"}, {"NovelTrench", "noveltrench"}
        };
    }
}
