using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HumanResource.DataGenerator;
using Newtonsoft.Json.Serialization;

namespace HumanResource.BackgroundWorkerFuncApp
{
    public static class GenerateBogusDataFile
    {
        [FunctionName("GenerateBogusDataFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous, 
                "post", Route = null)]HttpRequest request,
            IBinder binder,
            [Blob(
                "logs/{name}",
                FileAccess.Write,
                Connection = "AzureWebJobsStorage")]Stream logStream,
            ILogger log)
        {

            var requestData = new GenerateBogusDataFileRequest();

            var data = EmployeeDataGenerator.GenerateData(requestData.SampleSize);
            using (var fileStream = await binder.BindAsync<TextWriter>(
                new BlobAttribute(
                    $"logs/{requestData.FileName}.json",
                    FileAccess.Write)
                {
                    Connection = "AzureWebJobsStorage"
                }))
            {
                fileStream.Write(
                    JsonConvert.SerializeObject(
                        data,
                        Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
            }

            return new OkObjectResult($"File created with {data.Count} records with name {requestData.FileName}.json");
        }
    }
}
