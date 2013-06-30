using System;
using System.Text.RegularExpressions;

namespace CrawlRunner
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WebTestAttribute : Attribute
    {
        public WebTestAttribute() : this("*")
        {
        }

        public WebTestAttribute(string regex)
        {
            var wildcard = "^" + Regex.Escape(regex).
                        Replace("\\*", ".*").
                        Replace("\\?", ".") + "$";

            Regex = new Regex(wildcard);
        }

        public Regex Regex { get; set; }
    }
}