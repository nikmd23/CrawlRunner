using System;
using System.Collections.Generic;

namespace Kobo.WebTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            var uris = new Queue<Uri>();
            uris.Enqueue(new Uri("http://www.bing.com"));
            uris.Enqueue(new Uri("http://www.espn.com"));
            uris.Enqueue(new Uri("http://www.google.com"));
            uris.Enqueue(new Uri("http://www.msn.com"));
            uris.Enqueue(new Uri("http://www.yahoo.com"));

            var crawler = new Crawler(uris);
            crawler.Crawl();

            Console.ReadLine();
        }
    }
}
