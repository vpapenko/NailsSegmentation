using Android.Graphics;
using System.Threading.Tasks;

namespace Nails
{
    interface IStorage
    {
        Task SaveImage(Bitmap originalImage, Bitmap resultImage);
        Task<string> DownloadConfig();
        Task DownloadModel(string path);
        Task DownloadNewVersion(string path);
    }
}