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
             

            // Adjust the number of "../" to match your solution's folder structure
            string solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));


            // Construct the full file paths 

            string sourceFilePath = Path.Combine(solutionDirectory,   "TestFilesRepo", "TestSourceFolder", "TestFile1.txt");
            string replicaFilePath = Path.Combine(solutionDirectory,  "TestFilesRepo", "TestReplicaFolder", "TestFile1.txt");  
             

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


            // Adjust the number of "../" to match your solution's folder structure
            string solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));


            // Construct the full file paths 

            string sourceFilePath = Path.Combine(solutionDirectory, "TestFilesRepo", "TestSourceFolder", "TestFile2.txt");
            string replicaFilePath = Path.Combine(solutionDirectory, "TestFilesRepo", "TestReplicaFolder", "TestFile2.txt");


            IFolderSynchronizer folderSynchronizer = new FolderSynchronizer(lifeTimeMock.Object, settingConfigMock.Object);

            // Act
            var result = folderSynchronizer.CompareFilesTest(sourceFilePath, replicaFilePath);


            // Assert
            Assert.IsFalse(result); // Check that the result is true 

        }
         

    }
}