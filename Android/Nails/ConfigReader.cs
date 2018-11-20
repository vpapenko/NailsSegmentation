using System;
using System.IO;
using Android.App;
using Newtonsoft.Json;

namespace Nails
{
    public static class ConfigReader
    {
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