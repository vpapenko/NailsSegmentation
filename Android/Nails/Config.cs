﻿using System;
using System.IO;

namespace Nails
{
    public class Config
    {
        public Guid VersionId { get; set; }
        public StorageConfig StorageConfig { get; set; }
        public Guid ModelId { get; set; }
        public string ModelName { get; set; }
        public string NewVersionName { get; set; }
        
        public static readonly string ConfigName = "config.json";
        public static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ConfigName);

        public string GetModelPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ModelName);
        }
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
        public string ModelName { get; set; }
        public string NewVersionName { get; set; }
        public string ConfigName { get; set; }
    }
}