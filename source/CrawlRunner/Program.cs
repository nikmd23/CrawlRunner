using System;
using System.Collections.Generic;
using CrawlRunner.Crawler.Configuration;

namespace CrawlRunner
{
    public class Program
    {
        static void Main()
        {
            // TODO: Load strings externally from a JSON format. Context is passed from JSON
            var uris = new Queue<string>();
            uris.Enqueue("http://www.msn.com");
            uris.Enqueue("http://www.msn.com");
            uris.Enqueue("http://search.twitter.com/search.json?q=nikmd23");
            uris.Enqueue("http://www.bing.com");
            uris.Enqueue("http://www.espn.com");
            uris.Enqueue("http://www.google.com");
            uris.Enqueue("http://www.yahoo.com");
            // uris.Enqueue("http://localhost:1234/made-up.html");

            var config = CrawlerConfiguration.Configure(
                Start.At("http://www.msn.com", 
                    "http://search.twitter.com/search.json?q=nikmd23", 
                    "http://www.bing.com", 
                    "http://www.espn.com", 
                    "http://www.google.com", 
                    "http://www.yahoo.com")
                    .WithParameters(new {Title="Test", Value = 1}, new {Title="Test2"}),
                    Start.At("http://yahoo.com:80/").With(new Contexts
                        {
                            {"sample", client => client.DefaultRequestHeaders.Add("test", "X")}
                        }));

            Console.Write(config);
            Console.ReadLine();

            var crawler = new Crawler.Crawler(uris);
            var crawlResults = crawler.Crawl();

            var testSelector = new TestSelector(new WebTestIdentifier(), new WidgetTestIdentifier("#wrapper > div"));
            var tests = testSelector.SelectTests(@from: crawlResults);

            var testExecutor = new TestExecutor();
            var results = testExecutor.Execute(tests);

            var console = new ConsoleOutput();
            console.Display(results);

            //Thread.Sleep(2000);
            //crawler.Stop();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nComplete. Press <ENTER> to exit.");
            Console.ReadLine();
        }
    }
}
