namespace MyFileSystem
{
    public partial class Form1 : Form
    {
        FileSystem fileSystem;
        public Form1()
        {
            fileSystem = new FileSystem();
            fileSystem.FilesListRefreshed += PathChanged;
            InitializeComponent();
            filesList.Items.AddRange(DriveInfo.GetDrives().Select(x => x.Name).ToArray());
        }

        private void PathChanged(object? sender, FilesListRefreshedEventArgs e)
        {
            filesList.Items.Clear();
            filesList.Items.AddRange(e.Files.ToArray());
            currentPath.Text = e.Path + '\\';
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            fileSystem.ReturnToRootDir();
        }

        private void filesList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (filesList.SelectedItems.Count != 0)
            {
                fileSystem.GoToPath(currentPath.Text + filesList.SelectedItems[0].ToString());
            }
        }
    }
}