using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace ImageUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder;
            if (args.Length == 0)
            {
                folder = ConfigurationManager.AppSettings.Get("LocalFolder");
            }
            else
            {
                folder = args[0];
            }

            var directoryInfo = new DirectoryInfo(folder);
            var fileUploader = new FileUploader();

            foreach (var fileInfo in directoryInfo.GetFiles("*.jpg"))
            {
                Console.WriteLine($"Start uploading {fileInfo.Name} to Azure...");
                fileUploader.Upload(fileInfo);
                Thread.Sleep(2000);
                Console.WriteLine($"Completed uploading {fileInfo.Name} to Azure.");
            }

            Console.WriteLine("Completed uploading.");
            Console.ReadLine();
        }
    }
}
