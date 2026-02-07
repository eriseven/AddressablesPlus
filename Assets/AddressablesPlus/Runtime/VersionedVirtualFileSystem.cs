using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gilzoide.EasyProjectSettings;
using Newtonsoft.Json;
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

        // public const string VersionFileName = "version.txt";
        public const string FilesInfoFileName = "files_info.txt";

        static private Dictionary<string, FileInfo> baseVersionFiles = new();
        static private Dictionary<string, FileInfo> currVersionFiles = new();

        public static void Initialize(VersionInfo versionInfo = null)
        {
            baseVersion = new Version(Application.version);
            basePath = Path.Combine(Application.streamingAssetsPath, baseVersion.ToString()).Replace('\\', '/');

            settings = ProjectSettings.Load<Settings>();

            BetterStreamingAssets.Initialize();

            try
            {
                var baseVersionInfo = JsonConvert.DeserializeObject<VersionInfo>(
                    Encoding.UTF8.GetString(BetterStreamingAssets.ReadAllBytes(FilesInfoFileName)));
                baseVersionFiles = baseVersionInfo.files.ToDictionary(x => x.fileName, x => x);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                baseVersionFiles = new();
                return;
            }


            if (versionInfo != null)
            {
                if (Version.TryParse(versionInfo.version, out var ver))
                {
                    currVersion = ver;
                    if (currVersion > baseVersion)
                    {
                        currVersionPath = Path.Combine(Application.persistentDataPath, ver.ToString())
                            .Replace('\\', '/');
                    }
                    else
                    {
                        currVersion = baseVersion;
                        currVersionPath = basePath;
                    }

                    currVersionFiles = versionInfo.files.ToDictionary(x => x.fileName, x => x);
                }
                else
                {
                    currVersion = baseVersion;
                    currVersionPath = basePath;
                }
            }
        }

        public static bool FileExists(string path)
        {
            bool exists = false;
            if (currVersion > baseVersion && currVersionFiles.ContainsKey(path))
            {
                exists = File.Exists(Path.Combine(currVersionPath, path));
            }

            if (!exists && baseVersionFiles.ContainsKey(path))
            {
                exists = BetterStreamingAssets.FileExists(path);
            }

            return exists;
        }

        public static byte[] ReadAllBytes(string path)
        {
            try
            {
                if (currVersion > baseVersion && currVersionFiles.ContainsKey(path))
                {
                    var versionPath = Path.Combine(currVersionPath, path);
                    if (File.Exists(versionPath))
                    {
                        return File.ReadAllBytes(Path.Combine(currVersionPath, path));
                    }
                }

                if (baseVersionFiles.ContainsKey(path))
                {
                    return BetterStreamingAssets.ReadAllBytes(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }

            return null;
        }

        public static Stream OpenRead(string path)
        {
            try
            {
                if (currVersion > baseVersion && currVersionFiles.ContainsKey(path))
                {
                    var versionPath = Path.Combine(currVersionPath, path);
                    if (File.Exists(versionPath))
                    {
                        return File.OpenRead(Path.Combine(currVersionPath, path));
                    }
                }

                if (baseVersionFiles.ContainsKey(path))
                {
                    return BetterStreamingAssets.OpenRead(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }

            return null;
        }
    }
}