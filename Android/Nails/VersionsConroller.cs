using Android.App;
using System.IO;
using System.Threading.Tasks;

namespace Nails
{
    public class VersionsConroller
    {
        public VersionsConroller(Config config)
        {
            this.config = config;
            storage = new WindowsAzureStorage(config.StorageConfig);
        }

        readonly Config config;
        private readonly IStorage storage;

        public async Task<bool> IsNewVersionAvailable()
        {
            string content = await storage.DownloadConfig();
            Config storageConfig = ConfigReader.LoadConfig(content);

            if (config.VersionId != storageConfig.VersionId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task DownloadNewVersion()
        {
            await storage.DownloadNewVersion(config.GetNewVersionPath());
        }
    }
}