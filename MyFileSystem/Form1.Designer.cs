namespace MyFileSystem
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        /// 

        private ToolStripMenuItem propertiesBtn;
        private ToolStripMenuItem copyBtn;
        private ToolStripMenuItem cutBtn;
        private ToolStripMenuItem pasteBtn;
        private ToolStripMenuItem deleteBtn;
        private ToolStripMenuItem renameBtn;
        private ToolStripItem createTxtFileBtn;
        private ToolStripItem createFolderBtn;
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.currentPath = new System.Windows.Forms.TextBox();
            this.backBtn = new System.Windows.Forms.Button();
            this.filesList = new System.Windows.Forms.ListBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // currentPath
            // 
            this.currentPath.Location = new System.Drawing.Point(42, 12);
            this.currentPath.Name = "currentPath";
            this.currentPath.ReadOnly = true;
            this.currentPath.Size = new System.Drawing.Size(746, 23);
            this.currentPath.TabIndex = 0;
            // 
            // backBtn
            // 
            this.backBtn.Location = new System.Drawing.Point(12, 12);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(24, 23);
            this.backBtn.TabIndex = 1;
            this.backBtn.Text = "<";
            this.backBtn.UseVisualStyleBackColor = true;
            this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
            // 
            // filesList
            // 
            this.filesList.FormattingEnabled = true;
            this.filesList.ItemHeight = 15;
            this.filesList.Location = new System.Drawing.Point(12, 52);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(776, 394);
            this.filesList.TabIndex = 2;
            this.filesList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.filesList_MouseDoubleClick);
            this.filesList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.filesList_MouseDown);
            // 
            // contextMenu
            // 
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            this.contextMenu.Items.Add("Properties", null, properties_Clicked);
            this.contextMenu.Items.Add("Copy", null, copy_Clicked);
            this.contextMenu.Items.Add("Cut", null, cut_Clicked);
            this.contextMenu.Items.Add("Paste", null, paste_Clicked);
            this.contextMenu.Items.Add("Delete", null, delete_Clicked);
            this.contextMenu.Items.Add("Rename", null, rename_Clicked);
            this.contextMenu.Items.Add("Create");
            (this.contextMenu.Items[6] as ToolStripMenuItem).DropDownItems.Add("New .txt file", null, createTxtFile_Clicked);
            (this.contextMenu.Items[6] as ToolStripMenuItem).DropDownItems.Add("New folder", null, createFolder_Clicked);
            this.contextMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenu_Closed);
            // 
            // renameTextBox
            // 
            this.renameTextBox.Location = new System.Drawing.Point(12, 323);
            this.renameTextBox.AutoSize = false;
            this.renameTextBox.Name = "renameTextBox";
            this.renameTextBox.Size = new System.Drawing.Size(776, 20);
            this.renameTextBox.TabIndex = 3;
            this.renameTextBox.Visible = false;
            this.renameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.renameTextBox_KeyDown);
            this.renameTextBox.LostFocus += new System.EventHandler(this.renameTextBox_LostFocus);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.renameTextBox);
            this.Controls.Add(this.filesList);
            this.Controls.Add(this.backBtn);
            this.Controls.Add(this.currentPath);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox currentPath;
        private Button backBtn;
        private ListBox filesList;
        private ContextMenuStrip contextMenu;
        private TextBox renameTextBox;
    }
}