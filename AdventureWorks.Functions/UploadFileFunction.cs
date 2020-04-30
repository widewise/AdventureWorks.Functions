using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdventureWorks.Services.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AdventureWorks.Functions
{
    public class UploadFileFunction
    {
        private readonly IFileStore _fileStore;

        public UploadFileFunction(
            IFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        [FunctionName("UploadFileFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            await _fileStore.Save("", req.Body);

            //var provider = new MultipartMemoryStreamProvider();
            //await req.Content.ReadAsMultipartAsync(provider);
            //var file = provider.Contents.First();
            //var fileInfo = file.Headers.ContentDisposition;
            //var fileData = await file.ReadAsByteArrayAsync();





            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
