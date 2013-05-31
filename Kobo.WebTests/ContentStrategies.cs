using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kobo.WebTests
{
    public class ContentStrategies
    {
        private readonly IDictionary<string, Func<HttpContent, object>> strategies;
        private const string HtmlMediaType = "text/html";
        private const string JsonMediaType = "application/json";
        private const string XmlMediaType = "application/xml";

        public ContentStrategies()
        {
            strategies = new Dictionary<string, Func<HttpContent, object>>
                {
                    { HtmlMediaType, HtmlParser },
                    { JsonMediaType, JsonParser },
                    { XmlMediaType, XmlParser },
                };            
        }

        public Func<HttpContent, object> this[string mediaType]
        {
            get { return strategies[mediaType]; }
            set
            {
                if (strategies.ContainsKey(mediaType))
                    strategies[mediaType] = value;
                else
                    strategies.Add(mediaType, value);
            }
        }

        public bool StrategyExists(string mediaType)
        {
            return strategies.ContainsKey(mediaType);
        }

        public Func<HttpContent, object> Html
        {
            get { return strategies[HtmlMediaType]; }
            set { strategies[HtmlMediaType] = value; }
        }

        public Func<HttpContent, object> Json
        {
            get { return strategies[JsonMediaType]; }
            set { strategies[JsonMediaType] = value; }
        }

        public Func<HttpContent, object> Xml
        {
            get { return strategies[XmlMediaType]; }
            set { strategies[XmlMediaType] = value; }
        }

        private object XmlParser(HttpContent content)
        {
            var stream = content.ReadAsStreamAsync().Result;
            return XElement.Load(stream);
        }

        private object HtmlParser(HttpContent content)
        {
            var stream = content.ReadAsStreamAsync().Result;
            var result = new HtmlDocument();
            result.Load(stream);
            return result;
        }

        private object JsonParser(HttpContent content)
        {
            var stream = content.ReadAsStreamAsync().Result;
            var streamReader = new StreamReader(stream);
            var textReader = new JsonTextReader(streamReader);
            try
            {
                return JObject.Load(textReader);
            }
            catch
            {
                return JArray.Load(textReader);
            }
            finally
            {
                streamReader.Dispose();
            }
        }
    }
}