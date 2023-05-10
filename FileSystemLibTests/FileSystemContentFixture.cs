using System.IO.Abstractions;
using Moq;

namespace FileSystemLibTests
{
    public class FileSystemContentFixture
    { 
        private Mock<FileSystem> testFileSystem;
        private FileSystemContent fileSystemContent;
        private FileSystemActions fileSystemActions;
        private List<string> drives;

        [OneTimeSetUp]
        public void FillDrives()
        {
            drives = DriveInfo.GetDrives().Select(x => x.Name).ToList();
        }

        [SetUp]
        public void FillFileSystemClasses()
        {
            testFileSystem = new Mock<FileSystem>() { CallBase = true };
            fileSystemContent = new FileSystemContent(testFileSystem.Object);
            fileSystemActions = new FileSystemActions(testFileSystem.Object);
        }

        [Test]
        public void LibCreated_PathShouldBeEmpty_PathContentShouldBeDrivesName()
        {
            testFileSystem.CallBase = false;
            fileSystemContent = new FileSystemContent(testFileSystem.Object);
            Assert.That(fileSystemContent.CurrentPath, Is.EqualTo(""));
            CollectionAssert.AreEquivalent(fileSystemContent.PathContent, drives);
        }

        [Test]
        public void GoToPath_GoToNonExistingFile_ShouldThrowFileNotFoundException()
        {
            var notExistingDirPath = Path.Combine(drives[0], "nonExistent");
            var notExistingFilePath = Path.Combine(drives[0], "nonExistent.txt");
            testFileSystem.CallBase = false;
            testFileSystem.Setup(fs => fs.File.GetAttributes(notExistingDirPath)).Throws<FileNotFoundException>();
            testFileSystem.Setup(fs => fs.File.GetAttributes(notExistingFilePath)).Throws<FileNotFoundException>();
            Assert.Throws<FileNotFoundException>(() => fileSystemContent.GoToPath(notExistingDirPath));
            Assert.Throws<FileNotFoundException>(() => fileSystemContent.GoToPath(notExistingFilePath));
        }

        [Test]
        public void GoToPath_GoToExistingDirectory_PathAndPathContentShouldBeChanged()
        {
            List<string> prevPathContent = fileSystemContent.PathContent;
            string pathOfADirectory = Directory.GetDirectories(drives[0])
                                      .Where(dir => !(new DirectoryInfo(dir)).IsSystemOrHidden())
                                      .FirstOrDefault();
            fileSystemContent.GoToPath(pathOfADirectory);
            Assert.That(fileSystemContent.CurrentPath, Is.EqualTo(pathOfADirectory));
            CollectionAssert.AreNotEquivalent(prevPathContent, fileSystemContent.PathContent);
        }

        //go to file

        [Test]
        public void GoToPath_PathIsEmptyLine_ShouldThrowArgumentException()
        {
            testFileSystem.CallBase = false;
            testFileSystem.Setup(fs => fs.File.GetAttributes("")).Throws<ArgumentException>();
            Assert.Throws<ArgumentException>(() => fileSystemContent.GoToPath(""));
            testFileSystem.Verify(fs=> fs.File.GetAttributes(""),Times.Exactly(1));
        }

        [Test]
        public void ReturnToRootDir_ReturnFromDrive_PathContentShouldContainListOfDrives_PathShouldBeEmpty()
        {
            fileSystemContent.GoToPath(drives[0]);
            fileSystemContent.ReturnToRootDir();
            Assert.That(fileSystemContent.CurrentPath, Is.EqualTo(""));
            CollectionAssert.AreEquivalent(fileSystemContent.PathContent, drives);
        }

        [Test]
        public void ReturnToRootDir_ReturnFromListOfDrives_PathContentShouldContainListOfDrives_PathShouldBeEmpty()
        {
            fileSystemContent.ReturnToRootDir(); 
            Assert.That(fileSystemContent.CurrentPath, Is.EqualTo(""));
            CollectionAssert.AreEquivalent(fileSystemContent.PathContent, drives);
        }

        [Test]
        public void ReturnToRootDir_ReturnFromTheDirectoryToParent_PathShouldChangeToParent_PathContentShouldBeChanged()
        {
            fileSystemContent.GoToPath(drives[0]);

            string directoryToOpen = "";

            for (int i = 0; i < 2; i++)
            {

                directoryToOpen = Path.Combine(fileSystemContent.CurrentPath,
                                         fileSystemContent.PathContent
                                        .Where(item =>
                                        {
                                            string itemPath = Path.Combine(fileSystemContent.CurrentPath, item);
                                            return fileSystemActions.IsDirectory(itemPath) &&
                                                 (new DirectoryInfo(itemPath)).GetDirectories()
                                                 .Where(dir => !dir.IsSystemOrHidden()).Any();
                                        }).FirstOrDefault());
                fileSystemContent.GoToPath(directoryToOpen);
            }


            List<string> prevPathContent = fileSystemContent.PathContent;

            fileSystemContent.ReturnToRootDir();
            DirectoryInfo rootDir = (new DirectoryInfo(directoryToOpen)).Parent;
            Assert.That(fileSystemContent.CurrentPath, Is.EqualTo(rootDir.FullName));
            CollectionAssert.AreNotEquivalent(fileSystemContent.PathContent, prevPathContent);
        }
    }
}