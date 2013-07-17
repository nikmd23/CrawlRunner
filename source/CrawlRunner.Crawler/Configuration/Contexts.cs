using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CrawlRunner.Crawler.Configuration
{
    public class Contexts : Dictionary<string, Action<HttpClient>>
    {
    }
}