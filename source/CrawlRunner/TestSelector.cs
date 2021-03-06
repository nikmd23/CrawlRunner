﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CrawlRunner.Crawler;

namespace CrawlRunner
{
    public class TestSelector
    {
        private readonly IDictionary<int, IList<TestDefinition>> tests = new Dictionary<int, IList<TestDefinition>>();

        public TestSelector(params ITestIdentifier[] identifiers) : this(identifiers, null){}

        public TestSelector(IEnumerable<ITestIdentifier> identifiers) : this(identifiers, null){}

        public TestSelector(IEnumerable<ITestIdentifier> identifiers, params Assembly[] assemblies)
        {
            Identifiers = identifiers;

            foreach (var assembly in assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes().Where(type => type.IsPublic))
                    foreach (var method in type.GetMethods().Where(method => method.IsPublic))
                    {
                        var parameters = method.GetParameters();
                        foreach (var identifier in Identifiers)
                        {
                            var definition = identifier.Identify(method, parameters);

                            if (definition != null)
                            {
                                var key = identifier.GetHashCode();

                                if (!tests.ContainsKey(key))
                                    tests.Add(key, new List<TestDefinition>());

                                tests[key].Add(definition);
                            }
                        }
                    }
        }

        private IEnumerable<ITestIdentifier> Identifiers { get; set; }

        public IObservable<TestMethod> SelectTests(IObservable<CrawlResult> from)
        {
            return Observable.Create(
            (IObserver<TestMethod> observer) =>
            {
                from.Subscribe(
                    onNext: async result =>
                    {
                        foreach (var identifier in Identifiers)
                        {
                            var compatibleTests = tests[identifier.GetHashCode()].Where(t => t.CompatibleWith(result));

                            foreach (var test in identifier.Select(result, compatibleTests))
                                observer.OnNext(test);
                        }
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted);

                return Disposable.Empty;
            });
        }
    }
}