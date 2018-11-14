using System;
using System.IO;
using Android.App;
using Newtonsoft.Json;

namespace Nails
{
    public static class ConfigReader
    {
        public static Config LoadConfig()
        {
            Config result = null;
            var assetManager = Application.Context.Assets;
            using (StreamReader sr = new StreamReader(Config.ConfigPath))
            {
                var content = sr.ReadToEnd();
                result = LoadConfig(content);
            }

            return result;
        }

        public static void SaveConfig(Config config)
        {
            var content = JsonConvert.SerializeObject(config);
            using (var streamWriter = new StreamWriter(Config.ConfigPath, false))
            {
                streamWriter.Write(content);
            }
        }

        public static Config LoadConfigFromAssets()
        {
            Config result = null;
            var assetManager = Application.Context.Assets;
            using (StreamReader sr = new StreamReader(assetManager.Open(Config.ConfigName)))
            {
                var content = sr.ReadToEnd();
                result = LoadConfig(content);
            }

            return result;
        }

        public static Config LoadConfig(string content)
        {
            return JsonConvert.DeserializeObject<Config>(content);
        }
    }
}