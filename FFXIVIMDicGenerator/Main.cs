using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVIMDicGenerator
{
#pragma warning disable CS8600, CS8602, CS8603, CS8604, CS8622, CS4014

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            Text += localVersion;

            InitializeCheckedBox();

            StartUpdateProgram();

            AnalyzeDomains();
        }

        // 浏览本地文件夹
        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                disableAllbtns();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = folderDialog.SelectedPath;
                    var csvFileCount = Directory.EnumerateFiles(folderDialog.SelectedPath, "*.csv", SearchOption.AllDirectories).Count();
                    localFileCountLabel.Text = $"当前文件数: {csvFileCount}";
                }
                enableAllbtns();
            }
        }

        // 本地转换功能
        private async void btnConvert_Click(object sender, EventArgs e)
        {
            string folderPath = txtFolderPath.Text;

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                MessageBox.Show("请选择有效的文件夹");
                return;
            }

            var Format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
            if (string.IsNullOrEmpty(Format) || Format == "未知")
            {
                MessageBox.Show("请选择有效的格式转换类型");
                return;
            }

            List<string> allData = new();

            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");
            if (csvFiles.Length == 0)
            {
                MessageBox.Show("文件夹中没有找到任何 CSV 文件");
                return;
            }

            disableAllbtns();

            var tasks = csvFiles.Select(csvFile => ProcessCsvFile(csvFile, allData)).ToArray();
            await Task.WhenAll(tasks);

            try
            {
                File.WriteAllLines(outputFilePath, allData, Encoding.UTF8);
                var removedData = RemoveDuplicates(outputFilePath);
                OpenConvertCmd(Format);
                MessageBox.Show($"处理完成，共 {allData.Count - removedData} 条\n" +
                    $"输出文件位于: {outputFilePath}");
                enableAllbtns();
                handleGroup.Text = "生成";
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时发生错误: {ex.Message}");
                enableAllbtns();
                handleGroup.Text = "生成";
            }
        }

        // 在线文件生成功能
        private async void btnBrowseOnlineFiles_Click(object sender, EventArgs e)
        {
            disableAllbtns();
            List<string> allData = new List<string>();

            var Format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
            if (string.IsNullOrEmpty(Format) || Format == "未知")
            {
                MessageBox.Show("请选择有效的格式转换类型");
                return;
            }

            onlineLinksFromFile = onlineLinksFromFile.Where(link => !string.IsNullOrEmpty(link)).ToList();

            int totalFiles = onlineLinksFromFile.Count;
            int processedFiles = 0;

            progressBar.Maximum = totalFiles;
            progressBar.Value = 0;

            await Task.WhenAll(onlineLinksFromFile.Select(async link =>
            {
                await ProcessCsvFile(link, allData);

                processedFiles++;
                progressBar.Value = processedFiles;
            }));

            try
            {
                File.WriteAllLines(outputFilePath, allData, Encoding.UTF8);
                var removedData = RemoveDuplicates(outputFilePath);
                OpenConvertCmd(Format);
                MessageBox.Show($"处理完成，共 {allData.Count - removedData} 条\n" +
                    $"输出文件位于: {outputFilePath}");
                enableAllbtns();
                handleGroup.Text = "生成";
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时发生错误: {ex.Message}");
                enableAllbtns();
                handleGroup.Text = "生成";
            }
        }

        // 主界面初始化
        private void Main_Load(object sender, EventArgs e)
        {
        }

        // 菜单栏：相关工具/资料 ——开始——
        private void ffxivdataminingcnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/thewakingsands/ffxiv-datamining-cn");
        }

        private void 深蓝词库转换ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/studyzy/imewlconverter");
        }

        private void csv文件内容参考ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/Souma-Sumire/FFXIVChnTextPatch-Souma/wiki/CSV%E6%96%87%E4%BB%B6");
        }

        // 菜单栏：相关工具/资料 ——结束——

        // 在线文件生成-编辑 Links.txt 功能
        private void onlineFileLinkEdit_Click(object sender, EventArgs e)
        {
            OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
        }

        // 刷新各种与在线文件生成功能相关的组件
        private void RefreshOnlineRelatedComponents(int param = -1)
        {
            onlineFileList.ItemCheck -= onlineFileList_ItemCheck;

            string fileContent = File.ReadAllText(LinksFilePath);
            LinksName = GetFileNamesFromLinksFile(LinksFilePath);
            string[] lines = File.ReadAllLines(LinksFilePath);

            string pattern = @"(http://|https://)\S+";
            MatchCollection matches = Regex.Matches(fileContent, pattern);

            // 链接数标签重置
            onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";
            onlineLinkstextbox.Text = "./Links.txt";

            // 具体内容重置
            onlineLinksFromFile.Clear();
            onlineLinksFromFile.AddRange(lines);

            // 重置列表框
            if (param == -1)
            {
                for (int i = 0; i < onlineFileList.Items.Count; i++)
                {
                    onlineFileList.SetItemChecked(i, false);
                }

                foreach (var kvp in LinksName)
                {
                    List<string> keys = fileTypeNames.Keys.ToList();

                    var index = keys.IndexOf(kvp);

                    if (index != -1)
                    {
                        onlineFileList.SetItemChecked(index, true);
                    }
                }
            }
            onlineFileList.ItemCheck += onlineFileList_ItemCheck;
        }

        // 列表框内选项状态更改
        private void onlineFileList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string selectedItem = onlineFileList.Items[e.Index].ToString();
            string itemText = onlineFileList.Items[e.Index].ToString();

            if (itemText.StartsWith("——"))
            {
                e.NewValue = e.CurrentValue;
                return;
            }

            bool isChecked = (e.NewValue == CheckState.Checked);
            if (isChecked)
            {
                bool isAdded = AddLinkToFileIfNotExists(fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
                if (!isAdded)
                {
                    MessageBox.Show("添加失败");
                }
            }
            else
            {
                bool isRemoved = RemoveLinkFromFileIfExists(fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
                if (!isRemoved)
                {
                    MessageBox.Show("删除失败");
                }
            }

            RefreshOnlineRelatedComponents(0);
        }

        // 重置在线文件链接文件
        private void btnReloadOnline_Click(object sender, EventArgs e)
        {
            GetDefaultLinksList();

            try
            {
                File.WriteAllLines(LinksFilePath, onlineItemFileLinks);
                MessageBox.Show("重置成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重置时发生错误: {ex.Message}");
            }

            RefreshOnlineRelatedComponents();
        }

        // 点击在线链接数标签后重置各种相关状态
        private void onlineLinkCountLabel_Click(object sender, EventArgs e)
        {
            if (!File.Exists(LinksFilePath))
            {
                onlineLinkstextbox.Text = "读取错误，请重置！";
                return;
            }

            RefreshOnlineRelatedComponents();
        }

        // 替换为国内镜像链接 (gitmirror)
        private void 国内镜像链接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceDomainInFile();
        }

        // 禁用/启用 所有按钮 (防止误操作)
        private void disableAllbtns()
        {
            btnConvert.Enabled = false;
            btnBrowseFolder.Enabled = false;
            btnBrowseOnlineFiles.Enabled = false;
            desFormatCombo.Enabled = false;
            onlineFileLinkEdit.Enabled = false;
        }

        private void enableAllbtns()
        {
            btnConvert.Enabled = true;
            btnBrowseFolder.Enabled = true;
            btnBrowseOnlineFiles.Enabled = true;
            desFormatCombo.Enabled = true;
            onlineFileLinkEdit.Enabled = true;
        }

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN");
        }
    }
}