using System;
using System.Linq;

namespace CrawlRunner.Crawler.Configuration
{
    public static class Start
    {
        public static CrawlSession At(params string[] uris)
        {
            return new CrawlSession(uris.Select(uri => new Uri(uri)));
        }
    }
}