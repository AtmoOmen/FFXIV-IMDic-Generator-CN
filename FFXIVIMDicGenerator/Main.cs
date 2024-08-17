using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TinyPinyin;

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

    private void btnBrowseFolder_Click(object sender, EventArgs e)
    {
        using var folderDialog = new FolderBrowserDialog();
        DisableAllButtons();
        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtFolderPath.Text = folderDialog.SelectedPath;
            var csvFileCount = Directory.EnumerateFiles(folderDialog.SelectedPath, "*.csv", SearchOption.AllDirectories).Count();
            localFileCountLabel.Text = $"当前文件数: {csvFileCount}";
        }
        EnableAllButtons();
    }

    private async void btnConvert_Click(object sender, EventArgs e)
    {
        if (!ValidateConversionParameters()) return;

        try
        {
            DisableAllButtons();
            await ProcessFolder(txtFolderPath.Text, GetDesConvertType(desFormatCombo.SelectedItem.ToString()));
            MessageBox.Show("处理完成！");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"处理时发生错误: {ex.Message}");
        }
        finally
        {
            EnableAllButtons();
            handleGroup.Text = "生成";
        }
    }

    private bool ValidateConversionParameters()
    {
        if (!Directory.Exists(txtFolderPath.Text))
        {
            MessageBox.Show("请选择有效的文件夹");
            return false;
        }

        var format = GetDesConvertType(desFormatCombo.SelectedItem?.ToString());
        if (string.IsNullOrEmpty(format) || format == "未知")
        {
            MessageBox.Show("请选择有效的格式转换类型");
            return false;
        }

        return true;
    }

    private async Task ProcessFolder(string folderPath, string format)
    {
        var allData = new List<string>();
        var csvFiles = Directory.GetFiles(folderPath, "*.csv");

        if (csvFiles.Length == 0) throw new InvalidOperationException("文件夹中没有找到任何 CSV 文件");

        await Task.WhenAll(csvFiles.Select(csvFile => ProcessCsvFile(csvFile, allData)));

        await File.WriteAllLinesAsync(_outputFilePath, allData, Encoding.UTF8);
        OpenConvertCmd(format);
        Utility.OpenFile(Environment.CurrentDirectory);
    }

    private async void btnBrowseOnlineFiles_Click(object sender, EventArgs e)
    {
        if (!IsValidFormatSelected())
        {
            MessageBox.Show("请选择有效的格式转换类型");
            return;
        }

        try
        {
            DisableAllButtons();
            progressBar.Minimum = 0;
            progressBar.Maximum = _onlineLinksFromFile.Count;
            progressBar.Value   = 0;

            var allData = await ProcessOnlineFiles(new Progress<int>(UpdateProgressBar));

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
            EnableAllButtons();
            handleGroup.Text = "生成";
            Utility.OpenFile(Environment.CurrentDirectory);
        }
    }
    
    private async Task<List<string>> ProcessOnlineFiles(IProgress<int> progress)
    {
        var allData        = new List<string>();
        var processedCount = 0;

        foreach (var link in _onlineLinksFromFile.Where(link => !string.IsNullOrEmpty(link)))
        {
            await ProcessCsvFile(link, allData);
            processedCount++;
            progress.Report(processedCount);
        }

        return allData;
    }

    private async Task ProcessCsvFile(string filePath, List<string> allData)
    {
        handleGroup.Text = $"处理中: {Path.GetFileName(filePath)}";

        var uniqueNames = new HashSet<string>();
        var rowCount    = 0;

        await foreach (var row in Utility.ReadCsvFileAsync(filePath))
        {
            rowCount++;
            if (rowCount == 1) continue; // Skip header row

            if (rowCount == 2)
            {
                var columnIndices = _keywords
                    .Select(keyword => Utility.FindColumnIndex(row, keyword))
                    .Where(columnIndex => columnIndex != -1)
                    .ToList();

                ProcessRow(row, columnIndices, uniqueNames);
                continue;
            }

            var indices = _keywords
                .Select(keyword => Utility.FindColumnIndex(row, keyword))
                .Where(columnIndex => columnIndex != -1)
                .ToList();

            ProcessRow(row, indices, uniqueNames);
        }

        if (rowCount < 2) return;

        var pinyinDictionary = new ConcurrentDictionary<string, string>(
            uniqueNames.AsParallel().ToDictionary(
                name => name,
                name => $"'{PinyinHelper.GetPinyin(name, "'").ToLowerInvariant()}"
            )
        );

        allData.AddRange(pinyinDictionary.Select(kvp => $"{kvp.Value} {kvp.Key}"));
        handleGroup.Text = $"处理完成: {Path.GetFileName(filePath)}";
    }
    
    private async Task ProcessCsvFile(string filePath, List<string> allData, IProgress<int> progress, int currentFile, int totalFiles)
    {
        UpdateHandleGroupText($"处理中: {Path.GetFileName(filePath)} ({currentFile + 1}/{totalFiles})");

        var uniqueNames = new HashSet<string>();
        var rowCount = 0;
        var totalRows = await CountCsvRows(filePath);

        await foreach (var row in Utility.ReadCsvFileAsync(filePath))
        {
            rowCount++;
            if (rowCount == 1) continue; // Skip header row

            if (rowCount == 2)
            {
                var columnIndices = _keywords
                    .Select(keyword => Utility.FindColumnIndex(row, keyword))
                    .Where(columnIndex => columnIndex != -1)
                    .ToList();

                ProcessRow(row, columnIndices, uniqueNames);
                continue;
            }

            var indices = _keywords
                .Select(keyword => Utility.FindColumnIndex(row, keyword))
                .Where(columnIndex => columnIndex != -1)
                .ToList();

            ProcessRow(row, indices, uniqueNames);

            // 更新子进度
            var subProgress = (double)rowCount / totalRows;
            var overallProgress = (currentFile + subProgress) / totalFiles;
            progress.Report((int)(overallProgress * 100));
        }

        if (rowCount < 2) return;

        var pinyinDictionary = new ConcurrentDictionary<string, string>(
            uniqueNames.AsParallel().ToDictionary(
                name => name,
                name => $"'{PinyinHelper.GetPinyin(name, "'").ToLowerInvariant()}"
            )
        );

        allData.AddRange(pinyinDictionary.Select(kvp => $"{kvp.Value} {kvp.Key}"));
        UpdateHandleGroupText($"处理完成: {Path.GetFileName(filePath)} ({currentFile + 1}/{totalFiles})");
    }

    private static void ProcessRow(string[] row, List<int> columnIndices, HashSet<string> uniqueNames)
    {
        foreach (var columnIndex in columnIndices)
        {
            if (columnIndex >= row.Length) continue;
            var name = Regex.Replace(row[columnIndex].Trim(), @"[^\u4e00-\u9fa5]", "");
            if (!string.IsNullOrWhiteSpace(name) && name.Length > 1)
            {
                uniqueNames.Add(name);
            }
        }
    }

    private void UpdateHandleGroupText(string text)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<string>(UpdateHandleGroupText), text);
            return;
        }

        handleGroup.Text = text;
    }

    private async Task<int> CountCsvRows(string filePath)
    {
        var count = 0;
        await foreach (var _ in Utility.ReadCsvFileAsync(filePath))
        {
            count++;
        }
        return count;
    }

    private void UpdateProgressBar(int value)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<int>(UpdateProgressBar), value);
            return;
        }

        progressBar.Value = value;
    }

    private bool IsValidFormatSelected() => 
        GetDesConvertType(desFormatCombo.SelectedItem?.ToString()) is { } format && format != "未知";

    private async Task<List<string>> ProcessOnlineFiles()
    {
        var allData = new List<string>();
        var tasks = _onlineLinksFromFile
            .Where(link => !string.IsNullOrEmpty(link))
            .Select(link => ProcessCsvFile(link, allData));

        await Task.WhenAll(tasks);
        progressBar.Value = _onlineLinksFromFile.Count;
        return allData;
    }

    private void OnlineFileLinkEdit_Click(object sender, EventArgs e) => 
        Utility.OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));

    private void RefreshOnlineRelatedComponents(int param = -1)
    {
        onlineFileList.ItemCheck -= onlineFileList_ItemCheck;

        var fileContent = File.ReadAllText(LinksFilePath);
        _linksName = GetFileNamesFromLinksFile(LinksFilePath);
        var lines = File.ReadAllLines(LinksFilePath);

        UpdateOnlineLinkCount(fileContent);
        UpdateOnlineLinksFromFile(lines);
        UpdateOnlineFileList(param);

        onlineFileList.ItemCheck += onlineFileList_ItemCheck;
    }

    private void UpdateOnlineLinkCount(string fileContent)
    {
        var linkCount = Regex.Matches(fileContent, @"(http://|https://)\S+").Count;
        onlineLinkCountLabel.Text = $"当前链接数: {linkCount}";
        onlineLinkstextbox.Text = "./Links.txt";
    }

    private void UpdateOnlineLinksFromFile(string[] lines)
    {
        _onlineLinksFromFile.Clear();
        _onlineLinksFromFile.AddRange(lines);
    }

    private void UpdateOnlineFileList(int param)
    {
        if (param != -1) return;

        for (var i = 0; i < onlineFileList.Items.Count; i++)
        {
            onlineFileList.SetItemChecked(i, false);
        }

        foreach (var fileName in _linksName)
        {
            var index = _fileTypeNames.Keys.ToList().IndexOf(fileName);
            if (index != -1)
            {
                onlineFileList.SetItemChecked(index, true);
            }
        }
    }

    private void onlineFileList_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        var itemText = onlineFileList.Items[e.Index].ToString();

        if (itemText.StartsWith("——"))
        {
            e.NewValue = e.CurrentValue;
            return;
        }

        var fileName = _fileTypeNames.FirstOrDefault(x => x.Value == itemText).Key;
        var isChecked = e.NewValue == CheckState.Checked;

        bool operationSuccess = isChecked ? AddLinkToFileIfNotExists(fileName) : RemoveLinkFromFileIfExists(fileName);

        if (!operationSuccess)
        {
            MessageBox.Show(isChecked ? "添加失败" : "删除失败");
        }

        RefreshOnlineRelatedComponents(0);
    }

    private void btnReloadOnline_Click(object sender, EventArgs e)
    {
        try
        {
            GetDefaultLinksList();
            File.WriteAllLines(LinksFilePath, _onlineItemFileLinks);
            MessageBox.Show("重置成功");
            RefreshOnlineRelatedComponents();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"重置时发生错误: {ex.Message}");
        }
    }

    private void OnlineLinkCountLabel_Click(object sender, EventArgs e)
    {
        if (!File.Exists(LinksFilePath))
        {
            onlineLinkstextbox.Text = "读取错误，请重置！";
            return;
        }

        RefreshOnlineRelatedComponents();
    }

    private void 国内镜像链接ToolStripMenuItem_Click(object sender, EventArgs e) => ReplaceDomainInFile();

    private void DisableAllButtons()
    {
        btnConvert.Enabled = btnBrowseFolder.Enabled = btnBrowseOnlineFiles.Enabled = 
        desFormatCombo.Enabled = onlineFileLinkEdit.Enabled = false;
    }

    private void EnableAllButtons()
    {
        btnConvert.Enabled = btnBrowseFolder.Enabled = btnBrowseOnlineFiles.Enabled = 
        desFormatCombo.Enabled = onlineFileLinkEdit.Enabled = true;
    }

    private void ffxivdataminingcnToolStripMenuItem1_Click(object sender, EventArgs e) => 
        Utility.OpenFile("https://github.com/thewakingsands/ffxiv-datamining-cn");

    private void 深蓝词库转换ToolStripMenuItem1_Click(object sender, EventArgs e) =>
        Utility.OpenFile("https://github.com/studyzy/imewlconverter");

    private void csv文件内容参考ToolStripMenuItem_Click(object sender, EventArgs e) =>
        Utility.OpenFile("https://github.com/Souma-Sumire/FFXIVChnTextPatch-Souma/wiki/CSV%E6%96%87%E4%BB%B6");

    private void GitHubToolStripMenuItem_Click(object sender, EventArgs e) =>
        Utility.OpenFile("https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN");
    
    private static void OpenConvertCmd(string outputType)
    {
        var converterPath = Path.Combine(Environment.CurrentDirectory, "ConvertProgram", "ImeWlConverterCmd.dll");
        const string inputType = "sgpy";
        var inputPaths = Path.Combine(Environment.CurrentDirectory, "output.txt");
        var outputPath = Path.Combine(Environment.CurrentDirectory, "output.txt");
        var commandLineArgs = $"-i:{inputType} {inputPaths} -o:{outputType} {outputPath}";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"{converterPath} {commandLineArgs}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi);
            process?.WaitForExit();
            process?.StandardOutput.ReadToEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"格式转换过程中发生错误：{ex.Message}");
        }
    }

    private static string GetDesConvertType(string sourceName) 
        => _desTypes.GetValueOrDefault(sourceName, "未知");

    private List<string?> GetFileNamesFromLinksFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            try
            {
                GetDefaultLinksList();
                File.WriteAllLines(filePath, _onlineItemFileLinks);
                RefreshOnlineRelatedComponents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"写入文件时发生错误: {ex.Message}");
            }
        }

        try
        {
            return File.ReadAllLines(filePath)
                .Select(line => line.Split('/').LastOrDefault())
                .Where(fileName => !string.IsNullOrEmpty(fileName))
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show("发生错误: " + ex.Message);
            return new List<string?>();
        }
    }

    private void InitializeCheckedBox()
    {
        onlineFileList.ItemCheck -= onlineFileList_ItemCheck;

        try
        {
            if (!File.Exists(LinksFilePath))
            {
                GetDefaultLinksList();
                File.WriteAllLines(LinksFilePath, _onlineItemFileLinks);
            }

            _onlineLinksFromFile.AddRange(File.ReadAllLines(LinksFilePath));

            onlineLinkstextbox.Text = "./Links.txt";

            var fileContent = File.ReadAllText(LinksFilePath);
            var linkCount = Regex.Matches(fileContent, @"(http://|https://)\S+").Count;
            onlineLinkCountLabel.Text = $"当前链接数: {linkCount}";

            sourceFormatCombo.SelectedIndex = desFormatCombo.SelectedIndex = 0;

            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            onlineFileList.Items.AddRange(_fileTypeNames.Values.ToArray<object>());

            var linksName = GetFileNamesFromLinksFile(LinksFilePath);
            foreach (var fileName in linksName)
            {
                var index = _fileTypeNames.Keys.ToList().IndexOf(fileName);
                if (index != -1)
                {
                    onlineFileList.SetItemChecked(index, true);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"初始化时发生错误: {ex.Message}");
        }

        onlineFileList.ItemCheck += onlineFileList_ItemCheck;
    }

    private static bool AddLinkToFileIfNotExists(string fileName)
    {
        var link = _cnMirrorReplace
            ? $"https://raw.gitmirror.com/thewakingsands/ffxiv-datamining-cn/master/{fileName}"
            : $"https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/{fileName}";

        try
        {
            var lines = File.ReadAllLines(LinksFilePath).ToList();
            var pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
            if (!lines.Any(line => Regex.IsMatch(line, pattern)))
            {
                lines.Add(link);
                File.WriteAllLines(LinksFilePath, lines);
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("发生错误: " + ex.Message);
        }

        return false;
    }

    private static bool RemoveLinkFromFileIfExists(string fileName)
    {
        try
        {
            var lines = File.ReadAllLines(LinksFilePath).ToList();
            var pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
            var indexToRemove = lines.FindIndex(line => Regex.IsMatch(line, pattern));

            if (indexToRemove != -1)
            {
                lines.RemoveAt(indexToRemove);
                File.WriteAllLines(LinksFilePath, lines);
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("发生错误: " + ex.Message);
        }

        return false;
    }

    private static async Task GetDefaultLinksList()
    {
        const string fileUrl = "https://raw.githubusercontent.com/AtmoOmen/FFXIV-IMDic-Generator-CN/main/Assest/defaultLinks.txt";

        try
        {
            var response = await HttpClient.GetAsync(fileUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _onlineItemFileLinks = new List<string> { content };
            }
            else
            {
                MessageBox.Show($"获取默认在线文件时发生错误: {response.StatusCode}\n使用本地默认文件");
            }
        }
        catch (Exception e)
        {
            MessageBox.Show($"发生网络错误: {e.Message}\n使用本地默认文件");
        }
    }

    private void ReplaceDomainInFile()
    {
        try
        {
            if (!File.Exists(LinksFilePath)) return;
            var lines = File.ReadAllLines(LinksFilePath);

            const string newDomain = "raw.gitmirror.com";
            const string oldDomain = "raw.githubusercontent.com";

            var replacedLines = lines.Select(line => Regex.Replace(line, $@"https://(.*?){Regex.Escape(oldDomain)}",
                $"https://$1{newDomain}")).ToArray();

            File.WriteAllLines(LinksFilePath, replacedLines);

            RefreshOnlineRelatedComponents();

            _cnMirrorReplace = true;

            MessageBox.Show("域名替换完成并写入文件成功！");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"发生错误：{ex.Message}");
        }
    }

    private static void AnalyzeDomains()
    {
        try
        {
            var lines = File.ReadAllLines(LinksFilePath);
            var domainCount = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                if (!Uri.TryCreate(line, UriKind.Absolute, out var uri)) continue;
                var domain = uri.Host.StartsWith("www.") ? uri.Host[4..] : uri.Host;
                domainCount[domain] = domainCount.TryGetValue(domain, out var count) ? count + 1 : 1;
            }

            if (domainCount.TryGetValue("raw.githubusercontent.com", out var rawGithubCount))
            {
                _cnMirrorReplace = domainCount.Values.Max() > rawGithubCount;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生异常: {ex.Message}");
        }
    }
}