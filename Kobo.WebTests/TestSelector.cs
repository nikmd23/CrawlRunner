using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kobo.WebTests
{
    public class TestSelector
    {
        private readonly IList<TestDefinition> standardTests = new List<TestDefinition>();
        private readonly IDictionary<Type, IList<TestDefinition>> parametrizedTests = new Dictionary<Type, IList<TestDefinition>>();

        public TestSelector(Assembly[] assemblies = null)
        {
            foreach (var assembly in assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes().Where(type => type.IsPublic))
                    foreach (var method in type.GetMethods().Where(method => method.IsPublic && Attribute.IsDefined(method, typeof(WebTestAttribute))))
                    {
                        var parameters = method.GetParameters();
                        var parametersLength = parameters.Length;

                        if (parametersLength == 0 || parametersLength > 2)
                            throw new NotSupportedException("WebTest methods must have one or two parameters.");

                        if (parameters[0].ParameterType != typeof(CrawlResult))
                            throw new NotSupportedException("WebTest methods must take a CrawlResult as their first parameter.");

                        var attributes = method.GetCustomAttributes<WebTestAttribute>();

                        if (parametersLength == 1)
                        {
                            standardTests.Add(new TestDefinition(method, attributes.Select(a => a.Regex)));
                            continue;
                        }

                        var key = parameters[1].ParameterType;

                        if (!parametrizedTests.ContainsKey(parameters[1].ParameterType))
                            parametrizedTests.Add(key, new List<TestDefinition>());

                        parametrizedTests[key].Add(new TestDefinition(method, attributes.Select(a => a.Regex)));
                    }
        }

        public IObservable<TestMethod> SelectTests(IObservable<Task<CrawlResult>> from)
        {
            return Observable.Create(
            (IObserver<TestMethod> observer) =>
            {
                from.Subscribe(
                    onNext: async result =>
                    {
                        var r = await result;

                        foreach (var test in standardTests)
                            if (test.Regex.Any(regex => regex.IsMatch(r.Uri.AbsoluteUri)))
                            {
                                observer.OnNext(new TestMethod(test, r));
                            }

                        if (r.ContentType != null)
                            foreach (var test in parametrizedTests[r.ContentType])
                                if (test.Regex.Any(regex => regex.IsMatch(r.Uri.AbsoluteUri)))
                                {
                                    observer.OnNext(new TestMethod(test, r, r.Content));
                                }
                    },
                    onCompleted: observer.OnCompleted);

                return Disposable.Empty;
            });
        }
    }
}