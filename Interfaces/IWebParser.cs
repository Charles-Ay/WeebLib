using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Interfaces
{
    internal abstract class IWebParser<T>
    {
        protected string WorkDir { get; set; }
        /// <summary>
        /// returns the amount of items fetched
        /// </summary>
        /// <param name="data">The class data used</param>
        /// <param name="start">The start point of the fetch(ie chapter 1)</param>
        /// <returns></returns>
        protected abstract int Fetch(T data, int start);
        protected abstract void SetWorkDir();
        internal string GetWorkDir()
        {
            return WorkDir;
        }
    }
}
