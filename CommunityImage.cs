using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using System.Net;

namespace FileUploadFunction
{
    public static class CommunityImage
    {
        [FunctionName("CommunityImage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "community")] HttpRequest req, ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("CommunityContainer");

            Stream myBlob = new MemoryStream();
            var file = req.Form.Files["image"];
            myBlob = file.OpenReadStream();
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient(Guid.NewGuid().ToString());
            await blob.UploadAsync(myBlob);


            return new OkObjectResult(blob.Uri.AbsoluteUri);

        }
    }
}
