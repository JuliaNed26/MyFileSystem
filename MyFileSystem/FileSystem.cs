using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSystem
{
    internal class FilesListRefreshedEventArgs:EventArgs
    {
        public readonly List<string> Files;
        public readonly string Path;

        public FilesListRefreshedEventArgs(List<string> files, string path)
        {
            Files = files;
            Path = path;
        }
    }
    internal class FileSystem
    {
        public string Path { get; private set; }
        public List<string> Files { get; private set; }

        public event EventHandler<FilesListRefreshedEventArgs> FilesListRefreshed;


        public FileSystem()
        {
            Path = "";
            FillFilesList(Path);
        }
        protected virtual void OnFilesListRefreshed(FilesListRefreshedEventArgs e)
        {
            FilesListRefreshed?.Invoke(this, e);
        }

        private bool IsSystemOrHidden(string path)
        {
            FileAttributes fa = File.GetAttributes(path);
            return (fa & (FileAttributes.System | FileAttributes.Hidden)) != 0;
        }

        private void FillFilesList(string _path)
        {
            if (_path == "" || _path == null)
            {
                Files = DriveInfo.GetDrives().Select(x => x.Name).ToList();
            }
            else
            {
                List<string> newFilesList = new List<string>();

                foreach (var file in Directory.GetFiles(_path))
                {
                    if (!IsSystemOrHidden(file))
                    {
                        newFilesList.Add(file.Substring(file.LastIndexOf('\\') + 1));
                    }
                }

                foreach (var directory in Directory.GetDirectories(_path))
                {
                    if (!IsSystemOrHidden(directory))
                    {
                        newFilesList.Add(directory.Substring(directory.LastIndexOf('\\') + 1));
                    }
                }


                Files.Clear();
                Files = newFilesList;
            }
            Path = _path == null ? "" : _path;
            OnFilesListRefreshed(new FilesListRefreshedEventArgs(Files, Path));
        }

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
                FillFilesList(newPath);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


    }
}
