using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ImageAnalysisApp.Functions
{
    public static class ValidateBlobSize
    {
        private const string StorageConnectionString = "StorageConnectionString";
        private const long MaxBlobSize = 4194304; // 4MB

        [FunctionName("ValidateBlobSize")]
        public static void Run(
            [QueueTrigger("imagesinput", Connection = StorageConnectionString)]string blobNameInQueue,
            [Blob("images/{queueTrigger}", FileAccess.Read, Connection = StorageConnectionString)]Stream blob,
            [Queue("imagestoolarge", Connection = StorageConnectionString)]ICollector<string> imagesTooLarge,
            [Queue("imagestoanalyze", Connection = StorageConnectionString)]ICollector<string> imagesToProcess,
            ILogger log)
        {
            if (blob.Length < MaxBlobSize)
            {
                imagesToProcess.Add(blobNameInQueue);
            }
            else
            {
                log.Log(LogLevel.Warning, 0,  $"Image { blobNameInQueue } is too large and will be placed in the 'imagestoolarge' queue.", null, null);
                imagesTooLarge.Add(blobNameInQueue);
            }
        }
    }
}
