using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageAnalysisApp
{
    public static class ImageAnalyzer
    {
        [FunctionName("ImageAnalyzer")]
        public static void Run(
            [QueueTrigger("imagestoprocess", Connection = "https://imageanalysisappstorage.queue.core.windows.net/")]string blobName, 
            [Queue("analysisresultstostore", Connection = "https://imageanalysisappstorage.queue.core.windows.net/")]ICollector<string> outputQueueItem, 
            TraceWriter log)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var imagesBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("BlobStorageContainer"));

            var blob = imagesBlobContainer.GetBlockBlobReference(blobName);
            if (blob.Exists())
            {
                using (var stream = blob.OpenRead())
                {
                    byte[] image = ReadStream(stream);
                    var computerVision = new ComputerVisionHandler();
                    var analysisResult = computerVision.AnalyzeImage(image).Result;
                    outputQueueItem.Add(analysisResult.ToString(Formatting.Indented));
                }
            }
            else
            {
                log.Info($"Can't find blob '{blobName}' in {imagesBlobContainer.Name}.");
            }

            log.Info($"ImageAnalyzer completed for {blobName}.");
        }

        private static void PushAnalysisInfoToQueue(JToken analysisResult, CloudQueueClient queueClient)
        {
            var imagesToProcessQueue = queueClient.GetQueueReference(CloudConfigurationManager.GetSetting("QueueName"));
            var message = new CloudQueueMessage(analysisResult.ToString(Formatting.Indented));
            imagesToProcessQueue.AddMessage(message);
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
