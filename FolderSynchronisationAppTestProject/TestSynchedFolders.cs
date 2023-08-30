using FolderSynchronisationApp;
using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FolderSynchronisationAppTestProject
{
    [TestFixture]
    public class TestSynchedFolders
    {
        [Test]
        public void EqualFileTest()
        {
            // Arrange
            var lifeTimeMock = new Mock<IHostApplicationLifetime>();
            var settingConfigMock = new Mock<IOptions<SettingConfig>>();

            // Construct the full file paths 

            string sourceFilePath = GetBaseSourcePath("TestFile1.txt");
            string replicaFilePath = GetBaseReplicaPath("TestFile1.txt");


            IFolderSynchronizer folderSynchronizer = new FolderSynchronizer(lifeTimeMock.Object, settingConfigMock.Object);

            // Act
            var result = folderSynchronizer.CompareFilesTest(sourceFilePath, replicaFilePath);


            // Assert
            Assert.IsTrue(result); // Check that the result is true 

        }

        [Test]
        public void NotEqualFileTest()
        {
            // Arrange
            var lifeTimeMock = new Mock<IHostApplicationLifetime>();
            var settingConfigMock = new Mock<IOptions<SettingConfig>>();

            // Construct the full file paths 

            string sourceFilePath = GetBaseSourcePath("TestFile2.txt");
            string replicaFilePath = GetBaseReplicaPath("TestFile2.txt");


            IFolderSynchronizer folderSynchronizer = new FolderSynchronizer(lifeTimeMock.Object, settingConfigMock.Object);

            // Act
            var result = folderSynchronizer.CompareFilesTest(sourceFilePath, replicaFilePath);
            // Assert
            Assert.IsFalse(result); // Check that the result is true 

        }


        string GetBaseSourcePath(string filename)
        {
            // Adjust the number of "../" to match your solution's folder structure
            string solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));
            string sourceFilePath = Path.Combine(solutionDirectory, "TestFilesRepo", "TestSourceFolder", filename);

            return sourceFilePath;
        }


        string GetBaseReplicaPath(string filename)
        {
            // Adjust the number of "../" to match your solution's folder structure
            string solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));
            string sourceFilePath = Path.Combine(solutionDirectory, "TestFilesRepo", "TestReplicaFolder", filename);

            return sourceFilePath;

        }
    }
}