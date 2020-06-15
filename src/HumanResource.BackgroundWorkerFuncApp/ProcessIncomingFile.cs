using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        [FunctionName("ProcessIncomingFile")]
        public static async Task Run(
            [BlobTrigger(
                "file-drop/{name}",
                Connection = "AzureWebJobsStorage")]Stream blob,
            [ServiceBus(
                "requests",
                Connection = "AzureServiceBus")]IAsyncCollector<Employee> messages,
            [Blob(
                "logs/{name}",
                FileAccess.Write,
                Connection = "AzureWebJobsStorage")]Stream logStream,
            string name,
            ILogger log)
        {
            /// Record Start time and start the stop watch
            var startTime = DateTime.UtcNow;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Read Blob using StreamReader
            List<Employee> employees;
            using (var sr = new StreamReader(blob))
            {
                using var jtr = new JsonTextReader(sr);
                JsonSerializer serializer = new JsonSerializer()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                // Deserialize Stream to an Array
                employees = serializer.Deserialize<List<Employee>>(jtr);
            }

            // Add individual employees to Service Bus Queue
            Parallel.ForEach(employees, async e => await messages.AddAsync(e));

            await messages.FlushAsync();

            // Stop stopwatch and record end time
            stopwatch.Stop();
            var endTime = DateTime.UtcNow;

            // Write log information for analysis
            var logline = $"\"{name}\",\"{blob.Length}\",\"{employees.Count}\",\"{stopwatch.ElapsedMilliseconds}\",\"{stopwatch.ElapsedTicks}\",\"{startTime:yyyy-MM-ddTH:mm:ss}\",\"{endTime:yyyy-MM-ddTH:mm:ss}\"";

            using var sw = new StreamWriter(logStream, Encoding.UTF8);
            await sw.WriteAsync(logline);
            await sw.FlushAsync();

        }
    }
}
