using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using ImageAnalysisApp.CognitiveServices;

namespace ImageAnalysisApp.Functions
{
    public static class ImageAnalyzer
    {
        [FunctionName("ImageAnalyzer")]
        [return: Queue("analysisresultstostore")]
        public static void Run(
            [QueueTrigger("imagestoprocess", Connection = "https://imageanalysisappstorage.queue.core.windows.net/")]string blobName, 
            ICollector<string> outputQueueItem, 
            TraceWriter log)
        {
            log.Info("Started ImageAnalyzer function.");
            var computerVisionApiKey = CloudConfigurationManager.GetSetting("ComputerVisionApiKey");
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var imagesBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("BlobStorageContainer"));

            var blob = imagesBlobContainer.GetBlockBlobReference(blobName);
            if (blob.Exists())
            {
                using (var stream = blob.OpenRead())
                {
                    byte[] image = ReadStream(stream);
                    var computerVision = new ComputerVisionHandler(computerVisionApiKey);
                    var analysisResult = computerVision.AnalyzeImage(image).Result;
                    outputQueueItem.Add(analysisResult.ToString(Formatting.Indented));
                }
            }
            else
            {
                log.Warning($"Can't find blob '{blobName}' in {imagesBlobContainer.Name}.");
            }

            log.Info($"ImageAnalyzer completed for {blobName}.");
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
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
