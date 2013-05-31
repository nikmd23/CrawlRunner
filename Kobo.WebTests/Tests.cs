using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using PowerAssert;

namespace Kobo.WebTests
{
    public class Tests
    {
        [WebTest("*.twitter.com*")]
        public void ATest(CrawlResult result, JObject content)
        {
            PAssert.IsTrue(() => true);
        }

        [WebTest]
        public void BTest(CrawlResult result, HtmlDocument content)
        {
            PAssert.IsTrue(() => true);
        }

        [WebTest]
        public void CTest(CrawlResult result)
        {
            PAssert.IsTrue(() => true);
        }
    }
}
