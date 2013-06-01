using System;
using System.Reflection;

namespace Kobo.WebTests
{
    public class TestMethod
    {
        public TestMethod(TestDefinition method, params object[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                throw new ArgumentException("Tests must have at lease one parameter.");

            Method = method.Method;
            Parameters = parameters;
            Uri = ((CrawlResult)parameters[0]).Uri;
        }

        public MethodInfo Method { get; private set; }

        public object[] Parameters { get; private set; }

        public Uri Uri { get; private set; }
    }
}