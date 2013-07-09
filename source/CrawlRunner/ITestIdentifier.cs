using System.Collections.Generic;
using System.Reflection;
using CrawlRunner.Crawler;

namespace CrawlRunner
{
    public interface ITestIdentifier
    {
        TestDefinition Identify(MethodInfo method, ParameterInfo[] parameters);

        IEnumerable<TestMethod> Select(CrawlResult result, IEnumerable<TestDefinition> tests);
    }
}