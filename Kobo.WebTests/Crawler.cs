using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Kobo.WebTests
{
    public class Crawler
    {
        public Crawler(Queue<string> uris)
        {
            Uris = uris;
            ContentStrategies = new ContentStrategies();
        }

        public Queue<string> Uris { get; set; }

        public ContentStrategies ContentStrategies { get; private set; }

        public IObservable<Task<CrawlResult>> Crawl()
        {
            return Observable.Create(
                (IObserver<Task<CrawlResult>> observer) =>
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

                        var timer = Stopwatch.StartNew();
                        var task = httpClient.GetAsync(crawlRequest.Uri).ContinueWith(
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
                                }, timer);

                        tasks.Add(task);
                        observer.OnNext(task);
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
