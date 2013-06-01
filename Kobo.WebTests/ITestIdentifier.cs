using System.Collections.Generic;
using System.Reflection;

namespace Kobo.WebTests
{
    public interface ITestIdentifier
    {
        TestDefinition Identify(MethodInfo method, ParameterInfo[] parameters);

        IEnumerable<TestMethod> Select(CrawlResult result, IEnumerable<TestDefinition> tests);
    }
}