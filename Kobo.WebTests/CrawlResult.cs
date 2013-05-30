using System;
using System.Net.Http;

namespace Kobo.WebTests
{
    public class CrawlResult
    {
        public CrawlResult(HttpRequestMessage request, HttpResponseMessage response, TimeSpan responseTime)
        {
            Uri = request.RequestUri;
            ResponseTime = responseTime;
            StatusCode = (int)response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
        }

        public string ReasonPhrase { get; set; }

        public int StatusCode { get; set; }

        public TimeSpan ResponseTime { get; set; }

        public Uri Uri { get; private set; }

        public object Content { get; private set; }

        public Type ContentType { get; private set; }

        public string MediaType { get; private set; }

        public void SetContent(HttpContent content, object parsedContent)
        {
            ContentType = parsedContent == null ? null : parsedContent.GetType();
            Content = parsedContent;
            MediaType = content.Headers.ContentType.MediaType;
        }
    }
}