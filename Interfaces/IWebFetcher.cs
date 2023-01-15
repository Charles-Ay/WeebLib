using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Interfaces
{
    public abstract class IWebFetcher<T>
    {
        protected string WorkDir = "";
        /// <summary>
        /// Fetches content from web
        /// </summary>
        /// <param name="data">The class data used</param>
        /// <param name="start">The start point of the fetch(ie chapter 1)</param>
        /// <param name="outputToFile">Output results to file</param>
        /// <returns>text if output to file is false</returns>
        protected abstract string Fetch(T data, int start, bool outputToFile = true);
        /// <summary>
        /// Set the current working directory
        /// </summary>
        protected abstract void SetWorkDir(string dir = "");
        public string GetWorkDir()
        {
            return string.IsNullOrEmpty(WorkDir) ? throw new NullReferenceException("WorkDir is null") : WorkDir;
        }
    }
}
