using Android.App;
using System.IO;
using System.Threading.Tasks;

namespace Nails
{
    public class VersionsConroller
    {
        public VersionsConroller()
        {
            SetConfig();
            storage = new WindowsAzureStorage(Config.StorageConfig);
        }

        public Config Config;
        Config storageConfig = null;
        private readonly IStorage storage;

        public async Task<bool> IsNewModelAvailable()
        {
            Config storageConfig = await DownloadConfig();

            if (Config.ModelId != storageConfig.ModelId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task DownloadNewModel()
        {
            await storage.DownloadModel(Config.GetModelPath());
            Config storageConfig = await DownloadConfig();
            Config.ModelId = storageConfig.ModelId;
            ConfigReader.SaveConfig(Config);
        }

        public async Task<bool> IsNewVersionAvailable()
        {
            Config storageConfig = await DownloadConfig();

            if (Config.VersionId != storageConfig.VersionId)
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
            await storage.DownloadNewVersion(Config.GetModelPath());
        }

        private async Task<Config> DownloadConfig()
        {
            if (storageConfig == null)
            {
                string content = await storage.DownloadConfig();
                storageConfig = ConfigReader.LoadConfig(content);
            }
            return storageConfig;
        }

        private void SetConfig()
        {
            Config config;
            Config configFromAssets = ConfigReader.LoadConfigFromAssets();
            if (File.Exists(Config.ConfigPath))
            {
                config = ConfigReader.LoadConfig();

                if (config.ModelId != configFromAssets.ModelId)
                {
                    CopyModel(configFromAssets);
                }

                if (config.VersionId != configFromAssets.VersionId)
                {
                    ConfigReader.SaveConfig(configFromAssets);
                    config = configFromAssets;
                }
            }
            else
            {
                ConfigReader.SaveConfig(configFromAssets);
                config = configFromAssets;
            }

            Config = config;

            if (!File.Exists(config.GetModelPath()))
            {
                CopyModel(config);
            }
        }

        private void CopyModel(Config config)
        {
            using (var br = new BinaryReader(Application.Context.Assets.Open(config.ModelName)))
            {
                using (var bw = new BinaryWriter(new FileStream(config.GetModelPath(), FileMode.Create)))
                {
                    byte[] buffer = new byte[2048];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, length);
                    }
                }
            }
        }
    }
}