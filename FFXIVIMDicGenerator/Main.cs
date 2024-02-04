using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVIMDicGenerator;

public partial class Main : Form
{
    public Main()
    {
        InitializeComponent();

        Text += LocalVersion;

        InitializeCheckedBox();

        AnalyzeDomains();
    }

    // 浏览本地文件夹
    private void btnBrowseFolder_Click(object sender, EventArgs e)
    {
        using var folderDialog = new FolderBrowserDialog();
        DisableAllbtns();
        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtFolderPath.Text = folderDialog.SelectedPath;
            var csvFileCount = Directory
                .EnumerateFiles(folderDialog.SelectedPath, "*.csv", SearchOption.AllDirectories).Count();
            localFileCountLabel.Text = $"当前文件数: {csvFileCount}";
        }

        EnableAllbtns();
    }

    // 本地转换功能
    private async void btnConvert_Click(object sender, EventArgs e)
    {
        if (!IsValidFolderPath(txtFolderPath.Text))
        {
            MessageBox.Show("请选择有效的文件夹");
            return;
        }

        var format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
        if (string.IsNullOrEmpty(format) || format == "未知")
        {
            MessageBox.Show("请选择有效的格式转换类型");
            return;
        }

        try
        {
            DisableAllbtns();
            await ProcessFolder(txtFolderPath.Text, format);
            MessageBox.Show("处理完成！");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"处理时发生错误: {ex.Message}");
        }
        finally
        {
            EnableAllbtns();
            handleGroup.Text = "生成";
        }
    }

    private bool IsValidFolderPath(string folderPath)
    {
        return !string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath);
    }

    private async Task ProcessFolder(string folderPath, string format)
    {
        var allData = new List<string>();
        var csvFiles = Directory.GetFiles(folderPath, "*.csv");

        if (csvFiles.Length == 0) throw new InvalidOperationException("文件夹中没有找到任何 CSV 文件");

        var tasks = csvFiles.Select(csvFile => ProcessCsvFile(csvFile, allData)).ToArray();
        await Task.WhenAll(tasks);

        await File.WriteAllLinesAsync(_outputFilePath, allData, Encoding.UTF8);
        OpenConvertCmd(format);
        OpenFolder(Environment.CurrentDirectory);
    }


    // 在线文件生成功能
    private async void btnBrowseOnlineFiles_Click(object sender, EventArgs e)
    {
        if (!IsValidFormatSelected())
        {
            MessageBox.Show("请选择有效的格式转换类型");
            return;
        }

        try
        {
            DisableAllbtns();
            progressBar.Maximum = _onlineLinksFromFile.Count;
            progressBar.Value = 0;

            var allData = await ProcessOnlineFiles();

            await File.WriteAllLinesAsync(_outputFilePath, allData, Encoding.UTF8);
            OpenConvertCmd(GetDesConvertType(desFormatCombo.SelectedItem.ToString()));
            MessageBox.Show($"处理完成，共 {allData.Count} 条\n输出文件位于: {_outputFilePath}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"处理时发生错误: {ex.Message}");
        }
        finally
        {
            EnableAllbtns();
            handleGroup.Text = "生成";
            OpenFolder(Environment.CurrentDirectory);
        }
    }

    private bool IsValidFormatSelected()
    {
        var format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
        return !string.IsNullOrEmpty(format) && format != "未知";
    }

    private async Task<List<string>> ProcessOnlineFiles()
    {
        var allData = new List<string>();

        var tasks = _onlineLinksFromFile.Where(link => !string.IsNullOrEmpty(link))
            .Select(link => ProcessCsvFile(link, allData))
            .ToList();

        await Task.WhenAll(tasks);

        progressBar.Value = _onlineLinksFromFile.Count;

        return allData;
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
    private void OnlineFileLinkEdit_Click(object sender, EventArgs e)
    {
        OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
    }

    // 刷新各种与在线文件生成功能相关的组件
    private void RefreshOnlineRelatedComponents(int param = -1)
    {
        onlineFileList.ItemCheck -= onlineFileList_ItemCheck;

        var fileContent = File.ReadAllText(LinksFilePath);
        _linksName = GetFileNamesFromLinksFile(LinksFilePath);
        var lines = File.ReadAllLines(LinksFilePath);

        const string pattern = @"(http://|https://)\S+";
        var matches = Regex.Matches(fileContent, pattern);

        // 链接数标签重置
        onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";
        onlineLinkstextbox.Text = "./Links.txt";

        // 具体内容重置
        _onlineLinksFromFile.Clear();
        _onlineLinksFromFile.AddRange(lines);

        // 重置列表框
        if (param == -1)
        {
            for (var i = 0; i < onlineFileList.Items.Count; i++) onlineFileList.SetItemChecked(i, false);

            foreach (var index in from kvp in _linksName let keys = _fileTypeNames.Keys.ToList() select keys.IndexOf(kvp) into index where index != -1 select index)
            {
                onlineFileList.SetItemChecked(index, true);
            }
        }

        onlineFileList.ItemCheck += onlineFileList_ItemCheck;
    }

    // 列表框内选项状态更改
    private void onlineFileList_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        var selectedItem = onlineFileList.Items[e.Index].ToString();
        var itemText = onlineFileList.Items[e.Index].ToString();

        if (itemText.StartsWith("——"))
        {
            e.NewValue = e.CurrentValue;
            return;
        }

        var isChecked = e.NewValue == CheckState.Checked;
        if (isChecked)
        {
            var isAdded = AddLinkToFileIfNotExists(_fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
            if (!isAdded) MessageBox.Show("添加失败");
        }
        else
        {
            var isRemoved = RemoveLinkFromFileIfExists(_fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
            if (!isRemoved) MessageBox.Show("删除失败");
        }

        RefreshOnlineRelatedComponents(0);
    }

    // 重置在线文件链接文件
    private void btnReloadOnline_Click(object sender, EventArgs e)
    {
        GetDefaultLinksList();

        try
        {
            File.WriteAllLines(LinksFilePath, _onlineItemFileLinks);
            MessageBox.Show("重置成功");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"重置时发生错误: {ex.Message}");
        }

        RefreshOnlineRelatedComponents();
    }

    // 点击在线链接数标签后重置各种相关状态
    private void OnlineLinkCountLabel_Click(object sender, EventArgs e)
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
    private void DisableAllbtns()
    {
        btnConvert.Enabled = false;
        btnBrowseFolder.Enabled = false;
        btnBrowseOnlineFiles.Enabled = false;
        desFormatCombo.Enabled = false;
        onlineFileLinkEdit.Enabled = false;
    }

    private void EnableAllbtns()
    {
        btnConvert.Enabled = true;
        btnBrowseFolder.Enabled = true;
        btnBrowseOnlineFiles.Enabled = true;
        desFormatCombo.Enabled = true;
        onlineFileLinkEdit.Enabled = true;
    }

    private void GitHubToolStripMenuItem_Click(object sender, EventArgs e)
    {
        OpenUrl("https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN");
    }
}