using System;
using System.Collections.Generic;

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

            var crawler = new Crawler(uris);
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
