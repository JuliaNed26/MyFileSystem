namespace MyFileSystem
{
    public partial class Form1 : Form
    {
        FileSystem fileSystem;
        FilesActions lastAction;
        List<string> filesToCutCopy;
        public Form1()
        {
            fileSystem = new FileSystem();
            filesToCutCopy = new List<string>();
            fileSystem.FilesListRefreshed += PathChanged;
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
                    string rootDir = fileSystem.Path == "" ? fileSystem.Path : fileSystem.Path + '\\';
                    fileSystem.GoToPath(rootDir + filesList.SelectedItems[0].ToString());
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
                filesToCutCopy.Add(fileSystem.Path + '\\' + item.ToString());
            }
        }

        private void copy_Clicked(object sender, EventArgs e)
        {
            RememberCutCopyFiles();
            lastAction = FilesActions.Copied;
        }

        private void cut_Clicked(object sender, EventArgs e)
        {
            RememberCutCopyFiles();
            lastAction = FilesActions.Cut;
        }

        private void paste_Clicked(object sender, EventArgs e)
        {
            string destination = filesList.SelectedItems.Count == 0 ? currentPath.Text :
                currentPath.Text + filesList.SelectedItems[0].ToString();

            bool shouldCopy = (lastAction & FilesActions.Copied) != 0;

            try
            {
                OperationsWithFilesAndDirectories.RelocateMany(filesToCutCopy, destination, shouldCopy);

                if (!shouldCopy)
                {
                    filesToCutCopy.Clear();
                }

                fileSystem.GoToPath(fileSystem.Path);
            }
            catch(IOException ex)//access denied
            {
                filesToCutCopy.Clear();
                MessageBox.Show(ex.Message);
            }
        }

        private void delete_Clicked(object sender, EventArgs e)
        {
            foreach(var item in filesList.SelectedItems)
            {
                OperationsWithFilesAndDirectories.Delete(currentPath.Text + '\\' + item.ToString());
            }

            fileSystem.GoToPath(fileSystem.Path);
        }

        private void properties_Clicked(object sender, EventArgs e)
        {
            string path = filesList.SelectedItems.Count == 0 ? currentPath.Text : 
                currentPath.Text + (fileSystem.Path == "" ? "" : "\\") + filesList.SelectedItems[0].ToString();

            MessageBox.Show(OperationsWithFilesAndDirectories.GetProperties(path), "Properties");
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
                if(fileSystem.Path == "" && filesList.SelectedItems.Count == 0)
                {
                    for(int i = 0; i < contextMenu.Items.Count; i++)
                    {
                        contextMenu.Items[i].Enabled = false;
                    }
                }
                if(fileSystem.Path == "")
                {
                    copyBtn.Enabled = false;
                    pasteBtn.Enabled = false;
                    cutBtn.Enabled = false;
                    deleteBtn.Enabled = false;
                    renameBtn.Enabled = false;
                }
                if(filesList.SelectedItems.Count > 1)
                {
                    propertiesBtn.Enabled = false;
                    pasteBtn.Enabled = false;
                }
                if (filesList.SelectedItems.Count == 0)
                {
                    renameBtn.Enabled = false;
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
                    OperationsWithFilesAndDirectories.Rename($"{currentPath.Text}\\{filesList.SelectedItems[0]}", renameTextBox.Text);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    renameTextBox.Hide();
                    fileSystem.GoToPath(fileSystem.Path);
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
    }
}