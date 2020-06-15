using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanResource.DataElements;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HumanResource.BackgroundWorkerFuncApp
{
    public static class ProcessIncomingFile
    {
        //[FunctionName("ProcessIncomingFileParallelAsync")]
        //public static async Task ParallelAsyncRun(
        //    [BlobTrigger(
        //        "file-drop/{name}",
        //        Connection = "AzureWebJobsStorage")]Stream blob,
        //    [ServiceBus(
        //        "requests",
        //        Connection = "AzureServiceBus")]IAsyncCollector<Employee> messages,
        //    [Blob(
        //        "logs/Parallel{name}",
        //        FileAccess.Write,
        //        Connection = "AzureWebJobsStorage")]Stream logStream,
        //    string name,
        //    ILogger log)
        //{
        //    await CoreProcessing(blob, messages, logStream, name, ParallelAsyncProcessing);
        //}

        [FunctionName("ProcessIncomingFileAsync")]
        public static async Task AsyncRun(
            [BlobTrigger(
                "file-drop/{name}",
                Connection = "AzureWebJobsStorage")]Stream blob,
            [ServiceBus(
                "requests",
                Connection = "AzureServiceBus")]IAsyncCollector<Employee> messages,
            [Blob(
                "logs/Async{name}",
                FileAccess.Write,
                Connection = "AzureWebJobsStorage")]Stream logStream,
            string name,
            ILogger log)
        {
            await CoreProcessing(blob, messages, logStream, name, AsyncProcessing);
        }

        private static async Task CoreProcessing(Stream blob, IAsyncCollector<Employee> messages, Stream logStream, string name, Action<IAsyncCollector<Employee>, List<Employee>> strategyAction)
        {
            // Record Start time and start the stop watch
            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Read Blob using StreamReader
            List<Employee> employees;
            using (var sr = new StreamReader(blob))
            {
                using var jtr = new JsonTextReader(sr);
                var serializer = new JsonSerializer()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                // Deserialize Stream to an Array
                employees = serializer.Deserialize<List<Employee>>(jtr);
            }

            strategyAction(messages, employees);

            await messages.FlushAsync();

            // Stop stopwatch and record end time
            stopwatch.Stop();
            var endTime = DateTime.UtcNow;

            // Write log information for analysis
            var logLine =
                $"\"{name}\",\"{blob.Length}\",\"{employees.Count}\",\"{stopwatch.ElapsedMilliseconds}\",\"{stopwatch.ElapsedTicks}\",\"{startTime:yyyy-MM-ddTH:mm:ss}\",\"{endTime:yyyy-MM-ddTH:mm:ss}\"";

            await using var sw = new StreamWriter(logStream, Encoding.UTF8);
            await sw.WriteAsync(logLine);
            await sw.FlushAsync();
        }

        private static void ParallelAsyncProcessing(IAsyncCollector<Employee> messages, List<Employee> employees)
        {
            // Add individual employees to Service Bus Queue
            Parallel.ForEach(employees, async e => await messages.AddAsync(e));
        }

        private static void AsyncProcessing(IAsyncCollector<Employee> messages, List<Employee> employees)
        {
            var tasks = employees.Select(employee => messages.AddAsync(employee)).ToArray();
            Task.WaitAll(tasks);
        }
    }
}
