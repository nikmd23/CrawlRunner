﻿using System;

namespace CrawlRunner
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WidgetTestAttribute : Attribute
    {
        public WidgetTestAttribute(string cssSelector = null)
        {
            CssSelector = cssSelector;
        }

        public string CssSelector { get; set; }
    }
}