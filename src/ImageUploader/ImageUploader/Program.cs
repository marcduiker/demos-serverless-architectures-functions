using System;
using System.IO;

namespace ImageUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            if (args.Length == 0)
            {
                filePath = @"C:\temp\testimage.png";
            }
            else
            {
                filePath = args[0];
            }

            var fileInfo = new FileInfo(filePath);
            var fileUploader = new FileUploader();
            Console.WriteLine($"Start uploading {fileInfo.Name} to Azure...");
            fileUploader.Upload(fileInfo);
            Console.WriteLine($"Completed uploading {fileInfo.Name} to Azure.");
        }
    }
}
