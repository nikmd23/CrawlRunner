using System;
using System.Collections.Generic;
using System.Reflection;
using CsQuery;

namespace CrawlRunner
{
    public class WidgetTestDefinition : TestDefinition
    {
        public WidgetTestDefinition(MethodInfo method, IEnumerable<string> cssSelector, Type parsedObjectType = null) : base(method, parsedObjectType)
        {
            CssSelector = cssSelector;
        }

        public IEnumerable<string> CssSelector { get; set; }

        public override bool CompatibleWith(CrawlResult result)
        {
            return result.ContentType == typeof(CQ);
        }
    }
}