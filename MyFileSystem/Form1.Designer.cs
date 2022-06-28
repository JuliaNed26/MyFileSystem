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
        private void InitializeComponent()
        {
            this.currentPath = new System.Windows.Forms.TextBox();
            this.backBtn = new System.Windows.Forms.Button();
            this.filesList = new System.Windows.Forms.ListBox();
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
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}