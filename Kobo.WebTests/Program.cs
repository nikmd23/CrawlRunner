using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Kobo.WebTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            // TODO: Load me externally
            var uris = new Queue<string>();
            // uris.Enqueue("http://www.bing.com");
            // uris.Enqueue("http://www.espn.com");
            // uris.Enqueue("http://www.google.com");
            uris.Enqueue("http://www.msn.com");
            uris.Enqueue("http://www.msn.com");
            // uris.Enqueue("http://www.yahoo.com");
            uris.Enqueue("http://search.twitter.com/search.json?q=from%3A%40nikmd23");
            // uris.Enqueue("http://localhost:1234/madeup.html");

            var crawler = new Crawler(uris);
            var crawlResults = crawler.Crawl();

            using (crawlResults.Subscribe(
                onNext: async c => Console.WriteLine((await c).Uri),
                onCompleted: () => Console.WriteLine("Complete!"),
                onError: e => Console.WriteLine(e.Message)))
            {
                Console.WriteLine("WATING");
                Console.ReadLine();
            }

            //var testSelector = new TestSelector();
            //var tests = testSelector.SelectTests(from: crawlResults);

            //var testExecutor = new TestExecutor();
            //testExecutor.Execute(tests);

            Console.ReadLine();
        }
    }

/*
    public class TestSelector
    {
        public TestSelector()
        {
            
        }

        public ObservableCollection<Test> SelectTests(ObservableCollection<CrawlResult> from)
        {
            from.CollectionChanged += FindTest;
        }

        private void FindTest(object sender, NotifyCollectionChangedEventArgs e)
        {
            Parallel.ForEach(e.NewItems.Cast<CrawlResult>(), item => Console.WriteLine("Got Test! {0}\t{1}\t{2}", item.Uri, item.StatusCode, item.MediaType));
        }
    }
*/
}
