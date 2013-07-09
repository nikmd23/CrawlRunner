using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlRunner.Crawler
{
    public class Crawler
    {
        public Crawler(Queue<string> uris)
        {
            Uris = uris;
            ContentStrategies = new ContentStrategies();
            
            TokenSource = new CancellationTokenSource();
            Token = TokenSource.Token;
            Token.ThrowIfCancellationRequested();
        }

        private CancellationTokenSource TokenSource { get; set; }

        private CancellationToken Token { get; set; }

        public Queue<string> Uris { get; set; }

        public ContentStrategies ContentStrategies { get; private set; }

        public void Stop()
        {
            TokenSource.Cancel();
        }

        public IObservable<CrawlResult> Crawl()
        {
            return Observable.Create(
                async (IObserver<CrawlResult> observer) =>
                {
                    var httpClient = new HttpClient();
                    var tasks = new List<Task<CrawlResult>>();
                    var requests = new List<CrawlRequest>();

                    while (Uris.Count > 0)
                    {
                        var crawlRequest = new CrawlRequest(Uris.Dequeue());
                        if (requests.Any(r => r.Uri.Equals(crawlRequest.Uri))) // TODO: Add in filter checks for Depth, Domain white list, visited list
                            continue;
                        requests.Add(crawlRequest);

                        var task = httpClient.GetAsync(crawlRequest.Uri, Token).ContinueWith(
                            (responseTask, state) =>
                                {
                                    if (responseTask.IsFaulted)
                                    {
                                        observer.OnError(responseTask.Exception);
                                        // TODO: observer.OnNext(new CrawlResult(exception))??
                                    }

                                    var response = responseTask.Result;
                                    var stopwatch = (Stopwatch)state;

                                    var crawlResult = new CrawlResult(response.RequestMessage, response, stopwatch.Elapsed);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        var parsedContent = ParseContent(response.Content);
                                        crawlResult.SetContent(response.Content, parsedContent);
                                            
                                        // TODO: Find additional links in parsedContent and add to Uris
                                    }

                                    return crawlResult;
                                }, Stopwatch.StartNew(), Token);

                            tasks.Add(task);
                            observer.OnNext(await task);
                    }

                    Task.WhenAll(tasks).ContinueWith(_ => observer.OnCompleted());
                    return Disposable.Empty;
                });
        }

        private object ParseContent(HttpContent content)
        {
            var mediaType = content.Headers.ContentType.MediaType;

            if (ContentStrategies.StrategyExists(mediaType))
                return ContentStrategies[mediaType](content);

            // Default to returning the content as a string if all else fails
            return content.ReadAsStringAsync().Result;
        }
    }
}
