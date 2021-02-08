using System;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Testing
{
    public abstract class TestBase
    {
        protected readonly ITestOutputHelper Console;
        protected Stopwatch Stopwatch;

        protected TestBase(ITestOutputHelper console)
        {
            Console = console;
        }

        /// <summary>
        ///     Provides a mechanism to enact Assert, Time and Report on any test action
        /// </summary>
        protected virtual Action<string, Action> Enact => (s, o) =>
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            try
            {
                o.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e} during: {s}");
                throw;
            }
            finally
            {
                Stopwatch.Stop();
                Console.WriteLine($"{s}, Took {Stopwatch.ElapsedMilliseconds} ms");
            }
        };
    }
}
