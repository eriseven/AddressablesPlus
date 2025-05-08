using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AddressablesPlus.Runtime;
using UnityEngine;
using UnityEditor;

namespace AddressablesPlus.Editor
{
    public static class VersionInfoGenerator
    {
        [MenuItem("Assets/AddressablesPlus/Generate VersionInfo")]
        public static void Generate()
        {
            
        }
        
        private static Regex _regex = new Regex(@"^(\\|/)");
        static string GetRelativePath(string file, string path)
        {
            return _regex.Replace(file.Substring(path.Length), "");
        }
        
        static System.Security.Cryptography.MD5 _md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        static System.Text.StringBuilder _sb = new System.Text.StringBuilder();
 
        public static string MD5(this Stream stream)
        {
            try
            {
                byte[] retVal = _md5.ComputeHash(stream);
                _sb.Clear();
                for (int i = 0; i < retVal.Length; i++)
                {
                    _sb.Append(retVal[i].ToString("x2"));
                }

                return _sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }
                
        public static string MD5(this FileInfo f)
        {
            using (var fs = f.OpenRead()) { return fs.MD5(); }
        }

        
        public static VersionedVirtualFileSystem.VersionInfo Generate(string path, Version version = null)
        {
            if (version == null)
            {
                version = new Version(Application.version);
            }

            if (!Directory.Exists(path))
            {
                Debug.LogError("Directory does not exist: " + path); 
                return new VersionedVirtualFileSystem.VersionInfo() {version = version.ToString()};
            }

            var collected = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Where(f => !f.EndsWith(".meta") && !f.Contains(".DS_Store"))
                .Select(f =>
                {
                    FileInfo fi = new FileInfo(f);
                    var relativeFileName = GetRelativePath(f, path).Replace("\\", "/");
                    // bool optionalBundle = removeOptionalRes && relativeFileName.Contains("/optional/");
                    return new VersionedVirtualFileSystem.FileInfo()
                    {
                        fileName = relativeFileName,
                        // isUpdateFile = optionalBundle ? "1" : "0",
                        md5 = fi.MD5(),
                        size = fi.Length,
                    };
                });

            var versionInfo = new VersionedVirtualFileSystem.VersionInfo()
            {
                version = version.ToString(),
                files = collected.ToArray(),
            };

            return versionInfo;
        }
    }
}