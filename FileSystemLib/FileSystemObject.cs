using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemLib
{
    public static class FileSystemObject
    {
        public static bool IsDirectory(string path) => (File.GetAttributes(path) & FileAttributes.Directory) != 0;

        public static void MoveManyTo(List<string> pathsOfObjectsToMove, string destination)
        {

            if (Directory.Exists(destination))
            {
                foreach (string path in pathsOfObjectsToMove)
                {
                    if (IsDirectory(path))
                    {
                        Directory.Move(path, Path.Combine(destination, (new DirectoryInfo(path)).Name));
                    }

                    else
                    {
                        File.Move(path, Path.Combine(destination, Path.GetFileName(path)));
                    }
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {destination} wasn't found");
            }
        }

        public static void CopyManyTo(List<string> pathsOfObjectsToCopy, string destination)
        {

            if (Directory.Exists(destination))
            {
                foreach (string path in pathsOfObjectsToCopy)
                {
                    if (IsDirectory(path))
                    {
                        CopyDir(path, destination);
                    }

                    else
                    {
                        File.Copy(path, Path.Combine(destination, Path.GetFileName(path)));
                    }
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {destination} wasn't found");
            }

            static void CopyDir(string dirPath, string destination)
            {
                destination = Path.Combine(destination, (new DirectoryInfo(dirPath)).Name);
                Directory.CreateDirectory(destination);
                List<string> allFiles = Directory.GetFiles(dirPath).ToList();
                allFiles.AddRange(Directory.GetDirectories(dirPath).ToList());
                CopyManyTo(allFiles, destination);
            }
        }

        public static void Delete(string path)
        {
            var IsSystemDirectory = (string path) => ((new DirectoryInfo(path)).Attributes & FileAttributes.System) != 0;
            var IsSystemFile = (string path) => ((new FileInfo(path)).Attributes & FileAttributes.System) != 0;

            if (File.Exists(path) && !IsSystemFile(path))
            {
                File.Delete(path);
            }

            if(Directory.Exists(path) && !IsSystemDirectory(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static string GetProperties(string path)
        {
            if (IsDirectory(path))
            {
                if ((new DirectoryInfo(path)).Parent == null)//drive
                {
                    return GetDriveProperties(path);
                }
                return GetDirectoryProperties(path);
            }

            else
            {
                return GetFileProperties(path);
            }

            static string GetDriveProperties(string drive)
            {
                DriveInfo driveInfo = DriveInfo.GetDrives().Where(d => d.Name == drive).FirstOrDefault();

                if(driveInfo == null)
                {
                    throw new DriveNotFoundException();
                }

                StringBuilder propertiesMessage = new StringBuilder();
                if (driveInfo.IsReady)
                {
                    propertiesMessage.Append($"File system : {driveInfo.DriveFormat}\n\n");
                    propertiesMessage.Append($"Used space : {driveInfo.TotalSize - driveInfo.TotalFreeSpace} bytes\n\n");
                    propertiesMessage.Append($"Free space : {driveInfo.TotalFreeSpace} bytes\n\n");
                    propertiesMessage.Append($"Capacity : {driveInfo.TotalSize} bytes\n\n");
                }
                else
                {
                    propertiesMessage.Append($"File system : Unknown\n\n");
                    propertiesMessage.Append($"Used space : 0 bytes\n\n");
                    propertiesMessage.Append($"Free space : 0 bytes\n\n");
                    propertiesMessage.Append($"Capacity : 0 bytes\n\n");
                }

                return propertiesMessage.ToString();
            }

            static string GetFileProperties(string path)
            {
                var fileInfo = new FileInfo(path);
                StringBuilder propertiesMessage = new StringBuilder();
                propertiesMessage.Append($"Name : {fileInfo.Name}\n\n");
                propertiesMessage.Append($"Type of file : {fileInfo.Extension}\n\n");
                propertiesMessage.Append($"Location : {fileInfo.Directory}\n\n");
                propertiesMessage.Append($"Size : {fileInfo.Length} bytes\n\n");
                propertiesMessage.Append($"Created : {fileInfo.CreationTime}\n\n");
                propertiesMessage.Append($"Modified : {fileInfo.LastWriteTime}\n\n");
                propertiesMessage.Append($"Accessed : {fileInfo.LastAccessTime}\n\n");

                return propertiesMessage.ToString();
            }

            static string GetDirectoryProperties(string path)
            {
                var dirInfo = new DirectoryInfo(path);
                StringBuilder propertiesMessage = new StringBuilder();
                propertiesMessage.Append($"Name : {dirInfo.Name}\n\n");
                propertiesMessage.Append($"Type of file : File folder\n\n");
                propertiesMessage.Append($"Location : {dirInfo.Parent?.FullName}\n\n");
                propertiesMessage.Append($"Size : {CalculateDirSize(path)} bytes\n\n");
                propertiesMessage.Append($"Created : {dirInfo.CreationTime}\n\n");
                propertiesMessage.Append($"Modified : {dirInfo.LastWriteTime}\n\n");
                propertiesMessage.Append($"Accessed : {dirInfo.LastAccessTime}\n\n");
                if((dirInfo.Attributes & FileAttributes.ReadOnly) != 0)
                {
                    propertiesMessage.Append("ReadOnly ");
                }
                if ((dirInfo.Attributes & FileAttributes.Hidden) != 0)
                {
                    propertiesMessage.Append("Hidden");
                }

                return propertiesMessage.ToString();

                static double CalculateDirSize(string path)
                {
                    double size = 0;

                    try
                    {
                        Directory.GetDirectories(path)
                                 .Select(directory =>
                                 {
                                     size += CalculateDirSize(directory);
                                     return directory;
                                 }).ToArray();

                        Directory.GetFiles(path).Where(file => ((new FileInfo(file)).Attributes & FileAttributes.System) == 0)
                                                .Select(file =>
                                                {
                                                    size += (new FileInfo(file)).Length;
                                                    return file;
                                                }).ToArray();
                    }

                    catch(UnauthorizedAccessException)
                    {
                        return 0;
                    }

                    return size;
                }
            }
        }

        public static void Rename(string path, string newName)
        {
            if (newName != null && newName != "")
            {
                if (IsDirectory(path) && newName != (new DirectoryInfo(path)).Name)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    string newDirectoryPath = Path.Combine(dirInfo.Parent.ToString(), newName);
                    Directory.CreateDirectory(newDirectoryPath);
                    List<string> dirContent = dirInfo.GetDirectories()
                                                 .Select(directory => directory.FullName)
                                                 .ToList();
                    dirContent.AddRange(dirInfo.GetFiles().Select(file => file.FullName));
                    MoveManyTo(dirContent, newDirectoryPath);
                    Delete(path);
                }
                if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    File.Move(path,
                        Path.Combine(fileInfo.Directory.FullName, newName + (newName.Contains(fileInfo.Extension) ? "" : fileInfo.Extension)),
                        false);
                }
            }
        }

        public static string CreateTxtFileWithName(string destinationPath)
        {
            DirectoryInfo di = new DirectoryInfo(destinationPath);
            int countOfNewTxtFiles = di.EnumerateFiles()
                                    .Where(file => file.Extension == ".txt" && file.Name.Contains("New Text Document"))
                                    .Count();
            string prefixToName = countOfNewTxtFiles == 0 ? ".txt" : $"({countOfNewTxtFiles}).txt";
            string newName = "New Text Document" + prefixToName;
            FileInfo newFileInfo = new FileInfo(Path.Combine(destinationPath, newName));
            using FileStream fs = newFileInfo.Create();
            return newName;
        }

        public static string CreateFolderWithName(string destinationPath)
        {
            DirectoryInfo di = new DirectoryInfo(destinationPath);
            int countOfNewFolders = di.EnumerateDirectories()
                                    .Where(directory => directory.Name.Contains("New Folder"))
                                    .Count();
            string prefixToName = countOfNewFolders == 0 ? "" : $"({countOfNewFolders})";
            string newName = "New Folder" + prefixToName;
            Directory.CreateDirectory(Path.Combine(destinationPath, newName));
            return newName;
        }
    }
}
