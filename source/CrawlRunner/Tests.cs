using CrawlRunner.Crawler;
using CsQuery;
using Newtonsoft.Json.Linq;
using PowerAssert;

namespace CrawlRunner
{
    public class Tests
    {
        [WebTest("*.twitter.com*")]
        public void TwitterTest(CrawlResult result, JObject content)
        {
            PAssert.IsTrue(() => true);
        }

        [WebTest]
        public void HtmlTest(CrawlResult result, CQ content)
        {
            PAssert.IsTrue(() => true);
        }

        [WebTest]
        public void AllPagesTest(CrawlResult result)
        {
            PAssert.IsTrue(() => true);
        }


        [WidgetTest("#nav")]
        public void WidgetIdTest(CrawlResult result, IDomElement widget)
        {
            PAssert.IsTrue(() => "nav".Equals(widget.GetAttribute("id")));
        }
    }
}
