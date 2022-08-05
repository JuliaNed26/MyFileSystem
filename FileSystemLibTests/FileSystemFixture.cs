namespace FileSystemLibTests
{
    public class FileSystemFixture
    {
        FileSystem testFileSystem;
        List<string> drives;

        [OneTimeSetUp]
        public void FillDriveNames()
        {
            drives = DriveInfo.GetDrives().Select(x => x.Name).ToList();
        }

        [SetUp]
        public void Setup()
        {
            testFileSystem = new FileSystem();
        }

        [Test]
        public void LibCreated_PathShouldBeEmpty_PathContentSouldBeDrivesName()
        {
            Assert.That(testFileSystem.CurrentPath, Is.EqualTo(""));
            CollectionAssert.AreEqual(testFileSystem.PathContent, drives);
        }

        [Test]
        public void GoToPath_GoToNonExistingFile_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => testFileSystem.GoToPath(Path.Combine(drives[0],"nonExistent")));
            Assert.Throws<FileNotFoundException>(() => testFileSystem.GoToPath(Path.Combine(drives[0], "nonExistent.txt")));
        }

        [Test]
        public void GoToPath_GoToExistingDirectory_PathAndPathContentShouldBeChanged()
        {
            List<string> prevPathContent = testFileSystem.PathContent;
            string pathOfADirectory = Directory.GetDirectories(drives[0])
                                      .Where(dir => !(new DirectoryInfo(dir)).IsSystemOrHidden())
                                      .FirstOrDefault();
            testFileSystem.GoToPath(pathOfADirectory);
            Assert.That(testFileSystem.CurrentPath, Is.EqualTo(pathOfADirectory));
            CollectionAssert.AreNotEquivalent(prevPathContent, testFileSystem.PathContent);
        }

        //go to file

        [Test]
        public void GoToPath_PathIsEmptyLine_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => testFileSystem.GoToPath(""));
        }

        [Test]
        public void ReturnToRootDir_ReturnFromDrive_PathContentShouldContainListOfDrives_PathShouldBeEmpty()
        {
            testFileSystem.GoToPath(drives[0]);
            testFileSystem.ReturnToRootDir();
            Assert.That(testFileSystem.CurrentPath, Is.EqualTo(""));
            CollectionAssert.AreEquivalent(testFileSystem.PathContent, drives);
        }

        [Test]
        public void ReturnToRootDir_ReturnFromListOfDrives_PathContentShouldContainListOfDrives_PathShouldBeEmpty()
        {
            testFileSystem.ReturnToRootDir(); 
            Assert.That(testFileSystem.CurrentPath, Is.EqualTo(""));
            CollectionAssert.AreEquivalent(testFileSystem.PathContent, drives);
        }

        [Test]
        public void ReturnToRootDir_ReturnFromTheDirectoryToParent_PathShouldChangeToParent_PathContentShouldBeChanged()
        {
            testFileSystem.GoToPath(drives[0]);

            string directoryToOpen = "";

            for (int i = 0; i < 2; i++)
            {

                directoryToOpen = Path.Combine(testFileSystem.CurrentPath,
                                         testFileSystem.PathContent
                                        .Where(item =>
                                        {
                                            string itemPath = Path.Combine(testFileSystem.CurrentPath, item);
                                            return FileSystemObject.IsDirectory(itemPath) &&
                                                 (new DirectoryInfo(itemPath)).GetDirectories()
                                                 .Where(dir => !dir.IsSystemOrHidden()).Any();
                                        }).FirstOrDefault());
                testFileSystem.GoToPath(directoryToOpen);
            }


            List<string> prevPathContent = testFileSystem.PathContent;

            testFileSystem.ReturnToRootDir();
            DirectoryInfo rootDir = (new DirectoryInfo(directoryToOpen)).Parent;
            Assert.That(testFileSystem.CurrentPath, Is.EqualTo(rootDir.FullName));
            CollectionAssert.AreNotEquivalent(testFileSystem.PathContent, prevPathContent);
        }
    }
}