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
            csv文件内容参考ToolStripMenuItem = new ToolStripMenuItem();
            网络ToolStripMenuItem = new ToolStripMenuItem();
            国内镜像链接ToolStripMenuItem = new ToolStripMenuItem();
            dataSourceGroup = new GroupBox();
            btnReloadOnline = new Button();
            onlineLinkCountLabel = new Label();
            onlineFileLinkEdit = new Button();
            onlineLinkstextbox = new TextBox();
            onlineLinksFile = new Label();
            localFileCountLabel = new Label();
            convertGroup = new GroupBox();
            desFormatLabel = new Label();
            desFormatCombo = new ComboBox();
            rightArrowLabel = new Label();
            sourceFormatLabel = new Label();
            sourceFormatCombo = new ComboBox();
            handleGroup = new GroupBox();
            progressBar = new ProgressBar();
            onlineFileList = new CheckedListBox();
            onlineSelectGroup = new GroupBox();
            menuStrip1.SuspendLayout();
            dataSourceGroup.SuspendLayout();
            convertGroup.SuspendLayout();
            handleGroup.SuspendLayout();
            onlineSelectGroup.SuspendLayout();
            SuspendLayout();
            // 
            // txtFolderPath
            // 
            txtFolderPath.Location = new Point(116, 34);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.ReadOnly = true;
            txtFolderPath.Size = new Size(411, 30);
            txtFolderPath.TabIndex = 0;
            txtFolderPath.Text = "请选择包含 .csv 数据文件的目录 (仅本地生成用)";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(533, 33);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(92, 34);
            btnBrowseFolder.TabIndex = 1;
            btnBrowseFolder.Text = "选择...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(240, 29);
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
            fileText.Location = new Point(6, 37);
            fileText.Name = "fileText";
            fileText.Size = new Size(104, 24);
            fileText.TabIndex = 3;
            fileText.Text = "本地文件夹:";
            // 
            // btnBrowseOnlineFiles
            // 
            btnBrowseOnlineFiles.Location = new Point(395, 29);
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
            menuStrip1.Items.AddRange(new ToolStripItem[] { ffxivdataminingcnToolStripMenuItem, 网络ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(841, 32);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // ffxivdataminingcnToolStripMenuItem
            // 
            ffxivdataminingcnToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ffxivdataminingcnToolStripMenuItem1, 深蓝词库转换ToolStripMenuItem1, csv文件内容参考ToolStripMenuItem });
            ffxivdataminingcnToolStripMenuItem.Name = "ffxivdataminingcnToolStripMenuItem";
            ffxivdataminingcnToolStripMenuItem.Size = new Size(142, 28);
            ffxivdataminingcnToolStripMenuItem.Text = "相关工具/资料";
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
            // csv文件内容参考ToolStripMenuItem
            // 
            csv文件内容参考ToolStripMenuItem.Name = "csv文件内容参考ToolStripMenuItem";
            csv文件内容参考ToolStripMenuItem.Size = new Size(281, 34);
            csv文件内容参考ToolStripMenuItem.Text = ".csv 文件内容参考";
            csv文件内容参考ToolStripMenuItem.Click += csv文件内容参考ToolStripMenuItem_Click;
            // 
            // 网络ToolStripMenuItem
            // 
            网络ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 国内镜像链接ToolStripMenuItem });
            网络ToolStripMenuItem.Name = "网络ToolStripMenuItem";
            网络ToolStripMenuItem.Size = new Size(62, 28);
            网络ToolStripMenuItem.Text = "网络";
            // 
            // 国内镜像链接ToolStripMenuItem
            // 
            国内镜像链接ToolStripMenuItem.Name = "国内镜像链接ToolStripMenuItem";
            国内镜像链接ToolStripMenuItem.Size = new Size(314, 34);
            国内镜像链接ToolStripMenuItem.Text = "使用国内镜像 (Gitmirror)";
            国内镜像链接ToolStripMenuItem.Click += 国内镜像链接ToolStripMenuItem_Click;
            // 
            // dataSourceGroup
            // 
            dataSourceGroup.Controls.Add(btnReloadOnline);
            dataSourceGroup.Controls.Add(onlineLinkCountLabel);
            dataSourceGroup.Controls.Add(onlineFileLinkEdit);
            dataSourceGroup.Controls.Add(onlineLinkstextbox);
            dataSourceGroup.Controls.Add(onlineLinksFile);
            dataSourceGroup.Controls.Add(localFileCountLabel);
            dataSourceGroup.Controls.Add(fileText);
            dataSourceGroup.Controls.Add(txtFolderPath);
            dataSourceGroup.Controls.Add(btnBrowseFolder);
            dataSourceGroup.Location = new Point(16, 35);
            dataSourceGroup.Name = "dataSourceGroup";
            dataSourceGroup.Size = new Size(809, 127);
            dataSourceGroup.TabIndex = 9;
            dataSourceGroup.TabStop = false;
            dataSourceGroup.Text = "数据源";
            // 
            // btnReloadOnline
            // 
            btnReloadOnline.Location = new Point(435, 78);
            btnReloadOnline.Name = "btnReloadOnline";
            btnReloadOnline.Size = new Size(92, 34);
            btnReloadOnline.TabIndex = 9;
            btnReloadOnline.Text = "重置";
            btnReloadOnline.UseVisualStyleBackColor = true;
            btnReloadOnline.Click += btnReloadOnline_Click;
            // 
            // onlineLinkCountLabel
            // 
            onlineLinkCountLabel.AutoSize = true;
            onlineLinkCountLabel.Location = new Point(632, 83);
            onlineLinkCountLabel.Name = "onlineLinkCountLabel";
            onlineLinkCountLabel.Size = new Size(104, 24);
            onlineLinkCountLabel.TabIndex = 8;
            onlineLinkCountLabel.Text = "当前链接数:";
            onlineLinkCountLabel.Click += onlineLinkCountLabel_Click;
            // 
            // onlineFileLinkEdit
            // 
            onlineFileLinkEdit.Location = new Point(533, 78);
            onlineFileLinkEdit.Name = "onlineFileLinkEdit";
            onlineFileLinkEdit.Size = new Size(92, 34);
            onlineFileLinkEdit.TabIndex = 7;
            onlineFileLinkEdit.Text = "编辑";
            onlineFileLinkEdit.UseVisualStyleBackColor = true;
            onlineFileLinkEdit.Click += onlineFileLinkEdit_Click;
            // 
            // onlineLinkstextbox
            // 
            onlineLinkstextbox.Location = new Point(134, 80);
            onlineLinkstextbox.Name = "onlineLinkstextbox";
            onlineLinkstextbox.ReadOnly = true;
            onlineLinkstextbox.Size = new Size(295, 30);
            onlineLinkstextbox.TabIndex = 6;
            onlineLinkstextbox.Text = "Links.txt 文件读取错误";
            // 
            // onlineLinksFile
            // 
            onlineLinksFile.AutoSize = true;
            onlineLinksFile.Location = new Point(6, 83);
            onlineLinksFile.Name = "onlineLinksFile";
            onlineLinksFile.Size = new Size(122, 24);
            onlineLinksFile.TabIndex = 5;
            onlineLinksFile.Text = "在线链接文件:";
            // 
            // localFileCountLabel
            // 
            localFileCountLabel.AutoSize = true;
            localFileCountLabel.Location = new Point(632, 38);
            localFileCountLabel.Name = "localFileCountLabel";
            localFileCountLabel.Size = new Size(104, 24);
            localFileCountLabel.TabIndex = 4;
            localFileCountLabel.Text = "当前文件数:";
            // 
            // convertGroup
            // 
            convertGroup.Controls.Add(desFormatLabel);
            convertGroup.Controls.Add(desFormatCombo);
            convertGroup.Controls.Add(rightArrowLabel);
            convertGroup.Controls.Add(sourceFormatLabel);
            convertGroup.Controls.Add(sourceFormatCombo);
            convertGroup.Location = new Point(16, 172);
            convertGroup.Name = "convertGroup";
            convertGroup.Size = new Size(809, 86);
            convertGroup.TabIndex = 10;
            convertGroup.TabStop = false;
            convertGroup.Text = "输出格式转换 (来源: 深蓝词库转换)";
            // 
            // desFormatLabel
            // 
            desFormatLabel.AutoSize = true;
            desFormatLabel.Location = new Point(692, 38);
            desFormatLabel.Name = "desFormatLabel";
            desFormatLabel.Size = new Size(100, 24);
            desFormatLabel.TabIndex = 4;
            desFormatLabel.Text = "转换后格式";
            // 
            // desFormatCombo
            // 
            desFormatCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            desFormatCombo.FormattingEnabled = true;
            desFormatCombo.Items.AddRange(new object[] { "无拼音纯汉字 (搜狗新版)", "搜狗拼音txt (搜狗旧版)", "搜狗细胞词库scel", "搜狗拼音备份词库bin", "QQ拼音", "QQ分类词库qpyd", "QQ分类词库qcel", "QQ五笔", "QQ拼音英文", "百度拼音", "小小输入法", "百度分类词库bdict", "谷歌拼音", "Gboard", "拼音加加", "Win10微软拼音 (自定义短语)", "Win10微软五笔 (自定义短语)", "Win10微软拼音 (自学习词库)", "微软拼音", "必应输入法", "FIT输入法", "Rime中州韵", "Mac简体拼音", "华宇紫光拼音", "紫光拼音词库uwl", "libpinyin", "Chinese-pyim", "手心输入法", "新浪拼音", "极点五笔", "极点郑码", "极点五笔.mb文件", "小鸭五笔", "雅虎奇摩", "灵格斯ld2", "五笔86版", "五笔98版", "五笔新世纪版", "仓颉平台", "Emoji", "百度手机或Mac版百度拼音", "百度手机英文", "百度手机词库bcd", "QQ手机", "讯飞输入法" });
            desFormatCombo.Location = new Point(395, 35);
            desFormatCombo.Name = "desFormatCombo";
            desFormatCombo.Size = new Size(291, 32);
            desFormatCombo.TabIndex = 3;
            // 
            // rightArrowLabel
            // 
            rightArrowLabel.AutoSize = true;
            rightArrowLabel.Location = new Point(290, 38);
            rightArrowLabel.Name = "rightArrowLabel";
            rightArrowLabel.Size = new Size(99, 24);
            rightArrowLabel.TabIndex = 2;
            rightArrowLabel.Text = "————>";
            // 
            // sourceFormatLabel
            // 
            sourceFormatLabel.AutoSize = true;
            sourceFormatLabel.Location = new Point(10, 38);
            sourceFormatLabel.Name = "sourceFormatLabel";
            sourceFormatLabel.Size = new Size(68, 24);
            sourceFormatLabel.TabIndex = 1;
            sourceFormatLabel.Text = "源格式:";
            // 
            // sourceFormatCombo
            // 
            sourceFormatCombo.Enabled = false;
            sourceFormatCombo.FormattingEnabled = true;
            sourceFormatCombo.Items.AddRange(new object[] { "搜狗拼音txt" });
            sourceFormatCombo.Location = new Point(84, 35);
            sourceFormatCombo.Name = "sourceFormatCombo";
            sourceFormatCombo.Size = new Size(198, 32);
            sourceFormatCombo.TabIndex = 0;
            // 
            // handleGroup
            // 
            handleGroup.Controls.Add(progressBar);
            handleGroup.Controls.Add(btnBrowseOnlineFiles);
            handleGroup.Controls.Add(btnConvert);
            handleGroup.Location = new Point(16, 268);
            handleGroup.Name = "handleGroup";
            handleGroup.Size = new Size(809, 127);
            handleGroup.TabIndex = 12;
            handleGroup.TabStop = false;
            handleGroup.Text = "生成";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(10, 75);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(785, 34);
            progressBar.TabIndex = 12;
            // 
            // onlineFileList
            // 
            onlineFileList.CheckOnClick = true;
            onlineFileList.ColumnWidth = 250;
            onlineFileList.FormattingEnabled = true;
            onlineFileList.Location = new Point(10, 27);
            onlineFileList.MultiColumn = true;
            onlineFileList.Name = "onlineFileList";
            onlineFileList.Size = new Size(785, 274);
            onlineFileList.TabIndex = 13;
            // 
            // onlineSelectGroup
            // 
            onlineSelectGroup.Controls.Add(onlineFileList);
            onlineSelectGroup.Location = new Point(16, 401);
            onlineSelectGroup.Name = "onlineSelectGroup";
            onlineSelectGroup.Size = new Size(809, 334);
            onlineSelectGroup.TabIndex = 14;
            onlineSelectGroup.TabStop = false;
            onlineSelectGroup.Text = "在线生成数据源";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(841, 747);
            Controls.Add(onlineSelectGroup);
            Controls.Add(convertGroup);
            Controls.Add(menuStrip1);
            Controls.Add(dataSourceGroup);
            Controls.Add(handleGroup);
            MainMenuStrip = menuStrip1;
            Name = "Main";
            Text = "FF14 输入法自定义词库生成器 ";
            Load += Main_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            dataSourceGroup.ResumeLayout(false);
            dataSourceGroup.PerformLayout();
            convertGroup.ResumeLayout(false);
            convertGroup.PerformLayout();
            handleGroup.ResumeLayout(false);
            onlineSelectGroup.ResumeLayout(false);
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
        private GroupBox dataSourceGroup;
        private Label localFileCountLabel;
        private Label onlineLinksFile;
        private TextBox onlineLinkstextbox;
        private Label onlineLinkCountLabel;
        private Button onlineFileLinkEdit;
        private GroupBox convertGroup;
        private Label sourceFormatLabel;
        private ComboBox sourceFormatCombo;
        private Label rightArrowLabel;
        private Label desFormatLabel;
        private ComboBox desFormatCombo;
        private GroupBox handleGroup;
        private ProgressBar progressBar;
        private ToolStripMenuItem csv文件内容参考ToolStripMenuItem;
        private CheckedListBox onlineFileList;
        private GroupBox onlineSelectGroup;
        private Button btnReloadOnline;
        private ToolStripMenuItem 网络ToolStripMenuItem;
        private ToolStripMenuItem 国内镜像链接ToolStripMenuItem;
    }
}