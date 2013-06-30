using System;
using System.Reflection;

namespace CrawlRunner
{
    public abstract class TestDefinition
    {
        protected TestDefinition(MethodInfo method, Type parsedObjectType = null)
        {
            Method = method;
            ParsedObjectType = parsedObjectType;
        }

        public Type ParsedObjectType { get; private set; }

        public MethodInfo Method { get; private set; }

        public abstract bool CompatibleWith(CrawlResult result);
    }
}