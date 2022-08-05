using FileSystemLib;

namespace MyFileSystem
{
    public partial class Form1 : Form
    {
        FileSystem fileSystem;
        FileSystemObjectsState lastAction;
        List<string> filesToCutCopy;
        public Form1()
        {
            fileSystem = new FileSystem();
            filesToCutCopy = new List<string>();
            fileSystem.PathContentChanged += PathChanged;
            InitializeComponent();
            InitializeContMenuBtns();
            filesList.Items.AddRange(DriveInfo.GetDrives().Select(x => x.Name).ToArray());
        }

        private void InitializeContMenuBtns()
        {
            propertiesBtn = contextMenu.Items[0] as ToolStripMenuItem;
            copyBtn = contextMenu.Items[1] as ToolStripMenuItem;
            cutBtn = contextMenu.Items[2] as ToolStripMenuItem;
            pasteBtn = contextMenu.Items[3] as ToolStripMenuItem;
            deleteBtn = contextMenu.Items[4] as ToolStripMenuItem;
            renameBtn = contextMenu.Items[5] as ToolStripMenuItem;
            createTxtFileBtn = (contextMenu.Items[6] as ToolStripMenuItem).DropDownItems[0];
            createFolderBtn = (contextMenu.Items[6] as ToolStripMenuItem).DropDownItems[1]; ;
        }
        private void PathChanged(object? sender, FilesListRefreshedEventArgs e)
        {
            filesList.SelectionMode = e.Path == "" ? SelectionMode.One : SelectionMode.MultiExtended;
            filesList.Items.Clear();
            filesList.Items.AddRange(e.Files.ToArray());
            currentPath.Text = e.Path ;
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            fileSystem.ReturnToRootDir();
        }

        private void filesList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (filesList.SelectedItems.Count != 0)
            {
                try
                {
                    fileSystem.GoToPath(Path.Combine(fileSystem.CurrentPath,filesList.SelectedItems[0].ToString()));
                }
                catch(IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RememberCutCopyFiles()
        {
            foreach (var item in filesList.SelectedItems)
            {
                filesToCutCopy.Add(Path.Combine(fileSystem.CurrentPath, item.ToString()));
            }
        }

        private void copy_Clicked(object sender, EventArgs e)
        {
            RememberCutCopyFiles();
            lastAction = FileSystemObjectsState.Copied;
        }

        private void cut_Clicked(object sender, EventArgs e)
        {
            RememberCutCopyFiles();
            lastAction = FileSystemObjectsState.Cut;
        }

        private void paste_Clicked(object sender, EventArgs e)
        {
            string destination = filesList.SelectedItems.Count == 0 ?
                currentPath.Text : Path.Combine(currentPath.Text, filesList.SelectedItems[0].ToString());

            bool shouldCut = (lastAction & FileSystemObjectsState.Cut) != 0;

            try
            {
                if (shouldCut)
                {
                    FileSystemObject.MoveManyTo(filesToCutCopy, destination);
                    filesToCutCopy.Clear();
                }
                else
                {
                    FileSystemObject.CopyManyTo(filesToCutCopy, destination);
                }

                fileSystem.GoToPath(fileSystem.CurrentPath);
            }
            catch(IOException ex)
            {
                filesToCutCopy.Clear();
                MessageBox.Show(ex.Message);
            }
        }

        private void delete_Clicked(object sender, EventArgs e)
        {
            foreach(var item in filesList.SelectedItems)
            {
                FileSystemObject.Delete(Path.Combine(currentPath.Text, item.ToString()));
            }

            fileSystem.GoToPath(fileSystem.CurrentPath);
        }

        private void properties_Clicked(object sender, EventArgs e)
        {
            string path = filesList.SelectedItems.Count == 0 ? currentPath.Text : 
                Path.Combine(currentPath.Text, filesList.SelectedItems[0].ToString());

            MessageBox.Show(FileSystemObject.GetProperties(path), "Properties");
        }

        private void rename_Clicked(object? sender, EventArgs e)
        {
            renameTextBox.Text = filesList.SelectedItems[0].ToString();
            renameTextBox.Location = new Point(filesList.Location.X,
            filesList.Location.Y + filesList.Items.IndexOf(filesList.SelectedItems[0]) * filesList.ItemHeight);
            renameTextBox.BringToFront();
            renameTextBox.Visible = true;
            renameTextBox.Focus();
        }

        private void filesList_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if(fileSystem.CurrentPath == "")
                {
                    for(int i = 0; i < contextMenu.Items.Count; i++)
                    {
                        contextMenu.Items[i].Enabled = false;
                    }

                    if(filesList.SelectedItems.Count != 0)
                    {
                        propertiesBtn.Enabled = true;
                    }
                }
                else if (filesList.SelectedItems.Count == 0)
                {
                    renameBtn.Enabled = false;
                    createTxtFileBtn.Enabled = true;
                    createFolderBtn.Enabled = true;
                }
                else if (filesList.SelectedItems.Count > 1)
                {
                    propertiesBtn.Enabled = false;
                    pasteBtn.Enabled = false;
                    createTxtFileBtn.Enabled = false;
                }

                contextMenu.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            foreach(ToolStripItem item in contextMenu.Items)
            {
                item.Enabled = true;
            }
        }

        private void renameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (renameTextBox.Text != "")
            {
                try
                {
                    FileSystemObject.Rename
                        (Path.Combine(currentPath.Text, filesList.SelectedItems[0].ToString()), renameTextBox.Text);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    renameTextBox.Hide();
                    fileSystem.GoToPath(fileSystem.CurrentPath);
                }
            }
            renameTextBox.Hide();
        }

        private void renameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                renameTextBox_LostFocus(sender, e);
            }
        }

        private void createTxtFile_Clicked(object? sender, EventArgs e)
        {
            try
            {
                string newFileName = FileSystemObject.CreateTxtFileWithName(fileSystem.CurrentPath);
                fileSystem.GoToPath(fileSystem.CurrentPath);
                int indexOfCreatedFile = filesList.Items.IndexOf(newFileName);
                filesList.SelectedItems.Add(filesList.Items[indexOfCreatedFile]);
                rename_Clicked(sender, e);
            }
            catch(UnauthorizedAccessException ex)
            {
                MessageBox.Show("Can not create a file");
            }
        }

        private void createFolder_Clicked(object? sender, EventArgs e)
        {
            try
            {
                string newFileName = FileSystemObject.CreateFolderWithName(fileSystem.CurrentPath);
                fileSystem.GoToPath(fileSystem.CurrentPath);
                int indexOfCreatedFile = filesList.Items.IndexOf(newFileName);
                filesList.SelectedItems.Add(filesList.Items[indexOfCreatedFile]);
                rename_Clicked(sender, e);
            }
            catch(UnauthorizedAccessException)
            {
                MessageBox.Show("Should allow admin rights");
            }
        }
    }
}