using System.Collections.Generic;
using System.Linq;

namespace CrawlRunner.Crawler.Configuration
{
    public class Parameters : Dictionary<string, object>
    {
        public Parameters(IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            foreach (var item in dictionary)
                Add(item.Key, item.Value);
        }

        public override string ToString()
        {
            return string.Join("&", this.Select(i => string.Format("{0}={1}", i.Key, i.Value)));
        }
    }
}