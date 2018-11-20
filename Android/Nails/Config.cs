using System;
using System.IO;

namespace Nails
{
    public class Config
    {
        public Guid VersionId { get; set; }
        public StorageConfig StorageConfig { get; set; }
        public int ModelInputSize { get; set; }
        public string ModelInputName { get; set; }
        public string ModelOutputName { get; set; }
        public string NewVersionName { get; set; }

        public static readonly string ConfigName = "config.json";
        public static readonly string ModelName = "model.pb";

        public string GetNewVersionPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), NewVersionName);
        }
    }

    public class StorageConfig
    {
        public string StorageConnectionString { get; set; }
        public string DataContainerReference { get; set; }
        public string ImagesContainerReference { get; set; }
        public string NewVersionName { get; set; }
        public string ConfigName { get; set; }
    }
}