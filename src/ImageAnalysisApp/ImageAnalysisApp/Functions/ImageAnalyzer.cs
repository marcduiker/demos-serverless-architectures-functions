using System;
using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using ImageAnalysisApp.CognitiveServices;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace ImageAnalysisApp.Functions
{
    public static class ImageAnalyzer
    {
        [FunctionName("ImageAnalyzer")]
        [return: Queue("analysisresultstostore", Connection = "StorageConnectionString")]
        public static string Run(
            [QueueTrigger("imagestoprocess", Connection = "StorageConnectionString")]string blobName, 
            [Blob("images/{blobname}", Connection = "StorageConnectionString")]CloudBlockBlob blob,
            TraceWriter log)
        {
            var result = new JObject {{"file", blobName}};

            log.Info("Started ImageAnalyzer function.");
            var computerVisionApiKey = CloudConfigurationManager.GetSetting("ComputerVisionApiKey");

            if (blob.Exists())
            {
                using (var stream = blob.OpenRead())
                {
                    byte[] image = ReadStream(stream);
                    log.Info($"Starting computer vision analysis for {blobName}....");
                    var computerVision = new ComputerVisionHandler(computerVisionApiKey);
                    var analysisResult = computerVision.AnalyzeImage(image).Result;
                    result.Add("computer vision", analysisResult);
                    log.Info($"Completed computer vision analysis for {blobName}.");
                }
            }
            else
            {
                log.Warning($"Can't find blob '{blobName}' in {blob.Container.Name}.");
            }
            
            log.Info($"ImageAnalyzer completed for {blobName}.");

            return result.ToString(Formatting.Indented);
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
