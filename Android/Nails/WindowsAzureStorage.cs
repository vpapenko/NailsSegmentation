using Android.Graphics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Nails
{
    public class WindowsAzureStorage
        : IStorage
    {
        public WindowsAzureStorage(StorageConfig Config)
        {
            this.Config = Config;
            InitCloudBlockBlob();
        }

        public readonly StorageConfig Config;
        CloudBlobContainer dataCloudBlobContainer;
        CloudBlobContainer imagesCloudBlobContainer;

        private void InitCloudBlockBlob()
        {
            if (CloudStorageAccount.TryParse(Config.StorageConnectionString, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                imagesCloudBlobContainer = cloudBlobClient.GetContainerReference(Config.ImagesContainerReference);
                dataCloudBlobContainer = cloudBlobClient.GetContainerReference(Config.DataContainerReference);
            }
        }


        public async Task SaveImage(Bitmap originalImage, Bitmap resultImage)
        {
            string id = Guid.NewGuid().ToString();
            string originalName = id + ".jpg";
            string resultName = id + ".result" + ".jpg";
            await SaveImage(originalImage, originalName);
            await SaveImage(resultImage, resultName);
        }

        public async Task<string> DownloadConfig()
        {
            var blob = dataCloudBlobContainer.GetBlockBlobReference(Config.ConfigName);
            var result = await blob.DownloadTextAsync();
            return result;
        }

        public async Task DownloadNewVersion(string path)
        {
            var blob = dataCloudBlobContainer.GetBlockBlobReference(Config.NewVersionName);
            await blob.DownloadToFileAsync(path, FileMode.Create);
        }

        private async Task SaveImage(Bitmap image, string fileName)
        {
            CloudBlockBlob cloudBlockBlob = imagesCloudBlobContainer.GetBlockBlobReference(fileName);

            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                image.Compress(Bitmap.CompressFormat.Png, 0, stream);
                bitmapData = stream.ToArray();
            }
            await cloudBlockBlob.UploadFromByteArrayAsync(bitmapData, 0, bitmapData.Length);
        }        
    }
}