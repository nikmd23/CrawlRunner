using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace CrawlRunner
{
    public class TestExecutor
    {
        public IObservable<TestResult> Execute(IObservable<TestMethod> tests)
        {
            return Observable.Create(
                (IObserver<TestResult> observer) =>
                    {
                        tests.Subscribe(
                            onNext: test =>
                            {
                                try
                                {
                                    var declaringType = test.Method.DeclaringType;
                                    var instance = Activator.CreateInstance(declaringType);
                                    test.Method.Invoke(instance, test.Parameters);
                                    observer.OnNext(new TestResult(test.Uri, test.Method));
                                }
                                catch (Exception ex)
                                {
                                    observer.OnNext(new TestResult(test.Uri, test.Method, ex));
                                }
                            },
                            onError:observer.OnError,
                            onCompleted:observer.OnCompleted);

                        return Disposable.Empty;
                    });
        }
    }
}