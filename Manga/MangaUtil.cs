using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Manga
{
    public class MangaUtil
    {
        public enum MangaSources
        {
            KissManga
        }

        /// <summary>
        /// Turns source enum to string
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string sourceToString(MangaSources sources)
        {
            return supportedMangaWebsites[sources.ToString()];
        }

        /// <summary>
        /// Turns source enum to string with proper caseing
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string sourceToStringWithCasing(MangaSources sources)
        {
            foreach (var key in supportedMangaWebsites.Keys)
            {
                if (key == sources.ToString())
                    return supportedMangaWebsites[key];
            }
            return string.Empty;
        }

        private static Dictionary<string, string> supportedMangaWebsites = new Dictionary<string, string>()
        {
            {"KissManga", "kissmanga"}
        };
    }
}