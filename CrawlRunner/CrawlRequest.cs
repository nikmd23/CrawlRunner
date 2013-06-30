using System;
using System.Collections.Generic;
using System.Linq;
using Tavis.UriTemplates;

namespace CrawlRunner
{
    public class CrawlRequest
    {
        public CrawlRequest(string uri) : this(uri, Enumerable.Empty<KeyValuePair<string, object>>()){}

        public CrawlRequest(string uri, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var template = new UriTemplate(uri);

            foreach (var parameter in parameters)
                template.SetParameter(parameter.Key, parameter.Value);

            Uri = new Uri(template.Resolve());

            Depth = 0;
        }

        public int Depth { get; set; }

        public Uri Uri { get; set; }
    }
}