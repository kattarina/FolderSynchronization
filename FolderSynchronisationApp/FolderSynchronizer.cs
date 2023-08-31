using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FolderSynchronisationApp
{
    public class FolderSynchronizer : IFolderSynchronizer
    {

        public IHostApplicationLifetime _lifeTime;
        private readonly string APPSRV = "{FolderSyncApplication}";
        private readonly SettingConfig _settingConfig;

        public FolderSynchronizer(IHostApplicationLifetime lifeTime, IOptions<SettingConfig> settingConfig)
        {
            _lifeTime = lifeTime;
            _settingConfig = settingConfig.Value;
        }




        public async Task DoWork()
        {
            string PROCNAME = "DoWork";
            Log.Logger.Information($"{APPSRV} {{{PROCNAME}}}");

            await Task.Run(SyncFolder);
        }

        //protected async Task SyncFolder()
        //{
        //    await CheckFolder();
        //}

        private async Task SyncFolder()
        {
            try
            {

                // Check if the folder still exists
                if (!DirectoryExists(_settingConfig.SourceFolderPath))
                {
                   
                    CreateDirectory(_settingConfig.SourceFolderPath);
                }

                if (!DirectoryExists(_settingConfig.ReplicaFolderPath))
                {
                   
                    CreateDirectory(_settingConfig.ReplicaFolderPath);
                }

                SyncFolders(_settingConfig.SourceFolderPath, _settingConfig.ReplicaFolderPath);
            }
            catch (Exception ex)
            {
                Log.Logger.Information($"An error occurred: {ex.Message}");
            }
        }

        private void SyncFolders(string sourcePath, string replicaPath)
        {
            string[] sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            string[] replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);


            //check if there are any extra files in replica, which are not in source
            if (replicaFiles.Length > 0)
            {
                CheckAndDeleteExtraFiles(sourceFiles, replicaFiles, sourcePath, replicaPath);
            }
            //Check Source Folder and copy files to replica
            foreach (string sourceFile in sourceFiles)
            {
                string relativePath = sourceFile.Substring(sourcePath.Length).TrimStart('\\');
                string replicaFile = Path.Combine(replicaPath, relativePath);

                // Log file operations

                try
                {
                    if (!FileExists(replicaFile))
                    {
                        CreateDirectory(Path.GetDirectoryName(replicaFile));
                        FileCopy(sourceFile, replicaFile);

                    }
                    else
                    {

                        bool filesAreEqual = CompareFiles(sourceFile, replicaFile);

                        if (!filesAreEqual)
                        {
                            FileCopy(sourceFile, replicaFile);
                        }


                        //using (var sourceStream = File.OpenRead(sourceFile))
                        //using (var replicaStream = File.OpenRead(replicaFile))
                        //{
                        //    using (var sourceMd5 = MD5.Create())
                        //    using (var replicaMd5 = MD5.Create())
                        //    {
                        //        byte[] sourceHash = sourceMd5.ComputeHash(sourceStream);
                        //        byte[] replicaHash = replicaMd5.ComputeHash(replicaStream);

                        //        if (!StructuralComparisons.StructuralEqualityComparer.Equals(sourceHash, replicaHash))
                        //        {
                        //            File.Copy(sourceFile, replicaFile, true);
                        //            logMessage = string.Concat(logMessage, "Updated: ", sourceFile, " -> ", replicaFile);
                        //        }
                        //    }
                        //}
                    }

                }
                catch (Exception ex)
                {
                    string logMessage = string.Concat(DateTime.Now.ToString(), " - Error: ", ex.Message);
                    Log.Logger.Information($"file log:  {logMessage}");
                }

                // Append log  


                //if (logMessage.LastIndexOf("-") != -1 && logMessage.LastIndexOf("-") + 1 < logMessage.Length)
                //{
                //    if (string.IsNullOrWhiteSpace(logMessage.Substring(logMessage.LastIndexOf("-") + 1)))
                //    {
                //        logMessage = string.Concat(logMessage, "No difference found!");
                //    }
                //}
                //else
                //{
                //    logMessage = string.Concat(logMessage, "No differences found!");
                //}

                // Log.Logger.Information($"file log:  {logMessage}");
            }

        }


        private void CheckAndDeleteExtraFiles(string[] sourceFiles, string[] replicaFiles, string sourcePath, string replicaPath)
        {


            HashSet<string> sourceFileNames = new HashSet<string>(sourceFiles.Select(f => Path.GetFileName(f)));
            HashSet<string> replicaFileNames = new HashSet<string>(replicaFiles.Select(f => Path.GetFileName(f)));


            //check if there are any files in replica, while source folder is empty
            if (sourceFiles.Length == 0)
            {
                Log.Logger.Information($"file log: {DateTime.Now.ToString()} - No files found! ");


                if (replicaFiles.Length != 0)
                {
                    foreach (string replicaFile in replicaFiles)
                    {
                        FileDelete(replicaFile);
                    }
                }

            }
            else
            {
                string[] filesToDelete = replicaFileNames.Where(file => !sourceFileNames.Contains(file)).ToArray();

                foreach (string file in filesToDelete)
                {
                    string fullPathToDelete = Path.Combine(replicaPath, file);
                    FileDelete(fullPathToDelete);
                }
            }
        }

      
        static bool CompareFiles(string sourceFilePath, string replicaFilePath)
        {
            using (FileStream fs1 = File.OpenRead(sourceFilePath))
            using (FileStream fs2 = File.OpenRead(replicaFilePath))
            {
                long fileSize = fs1.Length;
                if (fileSize != fs2.Length)
                {
                    return false; // Files are of different sizes
                }

                int bufferSize = 64 * 1024;
                byte[] buffer1 = new byte[bufferSize];
                byte[] buffer2 = new byte[bufferSize];

                bool filesAreEqual = true;
                long bytesRemaining = fileSize;

                while (bytesRemaining > 0)
                {
                    int bytesRead1 = fs1.Read(buffer1, 0, bufferSize);
                    int bytesRead2 = fs2.Read(buffer2, 0, bufferSize);

                    if (bytesRead1 != bytesRead2 || !BuffersEqual(buffer1, buffer2, bytesRead1))
                    {
                        filesAreEqual = false;
                        break;
                    }

                    bytesRemaining -= bytesRead1;
                }

                return filesAreEqual;
            }
        }

        static bool BuffersEqual(byte[] buffer1, byte[] buffer2, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    return false;
                }
            }
            return true;
        }


        public bool CompareFilesTest(string sourceFilePath, string replicaFilePath)
        {
            return CompareFiles(sourceFilePath, replicaFilePath);
        }


        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public void FileCopy(string sourceFilePath, string replicaFilePath)
        {
            string logMessage = DateTime.Now.ToString() + " - ";
            try
            {
                File.Copy(sourceFilePath, replicaFilePath, true);
                logMessage = string.Concat(logMessage, "Copied: ", sourceFilePath, " -> ", replicaFilePath);
            }
            catch (Exception ex)
            {
                logMessage = String.Concat(logMessage, "Error: ", ex.Message);
            }
            Log.Logger.Information($"file log:  {logMessage}");
        }

        public void FileDelete(string FilePath)
        {
            string logMessage = DateTime.Now.ToString() + " - ";
            try
            {
                File.Delete(FilePath);
                logMessage = String.Concat(logMessage, "Deleted: ", FilePath);

            }
            catch (Exception ex)
            {
                logMessage = String.Concat(logMessage, "Error: ", ex.Message);
            }

            Log.Logger.Information($"file log:  {logMessage}");

        }

        public bool DirectoryExists(string DirectoryName)
        {
            bool result = false;
            if (Directory.Exists(DirectoryName))
            {
               // Log.Logger.Information($"The folder '{DirectoryName}' exists in the system");
                result = true;
            }
            else
            {
                Log.Logger.Information($"The folder '{DirectoryName}' does not exist in the system");
            }

            return result;

        }

        public void CreateDirectory(string DirectoryName)
        {

            try
            {
                DirectoryInfo di = Directory.CreateDirectory(DirectoryName);
                Log.Logger.Information($"The folder '{DirectoryName}' is created  in the system");
            }
            catch (Exception ex)
            {

                Log.Logger.Information($"The folder '{DirectoryName}' is not created  in the system! Error appeared: {ex.Message}");
            }
        }

        static string GetFolderName(string fullPath)
        {
            string directoryName = Path.GetDirectoryName(fullPath);
            string folderName = Path.GetFileName(directoryName);

            return folderName;
        }

    }


}
