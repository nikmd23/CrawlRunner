using System;
using System.Collections.Generic;

namespace CrawlRunner
{
    public class ConsoleOutput
    {
        public ConsoleOutput()
        {
            Count = 0;
            Failures = new List<string>();
        }

        private int Count { get; set; }

        private List<string> Failures { get; set; }

        public void Display(IObservable<TestResult> results)
        {
            results.Subscribe(
                onNext: result =>
                    {
                        Console.WriteLine("\n{0}.{1} for {2} : {3}",
                                          result.Test.DeclaringType.FullName,
                                          result.Test.Name,
                                          result.Uri,
                                          result.Success ? "Success" : "Failed");

                        if (!result.Success)
                        {
                            Console.Error.WriteLine(result.Exception);
                            Failures.Add(string.Format("\n{0}.{1} for {2}\n{3}",
                                                       result.Test.DeclaringType.FullName,
                                                       result.Test.Name,
                                                       result.Uri.AbsoluteUri,
                                                       result.Exception.Message));
                        }

                        Count++;
                    },
                onError: WriteMessage,
                onCompleted: WriteMessage);
        }

        private void WriteMessage()
        {
            WriteMessage(null);
        }

        private void WriteMessage(Exception exception)
        {
            if (exception != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                try
                {
                    throw exception;
                }
                catch (OperationCanceledException ex)
                {
                    Console.Out.WriteLine("\nCRAWL CANCELED BY USER");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("\nEXCEPTION: {0}", exception);
                }
            }

            if (Failures.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Error.WriteLine("\nTESTS FAILED ({0} passed/{1} total)", Count - Failures.Count, Count);
                Failures.ForEach(Console.Error.WriteLine);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Out.WriteLine("\nTESTS SUCCEEDED ({0} passed/{1} total)", Count, Count);
            }
        }
    }
}