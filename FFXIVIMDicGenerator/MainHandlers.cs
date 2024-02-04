using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Text.RegularExpressions;
using TinyPinyin;

namespace FFXIVIMDicGenerator;

public partial class Main : Form
{
    private async Task ProcessCsvFile(string filePath, List<string> allData)
    {
        handleGroup.Text = $"处理中: {Path.GetFileName(filePath)}";
        var rows = await ReadCsvFile(filePath);
        if (rows.Count < 2) return;

        var columnIndices = _keywords.Select(keyword => FindColumnIndex(rows[1], keyword))
            .Where(columnIndex => columnIndex != -1)
            .ToList();

        var uniqueNames = new HashSet<string>();
        foreach (var columnIndex in columnIndices) uniqueNames.UnionWith(ExtractNames(rows, columnIndex));

        var stringBuilder = new StringBuilder();

        var pinyinMap = uniqueNames.AsParallel().ToDictionary(
            name => name,
            name =>
            {
                var pinyin = PinyinHelper.GetPinyin(name, "'");
                stringBuilder.Clear();
                stringBuilder.Append('\'').Append(pinyin.ToLower());
                return stringBuilder.ToString();
            }
        );

        var pinyinDictionary = new ConcurrentDictionary<string, string>(pinyinMap);

        allData.AddRange(pinyinDictionary.Select(kvp => $"{kvp.Value} {kvp.Key}"));
        handleGroup.Text = $"处理完成: {Path.GetFileName(filePath)}";
    }


    private static async Task<List<string[]>> ReadCsvFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return null;

