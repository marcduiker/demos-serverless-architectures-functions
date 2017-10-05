using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ImageAnalysisApp.Functions
{
    public static class AnalyzeBlob
    {
        [FunctionName("AnalyzeBlob")]
        public static void Run(
            [BlobTrigger("images/{blobname}", Connection = "DefaultEndpointsProtocol=https;AccountName=imageanalysisappstorage;AccountKey=uhkAlqrbnZEQuNM6aKDULzDgkEV3NPMTO6bYA6tyemDxW5mlTPJNHch+tzpa4qW7dMcVVoUkI4I092lD1/1SFg==;EndpointSuffix=core.windows.net")]Stream myBlob,
            string blobname, 
            TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{blobname} \n Size: {myBlob.Length} Bytes");
        }
    }
}
