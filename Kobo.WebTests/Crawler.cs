using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kobo.WebTests
{
    public class Crawler
    {
        public Crawler(Queue<string> uris, TextWriter writer = null)
        {
            Uris = uris;
            //Writer = writer ?? TextWriter.Null;
            ContentStrategies = new Dictionary<string, Func<HttpContent, Task<object>>>
                {
                    { "text/html", Html },
                    { "application/json", Json },
                };
        }

        public Queue<string> Uris { get; set; }

        public IDictionary<string, Func<HttpContent, Task<object>>> ContentStrategies { get; set; }

        //public TextWriter Writer { get; set; }

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
                        if (requests.Any(r => r.Uri.Equals(crawlRequest.Uri))) // TODO: Add in filter checks for Depth, Domain whitelist, visited list
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
                                        var parsedContent = ParseContent(response.Content).Result;
                                        crawlResult.SetContent(response.Content, parsedContent);
                                            
                                        // TODO: Find additional links in parsedContent and add to Uri's
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

        private async Task<object> ParseContent(HttpContent content)
        {
            var mediaType = content.Headers.ContentType.MediaType;

            if (ContentStrategies.ContainsKey(mediaType))
                return await ContentStrategies[mediaType](content);

            throw new NotSupportedException("Content of type '" + mediaType + "' is not supported.");
        }

        private async Task<object> Html(HttpContent content)
        {
            var stream = await content.ReadAsStreamAsync();
            var result = new HtmlDocument();
            result.Load(stream);
            return result;
        }

        private async Task<object> Json(HttpContent content)
        {
            var stream = await content.ReadAsStreamAsync();
            var streamReader = new StreamReader(stream);
            var textReader = new JsonTextReader(streamReader);
            try
            {
                return JObject.Load(textReader);
            }
            catch
            {
                return JArray.Load(textReader);
            }
            finally
            {
                streamReader.Dispose();
            }
        }
    }
}
