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
            FreeWebNovel, NovelTrench
        }

        public static Dictionary<string, string> htmlSupportedWebsites = new Dictionary<string, string>()
        {
            {"freewebnovel", "FreeWebNovel"}, {"noveltrench", "NovelTrench"}
        };
    }
}
