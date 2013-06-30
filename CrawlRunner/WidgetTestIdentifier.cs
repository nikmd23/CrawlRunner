using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CsQuery;

namespace CrawlRunner
{
    public class WidgetTestIdentifier : ITestIdentifier
    {
        public WidgetTestIdentifier(string widgetSelector)
        {
            WidgetSelector = widgetSelector;
        }

        private string WidgetSelector { get; set; }

        public TestDefinition Identify(MethodInfo method, ParameterInfo[] parameters)
        {
            if (Attribute.IsDefined(method, typeof(WidgetTestAttribute)))
            {
                var parametersLength = parameters.Length;

                if (parametersLength != 2)
                    throw new NotSupportedException("WidgetTest methods must have two parameters.");

                if (parameters[0].ParameterType != typeof(CrawlResult))
                    throw new NotSupportedException("WebTest methods must take a CrawlResult as their first parameter.");

                if (parameters[1].ParameterType != typeof(IDomElement))
                    throw new NotSupportedException("WebTest methods must take an HtmlNode as their second parameter.");

                var attributes = method.GetCustomAttributes<WidgetTestAttribute>();

                return new WidgetTestDefinition(method, attributes.Select(a => a.CssSelector), parametersLength == 1 ? null : parameters[1].ParameterType);
            }

            return null;
        }

        public IEnumerable<TestMethod> Select(CrawlResult result, IEnumerable<TestDefinition> tests)
        {
            if (result.ContentType == typeof(CQ))
            {
                var document = (CQ)result.Content;

                foreach (var widget in document[WidgetSelector])
                    foreach (var test in tests.Cast<WidgetTestDefinition>())
                        foreach (var selector in test.CssSelector)
                        {
                            if (string.IsNullOrEmpty(selector))
                            {
                                yield return new TestMethod(test, result, widget);
                                continue;
                            }

                            if (IsMatch(widget, selector))
                            {
                                yield return new TestMethod(test, result, widget);
                            }
                        }
            }
        }

        private bool IsMatch(IDomObject widget, string cssSelector)
        {
            var fragment = CQ.CreateFragment(new []{widget});
            return fragment[cssSelector].Length > 0;
        }
    }
}