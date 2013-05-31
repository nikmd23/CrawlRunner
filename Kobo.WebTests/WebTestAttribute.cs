using System;
using System.Text.RegularExpressions;

namespace Kobo.WebTests
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