using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;

namespace WeebLib.Novel.Parser
{
    internal class NovelParser : IWebParser<NovelData>
    {
        protected override int Fetch(NovelData data, int start)
        {
            throw new NotImplementedException();
        }

        protected override void SetWorkDir()
        {
            throw new NotImplementedException();
        }
    }
}
