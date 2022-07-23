using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSystem
{
    internal class FileSystem
    {
        public FileSystem()
        {
            Path = "";
            FillFilesList(Path);
        }

        public event EventHandler<FilesListRefreshedEventArgs> FilesListRefreshed;

        public string Path { get; private set; }
        public List<string> Files { get; private set; }

        public void ReturnToRootDir()
        {
            if (Path != "")
            {
                FillFilesList(Directory.GetParent(Path)?.FullName);
            }
        }

        public void GoToPath(string newPath)
        {
            try
            {
                if (IsDirectory(newPath))
                {
                    FillFilesList(newPath);
                }
                else
                {
                    Process.Start(new ProcessStartInfo(newPath) { UseShellExecute = true });//uses operating system shell for opening the files
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual void OnFilesListRefreshed(FilesListRefreshedEventArgs e) => FilesListRefreshed?.Invoke(this, e);
        private bool IsSystemOrHidden (string path) => (File.GetAttributes(path) & (FileAttributes.System | FileAttributes.Hidden)) != 0;

        private bool IsDirectory(string path) => (File.GetAttributes(path) & FileAttributes.Directory) != 0;

        private void FillFilesList(string _path)
        {

            if (_path == "" || _path == null)
            {
                Files = DriveInfo.GetDrives().Select(x => x.Name).ToList();
            }
            else
            {
                Files = Directory.GetFiles(_path)
                              .Where(file => !IsSystemOrHidden(file))
                              .Select(file => file.Substring(file.LastIndexOf('\\') + 1))
                              .ToList();

                Files.AddRange(Directory.GetDirectories(_path)
                                      .Where(directory => !IsSystemOrHidden(directory))
                                      .Select(directory => directory.Substring(directory.LastIndexOf('\\') + 1)));
            }

            Path = _path == null ? "" : _path;
            OnFilesListRefreshed(new FilesListRefreshedEventArgs(Files, Path));
        }


    }

    internal class FilesListRefreshedEventArgs : EventArgs
    {
        public readonly List<string> Files;
        public readonly string Path;

        public FilesListRefreshedEventArgs(List<string> files, string path)
            => (Files, Path) = (files, path);
    }
}
