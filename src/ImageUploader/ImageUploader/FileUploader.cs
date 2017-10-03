using System;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ImageUploader
{
    public class FileUploader
    {
        private readonly CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _imagesBlobContainer;
        private readonly CloudQueueClient _queueClient;

        public FileUploader()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _blobClient = storageAccount.CreateCloudBlobClient();
            _imagesBlobContainer = _blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("BlobStorageContainer"));
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        public void Upload(FileInfo file)
        {
            var blobName = UploadToBlobStorage(file);
            if (IsBlobUploaded(blobName))
            {
                PushBlobInfoToQueue(blobName);
            }
        }

        private string UploadToBlobStorage(FileInfo file)
        {
            CloudBlockBlob blockBlob = _imagesBlobContainer.GetBlockBlobReference(file.Name);

            using (var fileStream = file.OpenRead())
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return file.Name;
        }

        private bool IsBlobUploaded(string blobName)
        {
            var blob = _imagesBlobContainer.GetBlockBlobReference(blobName);

            return blob.Exists();
        }

        private void PushBlobInfoToQueue(string blobInfo)
        {
            var imagesToProcessQueue = _queueClient.GetQueueReference(CloudConfigurationManager.GetSetting("QueueName"));
            var message = new CloudQueueMessage(blobInfo);
            imagesToProcessQueue.AddMessage(message);
        }
    }
}
