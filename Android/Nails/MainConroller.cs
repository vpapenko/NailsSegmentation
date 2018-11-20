using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace Nails
{
    public class MainConroller
    {
        public MainConroller()
        {
            config = ConfigReader.LoadConfigFromAssets();
            versionsConroller = new VersionsConroller(config);
            pipeline = new TensorflowContribAndroidPipeline(Config.ModelName, config.ModelInputSize
                     , config.ModelInputName, config.ModelOutputName);
        }

        IPipeline pipeline;
        readonly ImageProcess imageProcess = new ImageProcess();
        readonly VersionsConroller versionsConroller;
        readonly Config config;

        public int ImageWidth { get; set; } = 320;
        public Bitmap OriginalImage { get; private set; }
        public Bitmap ResultImage { get; private set; }
        public bool ImageIsSent { get; private set; } = true;
        public TimeSpan PreprocessTime { get; private set; }
        public TimeSpan SegmentationTime { get; private set; }
        public TimeSpan PostprocessTime { get; private set; }

        public async Task<Bitmap> ProcessNewImage()
        { 
            Bitmap image = await GetImage();
            OriginalImage = image;

            DateTime time = DateTime.Now;
            float[] input = imageProcess.GetBitmapPixels(image);
            PreprocessTime = DateTime.Now - time;

            time = DateTime.Now;
            var output = pipeline.RecognizeImage(input);
            SegmentationTime = DateTime.Now - time;

            time = DateTime.Now;
            Bitmap result = imageProcess.BitmapPostrocess(image, output);
            ResultImage = result;
            ImageIsSent = false;
            PostprocessTime = DateTime.Now - time;

            return result;
        }

        public async Task SendImage()
        {
            IStorage storage = new WindowsAzureStorage(config.StorageConfig);
            if (OriginalImage != null && ResultImage != null)
            {
                await storage.SaveImage(OriginalImage, ResultImage);
                ImageIsSent = true;
            }
        }

        public async Task<bool> IsNewVersionAvailable()
        {
            return await versionsConroller.IsNewVersionAvailable();
        }

        public async Task DownloadNewVersion()
        {
            await versionsConroller.DownloadNewVersion();
            if (File.Exists(config.GetNewVersionPath()))
            {
                Intent promptInstall = new Intent(Intent.ActionView).SetDataAndType(Android.Net.Uri.Parse(config.GetNewVersionPath())
                    , "application/vnd.android.package-archive");
                Application.Context.StartActivity(promptInstall);
            }
        }

        private async Task<Bitmap> GetImage()
        {
            return await GetImageFromCamera();
            //return await GetTestImage();
        }

        private async Task<Bitmap> GetImageFromCamera()
        {
            Bitmap result = null;
            var image = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                SaveMetaData = false,
                SaveToAlbum = false,
                RotateImage = true,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = ImageWidth
            });
            if (image != null)
            {
                result = await BitmapFactory.DecodeStreamAsync(image.GetStreamWithImageRotatedForExternalStorage());
            }
            return result;
        }

        private async Task<Bitmap> GetTestImage()
        {
            return GetBitmapFromAsset("p1.jpg");
        }

        private static Bitmap GetBitmapFromAsset(string filePath)
        {
            var assetManager = Application.Context.Assets;

            System.IO.Stream istr;
            Bitmap bitmap = null;
            istr = assetManager.Open(filePath);
            bitmap = BitmapFactory.DecodeStream(istr);
            return bitmap;
        }        
    }
}