using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CrawlRunner.Crawler.Configuration
{
    // TODO: Make this available via a scriptcs script pack: http://blog.martindoms.com/2013/05/14/building-scriptcs-script-pack/
    public class CrawlerConfiguration
    {
        private readonly IEnumerable<CrawlSession> sessions;

        private CrawlerConfiguration(IEnumerable<CrawlSession> sessions)
        {
            this.sessions = sessions;
        }

        public static CrawlerConfiguration Configure(params CrawlSession[] sessions)
        {
            return new CrawlerConfiguration(sessions);
        }

        public override string ToString()
        {
            var builder = new StringBuilder("╤ Crawler");

            var sessionCount = sessions.Count();

            sessions.Each((session, index) =>
                {
                    var isLastSession = index == sessionCount - 1;
                    builder.AppendFormat("\n{1}┬ Session: {0}", index + 1, isLastSession ? "└" : "├");

                    var treeStack = new Stack<string>();

                    treeStack.Push(isLastSession ? " " : "│");
                    BuildContextsString(session, builder, treeStack);
                    treeStack.Pop();
                });

            return builder.ToString();
        }

        private void BuildContextsString(CrawlSession session, StringBuilder builder, Stack<string> treeStack)
        {
            var contexts = session.Contexts;
            if (!contexts.Any())
            {
                builder.AppendFormat("\n{0}", treeStack.Print());
                builder.Append("└┬ Context: <Default>");
                treeStack.Push(" ");
                BuildParametersString(session, builder, treeStack);
                treeStack.Pop();
            }
            else
            {
                var contextCount = contexts.Count();
                contexts.Each((context, index) =>
                    {
                        var isLastContext = index == contextCount - 1;

                        builder.AppendFormat("\n{0}", treeStack.Print());
                        builder.AppendFormat("{1}┬ Context: {0}", context.Key, isLastContext ? "└" : "├");
                        treeStack.Push(isLastContext ? " " : "│");
                        BuildParametersString(session, builder, treeStack);
                        treeStack.Pop();
                    });
            }
        }

        private void BuildParametersString(CrawlSession session, StringBuilder builder, Stack<string> treeStack)
        {
            var parameters = session.Parameters;

            if (!parameters.Any())
            {
                builder.AppendFormat("\n{0}", treeStack.Print());
                builder.Append("└┬ Parameters: <None>");
                treeStack.Push(" ");
                BuildUrisString(session, builder, treeStack);
                treeStack.Pop();
            }
            else
            {
                var parameterCount = parameters.Count();
                parameters.Each((parameter, index) =>
                    {
                        var isLastParameter = index == parameterCount - 1;

                        builder.AppendFormat("\n{0}", treeStack.Print());
                        builder.AppendFormat("{1}┬ Parameters: {0}", parameter, isLastParameter ? "└" : "├");

                        treeStack.Push(isLastParameter ? " " : "│");
                        BuildUrisString(session, builder, treeStack);
                        treeStack.Pop();
                    });
            }
        }

        private void BuildUrisString(CrawlSession session, StringBuilder builder, Stack<string> treeStack)
        {
            var lastLink = session.Links.Count()-1;
            session.Links.Each((link, index) =>
                {
                    var isLastLink = index == lastLink;

                    builder.AppendFormat("\n{0}", treeStack.Print());
                    builder.AppendFormat("{1}¬ {0}", link.GetResolvedTarget(), isLastLink ? "└" : "├");
                });
        }
    }

    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in enumerable) action(item, i++);
        }
    }

    public static class StackExtensions
    {
        public static string Print(this Stack<string> stack)
        {
            var sb = new StringBuilder();
            foreach (var item in stack.Reverse())
            {
                sb.Append(item);
            }
            return sb.ToString();
        }
    }
}