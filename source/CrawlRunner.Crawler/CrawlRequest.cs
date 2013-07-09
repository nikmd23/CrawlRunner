using System;

namespace CrawlRunner.Crawler
{
    public class CrawlRequest
    {
        public CrawlRequest(string uri)
        {
            Uri = new Uri(uri);

            Depth = 0;
        }

        public int Depth { get; set; }

        public Uri Uri { get; set; }
    }
}