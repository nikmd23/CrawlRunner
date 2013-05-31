using System;
using System.Reflection;

namespace Kobo.WebTests
{
    public class TestResult
    {
        public TestResult(Uri uri, MethodInfo test) : this(uri, test, true, null){}

        public TestResult(Uri uri, MethodInfo test, Exception exception) : this(uri, test, false, exception){}

        private TestResult(Uri uri, MethodInfo test, bool success, Exception exception)
        {
            Uri = uri;
            Test = test;
            Success = success;
            Exception = exception;
        }

        public Uri Uri { get; private set; }

        public MethodInfo Test { get; private set; }

        public bool Success { get; private set; }

        public Exception Exception { get; private set; }
    }
}