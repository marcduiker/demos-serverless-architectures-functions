using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ImageAnalysisApp.Functions
{
    public static class IsBlobAvailable
    {
        [FunctionName("IsBlobAvailable")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "HttpTriggerCSharp/BlobName/{blobName}")]HttpRequestMessage req, 
            string blobName, 
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, blobName);
        }
    }
}
