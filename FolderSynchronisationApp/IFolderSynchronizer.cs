using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronisationApp
{
    public interface IFolderSynchronizer
    {
        Task DoWork();
        //Task SyncFolders(string SourceFolderPath, string ReplicaFolderPath);
        bool CompareFilesTest(string sourceFilePath, string replicaFilePath);

        bool FileExists(string fileName);
    }
}
