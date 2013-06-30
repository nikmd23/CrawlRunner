using System;
using System.Collections.Generic;

namespace CrawlRunner
{
    public class Program
    {
        static void Main()
        {
            var complete = false;
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

            var count = 0;
            var failures = new List<string>();
            results.Subscribe(
                onNext:result =>
                {
                    Console.WriteLine("\n{0}.{1} for {2} : {3}", 
                        result.Test.DeclaringType.FullName, 
                        result.Test.Name, 
                        result.Uri, 
                        result.Success ? "Success" : "Failed");

                    if (!result.Success)
                    {
                        Console.Error.WriteLine(result.Exception);
                        failures.Add(string.Format("\n{0}.{1} for {2}\n{3}",
                            result.Test.DeclaringType.FullName, 
                            result.Test.Name, 
                            result.Uri.AbsoluteUri, 
                            result.Exception.Message));
                    }

                    count++;
                },
                onCompleted: () =>
                    {
                        if (failures.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Error.WriteLine("\nTESTS FAILED ({0} passed/{1} total)", count-failures.Count, count);
                            failures.ForEach(Console.Error.WriteLine);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Error.WriteLine("\nTESTS SUCCEEDED ({0} passed/{1} total)", count, count);
                        }
                        complete = true;
                    });

            while(!complete){}

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nComplete. Press <ENTER> to exit.");
            Console.ReadLine();
        }
    }
}
