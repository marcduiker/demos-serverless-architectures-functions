using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ImageAnalysisApp.Functions
{
    public static class AnalyzeBlob
    {
        [FunctionName("AnalyzeBlob")]
        public static void Run(
            [BlobTrigger("images/{blobname}", Connection = "StorageConnectionString")]Stream myBlob,
            string blobname, 
            TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{blobname} \n Size: {myBlob.Length} Bytes");
        }
    }
}
