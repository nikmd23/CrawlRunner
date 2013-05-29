using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Kobo.WebTests
{
    public class TaskStateManager
    {
        private int count = 0;

        public TaskState GetToken(Uri uri)
        {
            return new TaskState(uri.AbsoluteUri, count++, Console.Out);
        }
    }

    public class TaskState
    {
        public TaskState(string uri, int line, TextWriter writer)
        {
            Uri = uri;
            Line = line;
            Writer = writer;
        }

        public string Uri { get; set; }

        public int Line { get; set; }

        public TextWriter Writer { get; set; }

        public void SetStatusCode(int statusCode, string phrase)
        {
            UpdateLine(string.Format("{0} ({1})", statusCode, phrase));
        }

        private void UpdateLine(string message)
        {
            Console.SetCursorPosition(0, Line);
            Console.WriteLine("{0}\t{1}\t", Uri, message);
        }
    }

    public class Crawler
    {
        public Crawler(Queue<Uri> uris)
        {
            Uris = uris;
            ContentStrategies = new Dictionary<string, Func<HttpContent, object>>
                {
                    { "test/html", Html }
                };
        }

        public Queue<Uri> Uris { get; set; }
        public IDictionary<string, Func<HttpContent, object>> ContentStrategies { get; set; }

        public void Crawl()
        {
            var tasks = new List<Task>();
            var httpClient = new HttpClient();
            var stateManager = new TaskStateManager();

            while(Uris.Count > 0)
            {
                var uri = Uris.Dequeue();
                var link = new CrawlerLink(uri).GetResolvedTarget();
                
                Console.WriteLine("{0}\tWaiting\t", link);
                
                tasks.Add(httpClient.GetAsync(link).ContinueWith(
                    (responseTask, taskState) =>
                        {
                            var response = responseTask.Result;
                            var state = (TaskState)taskState;

                            state.SetStatusCode((int)response.StatusCode, response.ReasonPhrase);

                            HandleContent(response);
                        }, 
                        stateManager.GetToken(link)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private object HandleContent(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) 
                return null;

            var mediaType = response.Content.Headers.ContentType.MediaType;

            if (ContentStrategies.ContainsKey(mediaType))
                return ContentStrategies[mediaType](response.Content);

            return null;
        }

        private object Html(HttpContent content)
        {
            var stream = content.ReadAsStreamAsync().Result;
            var document = new HtmlDocument();
            document.Load(stream);
            return document;
        }
    }
}
