using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gilzoide.EasyProjectSettings;
using UnityEngine;

namespace AddressablesPlus.Runtime
{
    public class VersionedVirtualFileSystem
    {
        [Serializable]
        public class FileInfo
        {
            public string fileName;
            public string md5;
            public long size;
        }

        [Serializable]
        public class VersionInfo
        {
            public string version = "1.0.0";
            public FileInfo[] files = new FileInfo[0];
        }

        private static Version baseVersion;
        private static string basePath;

        private static Version currVersion;
        private static string currVersionPath;

        private static Settings settings;

        private const string VersionFileName = "version.txt";
        private const string FilesInfoFileName = "files_info.txt";
        
        static private Dictionary<string, FileInfo> currVersionFiles = new(); 
        
        public static void Initialize(VersionInfo versionInfo = null)
        {
            settings = ProjectSettings.Load<Settings>();
            
            BetterStreamingAssets.Initialize();

            if (BetterStreamingAssets.FileExists(FilesInfoFileName))
            {
                
            }
            else
            {
                
            }
            
            
            baseVersion = new Version(Application.version);
            basePath = Path.Combine(Application.streamingAssetsPath, baseVersion.ToString()).Replace('\\', '/');
            
            
            if (versionInfo != null)
            {
                if (Version.TryParse(versionInfo.version, out var ver))
                {
                    currVersion = ver;
                    currVersionPath = Path.Combine(Application.persistentDataPath, ver.ToString()).Replace('\\', '/');
                    
                    // Load FileInfo
                }
                else
                {
                    currVersion = baseVersion;
                    currVersionPath = basePath;
                }
            }
        }
    }
}