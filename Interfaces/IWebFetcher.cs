using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Interfaces
{
    public abstract class IWebFetcher<T>
    {
        protected string? WorkDir = "";
        /// <summary>
        /// Fetches content from web
        /// </summary>
        /// <param name="data">The class data used</param>
        /// <param name="start">The start point of the fetch(ie chapter 1)</param>
        /// <returns>returns the amount of items fetched</returns>
        protected abstract int Fetch(T data, int start);
        /// <summary>
        /// Set the current working directory
        /// </summary>
        protected abstract void SetWorkDir();
        internal string GetWorkDir()
        {
            return string.IsNullOrEmpty(WorkDir) ? throw new NullReferenceException("WorkDir is null") : WorkDir;
        }
    }
}
