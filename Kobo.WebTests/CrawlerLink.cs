using System;
using System.Collections.Generic;
using System.Linq;
using Tavis;

namespace Kobo.WebTests
{
    public class CrawlerLink : Link
    {
        public CrawlerLink(Uri uri) : this(uri, Enumerable.Empty<KeyValuePair<string, object>>())
        {
        }

        public CrawlerLink(Uri uri, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            Target = uri;

            foreach (var parameter in parameters)
            {
                SetParameter(parameter.Key, parameter.Value);
            }
        }
    }
}
