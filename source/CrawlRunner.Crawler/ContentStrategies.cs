﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;
using CsQuery;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrawlRunner.Crawler
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
            return XElement.Load((Stream)stream);
        }

        private object HtmlParser(HttpContent content)
        {
            var stream = content.ReadAsStreamAsync().Result;
            return CQ.Create(stream);
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