        try
        {
            await using var stream = filePath.StartsWith("http")
                ? await HttpClient.GetStreamAsync(filePath)
                : File.OpenRead(filePath);
            using StreamReader reader = new(stream, Encoding.UTF8);
            var rows = new List<string[]>();
            while (await reader.ReadLineAsync() is { } line)
                rows.Add(line.Split(','));
            return rows;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"读取文件时发生错误: {ex.Message}");
            return null;
        }
    }

    private static IEnumerable<string> ExtractNames(IEnumerable<string[]> rows, int columnIndex)
    {
        return rows.Where(row => row.Length > columnIndex)
            .Select(row => Regex.Replace(row[columnIndex].Trim(), @"[^\u4e00-\u9fa5]", ""))
            .Where(name => !string.IsNullOrWhiteSpace(name) && name.Length > 1)
            .ToList();
    }

    private static int FindColumnIndex(string[] headerRow, string columnName)
    {
        return Array.FindIndex(headerRow, header => header.Trim() == columnName);
    }


    private static void OpenFolder(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
                Process.Start("explorer.exe", folderPath);
            else
                MessageBox.Show($"文件夹 '{folderPath}' 不存在。");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"打开文件夹时发生错误: {ex.Message}");
        }
    }

    private static void OpenFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            else
                Console.WriteLine($"文件 '{filePath}' 不存在。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"打开文件时发生错误: {ex.Message}");
        }
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"打开网址时发生错误: {ex.Message}");
        }
    }

    private static void OpenConvertCmd(string outputType)
    {
        var converterPath = Path.Combine(Environment.CurrentDirectory, "ConvertProgram", "ImeWlConverterCmd.dll");
        var inputType = "sgpy";
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

        var process = new Process
        {
            StartInfo = psi
        };

        try
        {
            process.Start();

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"格式转换过程中发生错误：{ex.Message}");
        }
    }

    private string GetDesConvertType(string sourceName)
    {
        return _desTypes.GetValueOrDefault(sourceName, "未知");
    }

    private List<string> GetFileNamesFromLinksFile(string filePath)
    {
        if (!File.Exists(filePath))
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

        var fileNames = new List<string>();

        try
        {
            var lines = File.ReadAllLines(filePath);

            fileNames.AddRange(from line in lines select line.Split('/') into parts where parts.Length > 0 select parts[^1]);
        }
        catch (Exception ex)
        {
            MessageBox.Show("发生错误: " + ex.Message);
        }

        return fileNames;
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

            if (File.Exists(LinksFilePath)) _onlineLinksFromFile.AddRange(File.ReadAllLines(LinksFilePath));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"操作文件时发生错误: {ex.Message}");
        }

        onlineLinkstextbox.Text = "./Links.txt";

        var fileContent = File.ReadAllText(LinksFilePath);

        var pattern = @"(http://|https://)\S+";
        var matches = Regex.Matches(fileContent, pattern);

        onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";

        sourceFormatCombo.SelectedIndex = desFormatCombo.SelectedIndex = 0;

        MaximizeBox = false;
        MinimizeBox = true;
        FormBorderStyle = FormBorderStyle.FixedSingle;

        foreach (var kvp in _fileTypeNames) onlineFileList.Items.Add(kvp.Value);

        _linksName = GetFileNamesFromLinksFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));

        foreach (var kvp in _linksName)
        {
            var keys = _fileTypeNames.Keys.ToList();

            var index = keys.IndexOf(kvp);

            if (index != -1) onlineFileList.SetItemChecked(index, true);
        }

        onlineFileList.ItemCheck += onlineFileList_ItemCheck;
    }

    private bool AddLinkToFileIfNotExists(string fileName)
    {
        var link = string.Empty;

        if (_cnMirrorReplace)
            link = "https://raw.gitmirror.com/thewakingsands/ffxiv-datamining-cn/master/" + fileName;
        else
            link = "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/" + fileName;

        try
        {
            var pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
            var regex = new Regex(pattern);
            var lines = File.ReadAllLines(LinksFilePath).ToList();

            if (!lines.Any(line => regex.IsMatch(line)))
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

    private bool RemoveLinkFromFileIfExists(string fileName)
    {
        try
        {
            var lines = File.ReadAllLines(LinksFilePath).ToList();

            var pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
            var regex = new Regex(pattern);

            var indexToRemove = lines.FindIndex(line => regex.IsMatch(line));

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

    // 在线获取默认链接
    private void GetDefaultLinksList()
    {
        const string fileUrl = "https://raw.githubusercontent.com/AtmoOmen/FFXIV-IMDic-Generator-CN/main/Assest/defaultLinks.txt";

        var onlineContents = new List<string>();

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            var response = httpClient.GetAsync(fileUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                onlineContents.Add(content);
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

        if (onlineContents.Count > 0) _onlineItemFileLinks = onlineContents;
    }

    // 批量替换域名
    private void ReplaceDomainInFile()
    {
        try
        {
            if (!File.Exists(LinksFilePath)) return;
            var filePath = LinksFilePath;
            var lines = File.ReadAllLines(filePath);

            const string newDomain = "raw.gitmirror.com";
            const string oldDomain = "raw.githubusercontent.com";

            var replacedLines = lines.Select(line => Regex.Replace(line, $@"https://(.*?){Regex.Escape(oldDomain)}",
                $"https://$1{newDomain}")).ToArray();

            File.WriteAllLines(filePath, replacedLines);

            RefreshOnlineRelatedComponents();

            while (File.ReadAllLines(filePath).Length == 0) File.WriteAllLines(filePath, replacedLines);

            _cnMirrorReplace = true;

            MessageBox.Show("域名替换完成并写入文件成功！");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"发生错误：{ex.Message}");
        }
    }

    // 检测 Links.txt 域名 (初始化用，不想写持久化配置的替代方案)
    public void AnalyzeDomains()
    {
        try
        {
            var lines = File.ReadAllLines(LinksFilePath);

            var domainCount = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                if (!Uri.TryCreate(line, UriKind.Absolute, out var uri)) continue;
                var domain = uri.Host;

                // 移除www前缀（如果有的话）
                if (domain.StartsWith("www.")) domain = domain[4..];

                if (!domainCount.TryAdd(domain, 1))
                    domainCount[domain]++;
            }

            // 查找raw.githubusercontent.com域名的计数
            if (!domainCount.ContainsKey("raw.githubusercontent.com")) return;
            var rawGithubCount = domainCount["raw.githubusercontent.com"];

            _cnMirrorReplace = domainCount.Values.Max() > rawGithubCount;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生异常: {ex.Message}");
        }
    }

    // 特殊目的用方法
    public static void ShowStringList(List<string>? stringList)
    {
        if (stringList == null || stringList.Count == 0)
        {
            MessageBox.Show("列表为空");
            return;
        }

        var message = string.Join("\n", stringList);
        MessageBox.Show(message, "String列表内容", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // 特殊目的用方法
    private List<string> ExtractNames1(List<string[]> rows, int columnIndex)
    {
        var names = (from row in rows where row.Length > columnIndex select row[columnIndex].Trim() into name select Regex.Replace(name, @"[^\u4e00-\u9fa5]", "") into name where !string.IsNullOrWhiteSpace(name) && name.Length > 1 select name).ToList();

        return names.Count == 0 ? null : names;
    }

    // 特殊目的用方法
    private async Task ProcessCsvFilesAndCopyToClipboard(IEnumerable<string> filePaths)
    {
        var matchingFileNames = new List<string>();

        foreach (var filePath in filePaths)
        {
            handleGroup.Text = $"正在处理文件: {Path.GetFileName(filePath)}";
            var rows = await ReadCsvFile(filePath);

            if (rows.Count < 2) continue;
            var skipFile = false;

            foreach (var names in from keyword in _keywords select FindColumnIndex(rows[1], keyword) into columnIndex where columnIndex != -1 select ExtractNames1(rows, columnIndex))
            {
                if (names == null || names.Count == 0)
                {
                    skipFile = true;
                    break;
                }

                matchingFileNames.Add(Path.GetFileName(filePath));
            }

            if (skipFile) continue;
        }

        if (matchingFileNames.Count > 0)
        {
            var fileNamesToCopy = string.Join(Environment.NewLine, matchingFileNames);
            Clipboard.SetText(fileNamesToCopy);
        }

        handleGroup.Text = "处理完成";
    }

    // 特殊目的用方法
    public static void CopyTranslationsToClipboard(string fileNamesPath, string translationsPath)
    {
        try
        {
            var fileNames = File.ReadAllLines(fileNamesPath);

            var translations = new Dictionary<string, string>();

            var translationLines = File.ReadAllLines(translationsPath);
            foreach (var line in translationLines)
            {
                var parts = line.Split(new[] { ' ' }, 2);
                if (parts.Length == 2) translations[parts[0]] = parts[1];
            }

            var clipboardText = new StringBuilder();

            foreach (var fileName in fileNames)
                if (translations.TryGetValue(fileName, out var translation))
                    clipboardText.AppendLine($"{fileName} {translation}");

            if (clipboardText.Length > 0)
            {
                Clipboard.SetText(clipboardText.ToString());
                MessageBox.Show("匹配的文件名和翻译已复制到剪贴板。");
            }
            else
            {
                MessageBox.Show("没有找到匹配的文件名和翻译。");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"发生错误：{ex.Message}");
        }
    }
}