using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageAnalysisApp.Functions
{
    public static class AnalyzeBlobSize
    {
        private const long MaxBlobSize = 4194304; // 4MB

        [FunctionName("AnalyzeBlobSize")]
        public static void Run(
            [QueueTrigger("imagesinput", Connection = "StorageConnectionString")]string blobNameInQueue,
            [Blob("images/{queueTrigger}", FileAccess.ReadWrite, Connection = "StorageConnectionString")]CloudBlockBlob blob,
            [Queue("imagestoolarge", Connection = "StorageConnectionString")]ICollector<string> imagesTooLarge,
            [Queue("imagestoprocess", Connection = "StorageConnectionString")]ICollector<string> imagesToProcess,
            TraceWriter log)
        {
            if (blob.Properties.Length < MaxBlobSize)
            {
                imagesToProcess.Add(blobNameInQueue);
            }
            else
            {
                log.Warning($"Image { blobNameInQueue } is too large and will be placed in the 'imagestoolarge' queue.");
                imagesTooLarge.Add(blobNameInQueue);
            }
        }
    }
}
