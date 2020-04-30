using System;
using System.Threading.Tasks;
using AdventureWorks.Services.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Types;

namespace AdventureWorks.Functions
{
    public class GetDocumentFunction
    {
        private readonly IDocumentService _documentService;

        public GetDocumentFunction(
            IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [FunctionName("GetDocumentFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string fileName = req.Query["name"];
            string nodeString = req.Query["node"].ToString();
            SqlHierarchyId? node = !string.IsNullOrWhiteSpace(nodeString)
                ? SqlHierarchyId.Parse(nodeString)
                : (SqlHierarchyId?) null;

            string rowguidString = req.Query["rowguid"];
            Guid? rowguid = !string.IsNullOrWhiteSpace(nodeString)
                ? Guid.Parse(nodeString)
                : (Guid?) null;

            var document = _documentService.Get(fileName, node, rowguid);

            if (document.DocumentData == null)
            {
                throw new Exception("Document is empty");
            }

            return new FileContentResult(document.DocumentData, "application/msword");
        }
    }
}
