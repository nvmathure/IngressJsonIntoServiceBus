using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HumanResource.BackgroundWorkerFuncApp
{
    public static class BackgroundWorker
    {
        [FunctionName("ProcessIncomingFile")]
        public static void ProcessIncomingFile(
            [BlobTrigger(
                "file-drop/{name}", 
                Connection = "AzureWebJobsStorage")]Stream blob, 
            string name, 
            ILogger log)
        {
            log.LogInformation($"Function ProcessIncomingFile received BLOB\n Name:{name} \n Size: {blob.Length} Bytes");
        }
    }
}
