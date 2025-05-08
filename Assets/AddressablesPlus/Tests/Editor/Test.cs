using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AddressablesPlus.Runtime;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace AddressablesPlus.Tests.Editor
{
    public class Test
    {
        [Test]
        public void TestInit()
        {
            var crurrVersionInfoPath = @"C:\Users\Avg\AppData\LocalLow\EriSeven\AddressablesPlus\1.5.43\files_info.txt";
            var currVersionInfoJson = Encoding.UTF8.GetString(File.ReadAllBytes(crurrVersionInfoPath));
            var currVersionInfo = JsonConvert.DeserializeObject<VersionedVirtualFileSystem.VersionInfo>(currVersionInfoJson);
            
            VersionedVirtualFileSystem.Initialize(currVersionInfo);
            
            Debug.Log("TestInit");
        }
    }
}