using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSystem
{
    internal static class OperationsWithFilesAndDirectories
    {
        public static void RelocateMany(List<string> pathsToRelocate, string destination, bool move)
        {
            if (Directory.Exists(destination))
            {
                foreach (string path in pathsToRelocate)
                {
                    if (File.Exists(path))
                    {
                        if (move)
                        {
                            File.Move(path, $"{destination}\\{Path.GetFileName(path)}");
                        }
                        else
                        {
                            File.Copy(path, $"{destination}\\{Path.GetFileName(path)}");
                        }
                    }

                    if (Directory.Exists(path))
                    {
                        if (move)
                        {
                            string s = $"{destination}\\{path.Substring(path.LastIndexOf('\\') + 1)}";
                            Directory.Move(path, $"{destination}\\{path.Substring(path.LastIndexOf('\\'))}");
                        }
                        else
                        {
                            CopyDir(path, destination);
                        }
                    }
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {destination} wasn't found");
            }

            static void CopyDir(string dirPath, string destination)
            {
                destination += '\\' + (new DirectoryInfo(dirPath)).Name;
                Directory.CreateDirectory(destination);
                List<string> allFiles = Directory.GetFiles(dirPath).ToList();
                allFiles.AddRange(Directory.GetDirectories(dirPath).ToList());
                RelocateMany(allFiles, destination, false);
            }
        }

        public static void Delete(string path)
        {
            if(File.Exists(path) && 
              ((new FileInfo(path)).Attributes & FileAttributes.System) == 0)
            {
                File.Delete(path);
            }

            if(Directory.Exists(path) &&
               ((new DirectoryInfo(path)).Attributes & FileAttributes.System) == 0)
            {
                Directory.Delete(path, true);
            }
        }

        public static string GetProperties(string path)
        {
            if(File.Exists(path))
            {
                return GetFileProperties(path);
            }

            if(path.Length == 3)//drive
            {
                return GetDriveProperties(path);
            }

            if(Directory.Exists(path))
            {
                return GetDirectoryProperties(path);
            }

            throw new FileNotFoundException();

            static string GetDriveProperties(string drive)
            {
                DriveInfo driveInfo = DriveInfo.GetDrives().Where(d => d.Name == drive).FirstOrDefault();
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
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                File.Move(path, 
                    fileInfo.Directory.FullName + '\\' + newName + (newName.Contains(fileInfo.Extension) ? "" : fileInfo.Extension));
            }
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                string newDirectoryPath = $"{dirInfo.Parent}\\{newName}";
                Directory.CreateDirectory(newDirectoryPath);
                List<string> dirConsistance = dirInfo.GetDirectories()
                                             .Select(directory => directory.FullName)
                                             .ToList();
                dirConsistance.AddRange(dirInfo.GetFiles().Select(file => file.FullName));
                RelocateMany(dirConsistance, newDirectoryPath, true);
                Delete(path);
            }
        }
    }
}
