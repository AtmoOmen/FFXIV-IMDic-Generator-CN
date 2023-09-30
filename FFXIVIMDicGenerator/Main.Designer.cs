namespace FFXIVIMDicGenerator
{
    partial class Main
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
            txtFolderPath = new TextBox();
            btnBrowseFolder = new Button();
            btnConvert = new Button();
            fileText = new Label();
            btnBrowseOnlineFiles = new Button();
            menuStrip1 = new MenuStrip();
            ffxivdataminingcnToolStripMenuItem = new ToolStripMenuItem();
            ffxivdataminingcnToolStripMenuItem1 = new ToolStripMenuItem();
            深蓝词库转换ToolStripMenuItem1 = new ToolStripMenuItem();
            编辑ToolStripMenuItem = new ToolStripMenuItem();
            linkstxtToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            outputtxtToolStripMenuItem = new ToolStripMenuItem();
            processLabel = new Label();
            processTitle = new Label();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFolderPath
            // 
            txtFolderPath.Location = new Point(128, 45);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.ReadOnly = true;
            txtFolderPath.Size = new Size(334, 30);
            txtFolderPath.TabIndex = 0;
            txtFolderPath.Text = "请选择包含.csv文件的目录";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(473, 45);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(92, 34);
            btnBrowseFolder.TabIndex = 1;
            btnBrowseFolder.Text = "选择...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(571, 45);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(149, 34);
            btnConvert.TabIndex = 2;
            btnConvert.Text = "从本地文件生成";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // fileText
            // 
            fileText.AutoSize = true;
            fileText.Location = new Point(16, 48);
            fileText.Name = "fileText";
            fileText.Size = new Size(104, 24);
            fileText.TabIndex = 3;
            fileText.Text = "文件夹路径:";
            fileText.Click += fileText_Click;
            // 
            // btnBrowseOnlineFiles
            // 
            btnBrowseOnlineFiles.Location = new Point(726, 45);
            btnBrowseOnlineFiles.Name = "btnBrowseOnlineFiles";
            btnBrowseOnlineFiles.Size = new Size(152, 34);
            btnBrowseOnlineFiles.TabIndex = 4;
            btnBrowseOnlineFiles.Text = "从在线文件生成";
            btnBrowseOnlineFiles.UseVisualStyleBackColor = true;
            btnBrowseOnlineFiles.Click += btnBrowseOnlineFiles_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { ffxivdataminingcnToolStripMenuItem, 编辑ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(890, 32);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // ffxivdataminingcnToolStripMenuItem
            // 
            ffxivdataminingcnToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ffxivdataminingcnToolStripMenuItem1, 深蓝词库转换ToolStripMenuItem1 });
            ffxivdataminingcnToolStripMenuItem.Name = "ffxivdataminingcnToolStripMenuItem";
            ffxivdataminingcnToolStripMenuItem.Size = new Size(98, 28);
            ffxivdataminingcnToolStripMenuItem.Text = "相关工具";
            // 
            // ffxivdataminingcnToolStripMenuItem1
            // 
            ffxivdataminingcnToolStripMenuItem1.Name = "ffxivdataminingcnToolStripMenuItem1";
            ffxivdataminingcnToolStripMenuItem1.Size = new Size(281, 34);
            ffxivdataminingcnToolStripMenuItem1.Text = "ffxiv-datamining-cn";
            ffxivdataminingcnToolStripMenuItem1.Click += ffxivdataminingcnToolStripMenuItem1_Click;
            // 
            // 深蓝词库转换ToolStripMenuItem1
            // 
            深蓝词库转换ToolStripMenuItem1.Name = "深蓝词库转换ToolStripMenuItem1";
            深蓝词库转换ToolStripMenuItem1.Size = new Size(281, 34);
            深蓝词库转换ToolStripMenuItem1.Text = "深蓝词库转换";
            深蓝词库转换ToolStripMenuItem1.Click += 深蓝词库转换ToolStripMenuItem1_Click;
            // 
            // 编辑ToolStripMenuItem
            // 
            编辑ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { linkstxtToolStripMenuItem, toolStripMenuItem1, outputtxtToolStripMenuItem });
            编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            编辑ToolStripMenuItem.Size = new Size(62, 28);
            编辑ToolStripMenuItem.Text = "编辑";
            // 
            // linkstxtToolStripMenuItem
            // 
            linkstxtToolStripMenuItem.Name = "linkstxtToolStripMenuItem";
            linkstxtToolStripMenuItem.Size = new Size(221, 34);
            linkstxtToolStripMenuItem.Text = "Links.txt";
            linkstxtToolStripMenuItem.Click += linkstxtToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(221, 34);
            toolStripMenuItem1.Text = "重置 Links.txt";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // outputtxtToolStripMenuItem
            // 
            outputtxtToolStripMenuItem.Name = "outputtxtToolStripMenuItem";
            outputtxtToolStripMenuItem.Size = new Size(221, 34);
            outputtxtToolStripMenuItem.Text = "output.txt";
            outputtxtToolStripMenuItem.Click += outputtxtToolStripMenuItem_Click;
            // 
            // processLabel
            // 
            processLabel.AutoSize = true;
            processLabel.BackColor = Color.Transparent;
            processLabel.Location = new Point(114, 96);
            processLabel.Name = "processLabel";
            processLabel.Size = new Size(0, 24);
            processLabel.TabIndex = 7;
            // 
            // processTitle
            // 
            processTitle.AutoSize = true;
            processTitle.BackColor = Color.Transparent;
            processTitle.Location = new Point(16, 96);
            processTitle.Name = "processTitle";
            processTitle.Size = new Size(86, 24);
            processTitle.TabIndex = 8;
            processTitle.Text = "处理进度:";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(890, 149);
            Controls.Add(processTitle);
            Controls.Add(processLabel);
            Controls.Add(btnBrowseOnlineFiles);
            Controls.Add(fileText);
            Controls.Add(btnConvert);
            Controls.Add(btnBrowseFolder);
            Controls.Add(txtFolderPath);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Main";
            Text = "FF14自定词库生成器";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtFolderPath;
        private Button btnBrowseFolder;
        private Button btnConvert;
        private Label fileText;
        private Button btnBrowseOnlineFiles;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ffxivdataminingcnToolStripMenuItem;
        private ToolStripMenuItem ffxivdataminingcnToolStripMenuItem1;
        private ToolStripMenuItem 深蓝词库转换ToolStripMenuItem1;
        private ToolStripMenuItem 编辑ToolStripMenuItem;
        private ToolStripMenuItem linkstxtToolStripMenuItem;
        private ToolStripMenuItem outputtxtToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private Label processLabel;
        private Label processTitle;
    }
}