using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Novel
{
    public class NovelUtil
    {
        public enum NovelSources
        {
            FreeWebNovel, FullNovel, NovelTrench
        }

        public static string sourceToString(NovelSources sources)
        {
            return htmlSupportedWebsites[sources.ToString()];
        }

        public static string sourceToStringUpper(NovelSources sources)
        {
            foreach (var key in htmlSupportedWebsites.Keys)
            {
                if (key == sources.ToString())
                    return htmlSupportedWebsites[key];
            }
            //TODO: FIX THIS
            throw new Exception();
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
