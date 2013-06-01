using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kobo.WebTests
{
    public class WebTestDefinition : TestDefinition
    {
        public WebTestDefinition(MethodInfo method, IEnumerable<Regex> regex, Type parsedObjectType = null) : base(method, parsedObjectType)
        {
            Regex = regex;
        }

        public IEnumerable<Regex> Regex { get; private set; } // TODO: make this abstract as a method

        public override bool CompatibleWith(CrawlResult result)
        {
            return Regex.Any(regex => regex.IsMatch(result.Uri.AbsoluteUri));
        }
    }
}