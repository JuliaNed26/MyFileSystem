using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;

namespace FileSystemLibTests
{
    internal class FileSystemActionsFixture
    {
        MockFileSystem testFileSystem;
        FileSystemActions fileSystemActions;
        List<string> drives;
        private string startPath = "C:\\TestDirectory";

        [OneTimeSetUp]
        public void FillDriveNames()
        {
            drives = DriveInfo.GetDrives().Select(x => x.Name).ToList();
            testFileSystem = new MockFileSystem();
            fileSystemActions = new FileSystemActions(testFileSystem);
            testFileSystem.Directory.CreateDirectory(startPath);
        }

        [Test]
        public void CreateTxtFileWithName_ShouldCreateNewFile()
        {
            var nameOfCreatedFile = fileSystemActions.CreateTxtFileWithName(startPath); 
        }

        [Test]
        public void CreateFolderWithName_ShouldCreateNewFolder()
        {
            var nameOfCreatedFolder = fileSystemActions.CreateFolderWithName(startPath);
            Assert.IsTrue(testFileSystem.Directory.Exists(Path.Combine(startPath, nameOfCreatedFolder)));
        }

        [Test]
        public void Rename_RenameTxtFile_ShouldRenameExistingFile()
        {
            string newName = "newName";
            var nameOfCreatedFile = fileSystemActions.CreateTxtFileWithName(startPath);
            fileSystemActions.Rename(Path.Combine(startPath,nameOfCreatedFile), newName);
            Assert.IsTrue(testFileSystem.File.Exists(Path.Combine(startPath, newName + ".txt")));
            Assert.IsFalse(testFileSystem.File.Exists(Path.Combine(startPath, nameOfCreatedFile)));
        }

        [Test]
        public void Rename_RenameDirectory_ShouldRenameExistingDirectory()
        {
            string newName = "newName";
            var nameOfCreatedFolder = fileSystemActions.CreateFolderWithName(startPath);
            fileSystemActions.Rename(Path.Combine(startPath, nameOfCreatedFolder), newName);
            Assert.IsTrue(testFileSystem.Directory.Exists(Path.Combine(startPath, newName)));
            Assert.IsFalse(testFileSystem.Directory.Exists(Path.Combine(startPath, nameOfCreatedFolder)));
        }

        [Test]
        public void Delete_DeleteTxtFile_ShouldDeleteFile()
        {
            var nameOfCreatedFile = fileSystemActions.CreateTxtFileWithName(startPath);
            fileSystemActions.Delete(Path.Combine(startPath, nameOfCreatedFile));
            Assert.IsFalse(testFileSystem.File.Exists(Path.Combine(startPath, nameOfCreatedFile)));
        }

        [Test]
        public void Delete_DeleteDirectory_ShouldDeleteDirectory()
        {
            var nameOfCreatedFolder = fileSystemActions.CreateFolderWithName(startPath);
            fileSystemActions.Delete(Path.Combine(startPath, nameOfCreatedFolder));
            Assert.IsFalse(testFileSystem.File.Exists(Path.Combine(startPath, nameOfCreatedFolder)));
        }

        [Test]
        public void Rename_TryToRenameNotExistingDirectory_ShouldGetException()
        {
            Assert.Throws<FileNotFoundException>(() => fileSystemActions.Rename(Path.Combine(startPath,"notExistFolder"),"newName"));
        }
    }
}
