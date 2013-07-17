using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Tavis;

namespace CrawlRunner.Crawler.Configuration
{
    public class CrawlSession
    {
        internal IEnumerable<Link> Links { get; private set; }
        internal IEnumerable<string> Authorities { get; private set; }
        internal IEnumerable<Parameters> Parameters { get; private set; }
        internal IDictionary<string, Action<HttpClient>> Contexts { get; private set; }

        internal CrawlSession(IEnumerable<Uri> rootUris)
        {
            Links = rootUris.Select(uri => new Link {Target = uri});
            Authorities = Links.Select(link => link.Target.Authority).Distinct();
            Parameters = Enumerable.Empty<Parameters>();
            Contexts = new Dictionary<string, Action<HttpClient>>();
        }

        public CrawlSession Including(params string[] whitelist)
        {
            Authorities = Authorities.Concat(whitelist.Select(uri => new Uri(uri).Authority).Distinct());

            return this;
        }

        public CrawlSession With(Contexts contexts)
        {
            foreach (var context in contexts)
                Contexts.Add(context.Key, context.Value);

            return this;
        }

        public CrawlSession With(params Parameters[] parameters)
        {
            Parameters = Parameters.Concat(parameters);

            return this;
        }

        public CrawlSession WithParameters(params object[] parameters)
        {
            return With(parameters.Select(param =>
                {
                    var properties = param.GetType().GetProperties();
                    return new Parameters(properties.ToDictionary(p => p.Name, p => p.GetValue(param)));
                }).ToArray());
        }
    }
}