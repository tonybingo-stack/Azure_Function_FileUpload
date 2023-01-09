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
using Azure.Storage.Blobs.Models;

namespace FileUploadFunction
{
    public static class UserAvatar
    {
        [FunctionName("UserAvatar")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/avatar")] HttpRequest req, ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("UserContainer");

            Stream myBlob = new MemoryStream();
            var file = req.Form.Files["image"];
            myBlob = file.OpenReadStream();
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient(Guid.NewGuid().ToString());

            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = "image/jpeg";
            var o = new BlobUploadOptions();
            o.HttpHeaders = blobHttpHeader;

            await blob.UploadAsync(myBlob, o);

            return new OkObjectResult(blob.Uri.AbsoluteUri);
        }
    }
}
