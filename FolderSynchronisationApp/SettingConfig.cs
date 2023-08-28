using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FolderSynchronisationApp
{
    public class SettingConfig 
    {
        
        public string SourceFolderPath { get; set; }
        public string ReplicaFolderPath { get; set; }
        public string LogFolderPath { get; set; }
        public int SyncInterval { get;set; }
    }
}
