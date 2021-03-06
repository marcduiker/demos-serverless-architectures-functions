using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using ImageAnalysisApp.CognitiveServices;
using Newtonsoft.Json.Linq;

namespace ImageAnalysisApp.Functions
{
    public static class ImageAnalyzer
    {
        private const string StorageConnectionString = "StorageConnectionString";

        [FunctionName("ImageAnalyzer")]
        [return: Queue("analysisresultstostore", Connection = StorageConnectionString)]
        public static string Run(
            [QueueTrigger("imagestoanalyze", Connection = StorageConnectionString)]string blobNameInQueue, 
            [Blob("images/{queueTrigger}", FileAccess.Read, Connection = StorageConnectionString)]Stream blob,
            TraceWriter log)
        {
            var result = new JObject {{"file", blobNameInQueue}};

            log.Info("Started ImageAnalyzer function.");
            var computerVisionApiKey = CloudConfigurationManager.GetSetting("ComputerVisionApiKey");

            using (blob)
            {
                log.Info($"Starting computer vision analysis for {blobNameInQueue}...");
                byte[] image = ReadStream(blob);
                var computerVision = new ComputerVisionHandler(computerVisionApiKey);
                var analysisResult = computerVision.AnalyzeImage(image).Result;

                result.Add("computer vision", analysisResult);
                log.Info($"Completed computer vision analysis for {blobNameInQueue}.");
            }

            log.Info($"ImageAnalyzer completed for {blobNameInQueue}.");

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
