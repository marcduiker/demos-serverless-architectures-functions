using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageAnalysisApp.Functions
{
    public static class PostFileToBlobStorage
    {
        [FunctionName("PostFileToBlobStorage")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PostFile/name/{fileName}")]HttpRequestMessage req, 
            string fileName,
            [Blob("images/{fileName}", FileAccess.ReadWrite, Connection = "StorageConnectionString")]Stream blob,
            [Queue("imagesinput", Connection = "StorageConnectionString")]ICollector<string> imagesInputQueue,
            TraceWriter log)
        {
            var fileContents = await req.Content.ReadAsByteArrayAsync();
            await blob.WriteAsync(fileContents, 0, fileContents.Length);

            imagesInputQueue.Add(fileName);

            return req.CreateResponse(HttpStatusCode.OK, $"Posted {fileName} to queue.");
        }
    }
}
