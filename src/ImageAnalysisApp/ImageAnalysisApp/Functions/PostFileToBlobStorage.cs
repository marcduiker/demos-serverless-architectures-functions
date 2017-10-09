using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageAnalysisApp.Functions
{
    public static class PostFileToBlobStorage
    {
        [FunctionName("PostFileToBlobStorage")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostFile/name/{fileName}")]HttpRequestMessage req, 
            string fileName,
            [Blob("images", FileAccess.ReadWrite, Connection = "StorageConnectionString")] out Stream blob,
            [Queue("imagesinput", Connection = "StorageConnectionString")]ICollector<string> imagesInputQueue,
            TraceWriter log)
        {
            using (blob = new MemoryStream())
            {
                var fileContents = req.Content.ReadAsByteArrayAsync().Result;
                blob.Write(fileContents, 0, fileContents.Length);
            }

            imagesInputQueue.Add(fileName);

            return req.CreateResponse(HttpStatusCode.OK, $"Posted {fileName} to queue.");
        }
    }
}
