using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CrawlRunner.Crawler;

namespace CrawlRunner
{
    public class WebTestIdentifier : ITestIdentifier
    {
        public TestDefinition Identify(MethodInfo method, ParameterInfo[] parameters)
        {
            if (Attribute.IsDefined(method, typeof(WebTestAttribute)))
            {
                var parametersLength = parameters.Length;

                if (parametersLength == 0 || parametersLength > 2)
                    throw new NotSupportedException("WebTest methods must have one or two parameters.");

                if (parameters[0].ParameterType != typeof(CrawlResult))
                    throw new NotSupportedException("WebTest methods must take a CrawlResult as their first parameter.");

                var attributes = method.GetCustomAttributes<WebTestAttribute>();

                return new WebTestDefinition(method, attributes.Select(a => a.Regex), parametersLength == 1 ? null : parameters[1].ParameterType);
            }

            return null;
        }

        public IEnumerable<TestMethod> Select(CrawlResult result, IEnumerable<TestDefinition> tests)
        {
            foreach (var test in tests.Where(t => t.ParsedObjectType == null))
                yield return new TestMethod(test, result);

            if (result.ContentType != null)
                foreach (var test in tests.Where(t => t.ParsedObjectType == result.ContentType))
                    yield return new TestMethod(test, result, result.Content);
        }
    }
}