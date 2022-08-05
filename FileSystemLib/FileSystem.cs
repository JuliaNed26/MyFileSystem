using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemLib
{
    public class FileSystem
    {
        public FileSystem()
        {
            CurrentPath = "";
            FillPathContent(CurrentPath);
        }

        public event EventHandler<FilesListRefreshedEventArgs> PathContentChanged;

        public string CurrentPath { get; private set; }
        public List<string> PathContent { get; private set; }

        public void ReturnToRootDir()
        {
            if (CurrentPath != "")
            {
                FillPathContent(Directory.GetParent(CurrentPath)?.FullName);
            }
        }

        public void GoToPath(string newPath)
        {
            if (FileSystemObject.IsDirectory(newPath))
            {
                FillPathContent(newPath);
            }
            else
            {
                Process.Start(new ProcessStartInfo(newPath) { UseShellExecute = true });//uses operating system shell for opening the files
            }
        }

        protected virtual void OnPathContentRefilled(FilesListRefreshedEventArgs e) => PathContentChanged?.Invoke(this, e);

        private void FillPathContent(string _path)
        {

            if (_path == "" || _path == null)
            {
                PathContent = DriveInfo.GetDrives().Select(x => x.Name).ToList();
            }
            else
            {
                PathContent = Directory.GetFiles(_path)
                              .Where(file => !(new FileInfo(file)).IsSystemOrHidden())
                              .Select(file => (new FileInfo(file)).Name)
                              .ToList();

                PathContent.AddRange(Directory.GetDirectories(_path)
                                      .Where(directory => !(new DirectoryInfo(directory)).IsSystemOrHidden())
                                      .Select(directory => (new DirectoryInfo(directory)).Name));
            }

            CurrentPath = _path == null ? "" : _path;
            OnPathContentRefilled(new FilesListRefreshedEventArgs(PathContent, CurrentPath));
        }


    }

    public static class FileInfoExtensions
    {
        public static bool IsSystemOrHidden(this FileInfo file) => 
            (File.GetAttributes(file.FullName) & (FileAttributes.System | FileAttributes.Hidden)) != 0;
    }

    public static class DirectoryInfoExtensions
    {
        public static bool IsSystemOrHidden(this DirectoryInfo dir) =>
            (File.GetAttributes(dir.FullName) & (FileAttributes.System | FileAttributes.Hidden)) != 0;
    }

    public class FilesListRefreshedEventArgs : EventArgs
    {
        public readonly List<string> Files;
        public readonly string Path;

        public FilesListRefreshedEventArgs(List<string> files, string path)
            => (Files, Path) = (files, path);
    }
}
