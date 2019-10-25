using System;
using System.Threading;
using Hangfire;
using Hangfire.Logging;
using Hangfire.SqlServer;

namespace ConsoleApp13
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider(LogLevel.Debug)
                .UseSqlServerStorage("Database=consoleapp3;Integrated Security=true", new SqlServerStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(1)
                });

            for (var i = 0; i < 10_000; i++) BackgroundJob.Enqueue(() => Empty());

            using (new CustomBackgroundJobServer())
            {
                while (Console.ReadLine() != "exit")
                {
                    BackgroundJob.Enqueue(() => Thread.Sleep(5000));
                }
            }
        }

        public static void Empty()
        {
        }
    }
}
