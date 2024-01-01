using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Text.RegularExpressions;
using TinyPinyin;

namespace FFXIVIMDicGenerator
{
    public partial class Main : Form
    {
        private void StartUpdateProgram()
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, "Update", "UpdateProgram.exe"));

            using NamedPipeServerStream pipeServer = new("FFXIVIMDICGENERATORLocalVersionPipe", PipeDirection.Out);
            pipeServer.WaitForConnection();
            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                writer.WriteLine(LocalVersion);
            }
        }

        private async Task ProcessCsvFile(string filePath, List<string> allData)
        {
            handleGroup.Text = $"处理中: {Path.GetFileName(filePath)}";
            var rows = await ReadCsvFile(filePath);
            if (rows == null || rows.Count < 2) return;

            var pinyinDictionary = _keywords.AsParallel().SelectMany(keyword =>
            {
                int columnIndex = FindColumnIndex(rows[1], keyword);
                return columnIndex != -1 ? ExtractNames(rows, columnIndex) : Enumerable.Empty<string>();
            }).ToDictionary(name => name, name => "'" + PinyinHelper.GetPinyin(name, "'").ToLower());

            allData.AddRange(pinyinDictionary.Select(kvp => $"{kvp.Value} {kvp.Key}"));
            handleGroup.Text = $"处理完成: {Path.GetFileName(filePath)}";
        }

        private static async Task<List<string[]>> ReadCsvFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            try
            {
                using Stream stream = filePath.StartsWith("http") ?
                    await HttpClient.GetStreamAsync(filePath) : File.OpenRead(filePath);
                using StreamReader reader = new(stream, Encoding.UTF8);
                var rows = new List<string[]>();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                    rows.Add(line.Split(','));
                return rows;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取文件时发生错误: {ex.Message}");
                return null;
            }
        }

        private static List<string> ExtractNames(List<string[]> rows, int columnIndex)
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

        private static int RemoveDuplicates(string filePath)
        {
            try
            {
                var lines = new HashSet<string>(File.ReadAllLines(filePath, Encoding.UTF8));
                if (lines.Count == 0) return 0;

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                return lines.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理重复项时发生错误: {ex.Message}");
                return 0;
            }
        }


        private static void OpenFolder(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else
                {
                    MessageBox.Show($"文件夹 '{folderPath}' 不存在。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开文件夹时发生错误: {ex.Message}");
            }
        }

        private void OpenFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine($"文件 '{filePath}' 不存在。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"打开文件时发生错误: {ex.Message}");
            }
        }

        private void OpenUrl(string url)
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

        private void OpenConvertCmd(string outputType)
        {
            string converterPath = Path.Combine(Environment.CurrentDirectory, "ConvertProgram", "ImeWlConverterCmd.dll");
            string inputType = "sgpy";
            string inputPaths = Path.Combine(Environment.CurrentDirectory, "output.txt");
            string outputPath = Path.Combine(Environment.CurrentDirectory, "output.txt");
            string commandLineArgs = $"-i:{inputType} {inputPaths} -o:{outputType} {outputPath}";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{converterPath} {commandLineArgs}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = psi
            };

            try
            {
                process.Start();

                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"格式转换过程中发生错误：{ex.Message}");
            }
        }

        private string GetDesConvertType(string sourceName)
        {
            return _desTypes.TryGetValue(sourceName, out string? desType) ? desType : "未知";
        }

        private List<string> GetFileNamesFromLinksFile(string filePath)
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

            List<string> fileNames = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('/');
                    if (parts.Length > 0)
                    {
                        string fileName = parts[parts.Length - 1];
                        fileNames.Add(fileName);
                    }
                }
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

                if (File.Exists(LinksFilePath))
                {
                    _onlineLinksFromFile.AddRange(File.ReadAllLines(LinksFilePath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作文件时发生错误: {ex.Message}");
            }

            onlineLinkstextbox.Text = "./Links.txt";

            string fileContent = File.ReadAllText(LinksFilePath);

            string pattern = @"(http://|https://)\S+";
            MatchCollection matches = Regex.Matches(fileContent, pattern);

            onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";

            sourceFormatCombo.SelectedIndex = desFormatCombo.SelectedIndex = 0;

            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            foreach (var kvp in _fileTypeNames)
            {
                onlineFileList.Items.Add(kvp.Value);
            }

            _linksName = GetFileNamesFromLinksFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));

            foreach (var kvp in _linksName)
            {
                List<string> keys = _fileTypeNames.Keys.ToList();

                var index = keys.IndexOf(kvp);

                if (index != -1)
                {
                    onlineFileList.SetItemChecked(index, true);
                }
            }
            onlineFileList.ItemCheck += onlineFileList_ItemCheck;
        }

        private bool AddLinkToFileIfNotExists(string fileName)
        {
            var link = string.Empty;

            if (_cnMirrorReplace)
            {
                link = "https://raw.gitmirror.com/thewakingsands/ffxiv-datamining-cn/master/" + fileName;
            }
            else
            {
                link = "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/" + fileName;
            }

            try
            {
                string pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
                Regex regex = new Regex(pattern);
                List<string> lines = File.ReadAllLines(LinksFilePath).ToList();

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
                List<string> lines = File.ReadAllLines(LinksFilePath).ToList();

                string pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
                Regex regex = new Regex(pattern);

                int indexToRemove = lines.FindIndex(line => regex.IsMatch(line));

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
            var fileUrl = "https://raw.githubusercontent.com/AtmoOmen/FFXIV-IMDic-Generator-CN/main/Assest/defaultLinks.txt";

            var onlineContents = new List<string>();

            try
            {
                using (var httpClient = new HttpClient())
                {
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
            }
            catch (Exception e)
            {
                MessageBox.Show($"发生网络错误: {e.Message}\n使用本地默认文件");
            }

            if (onlineContents.Count > 0)
            {
                _onlineItemFileLinks = onlineContents;
            }
        }

        // 批量替换域名
        private void ReplaceDomainInFile()
        {
            try
            {
                if (File.Exists(LinksFilePath))
                {
                    string filePath = LinksFilePath;
                    string[] lines = File.ReadAllLines(filePath);

                    var newDomain = "raw.gitmirror.com";
                    var oldDomain = "raw.githubusercontent.com";

                    var replacedLines = lines.Select(line =>
                    {
                        return Regex.Replace(line, $@"https://(.*?){Regex.Escape(oldDomain)}", $"https://$1{newDomain}");
                    }).ToArray();

                    File.WriteAllLines(filePath, replacedLines);

                    RefreshOnlineRelatedComponents();

                    while (File.ReadAllLines(filePath).Length == 0)
                    {
                        File.WriteAllLines(filePath, replacedLines);
                    }

                    _cnMirrorReplace = true;

                    MessageBox.Show("域名替换完成并写入文件成功！");
                }
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
                string[] lines = File.ReadAllLines(LinksFilePath);

                Dictionary<string, int> domainCount = new Dictionary<string, int>();

                foreach (string line in lines)
                {
                    Uri uri;
                    if (Uri.TryCreate(line, UriKind.Absolute, out uri))
                    {
                        string domain = uri.Host;

                        // 移除www前缀（如果有的话）
                        if (domain.StartsWith("www."))
                        {
                            domain = domain.Substring(4);
                        }

                        if (domainCount.ContainsKey(domain))
                        {
                            domainCount[domain]++;
                        }
                        else
                        {
                            domainCount[domain] = 1;
                        }
                    }
                }

                // 查找raw.githubusercontent.com域名的计数
                if (domainCount.ContainsKey("raw.githubusercontent.com"))
                {
                    int rawGithubCount = domainCount["raw.githubusercontent.com"];

                    if (domainCount.Values.Max() > rawGithubCount)
                    {
                        _cnMirrorReplace = true;
                    }
                    else
                    {
                        _cnMirrorReplace = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
            }
        }

        // 特殊目的用方法
        public static void ShowStringList(List<string> stringList)
        {
            if (stringList == null || stringList.Count == 0)
            {
                MessageBox.Show("列表为空");
                return;
            }

            string message = string.Join("\n", stringList);
            MessageBox.Show(message, "String列表内容", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 特殊目的用方法
        private List<string> ExtractNames1(List<string[]> rows, int columnIndex)
        {
            List<string> names = new List<string>();

            foreach (string[] row in rows)
            {
                if (row.Length > columnIndex)
                {
                    string name = row[columnIndex].Trim();
                    name = Regex.Replace(name, @"[^\u4e00-\u9fa5]", "");
                    if (!string.IsNullOrWhiteSpace(name) && name.Length > 1)
                    {
                        names.Add(name);
                    }
                }
            }

            if (names.Count == 0)
            {
                return null;
            }

            return names;
        }

        // 特殊目的用方法
        private async Task ProcessCsvFilesAndCopyToClipboard(string[] filePaths)
        {
            List<string> matchingFileNames = new List<string>();

            foreach (string filePath in filePaths)
            {
                handleGroup.Text = $"正在处理文件: {Path.GetFileName(filePath)}";
                List<string[]> rows = await ReadCsvFile(filePath);

                if (rows != null && rows.Count >= 2)
                {
                    bool skipFile = false;

                    foreach (var keyword in _keywords)
                    {
                        int columnIndex = FindColumnIndex(rows[1], keyword);

                        if (columnIndex != -1)
                        {
                            List<string> names = ExtractNames1(rows, columnIndex);

                            if (names == null || names.Count == 0)
                            {
                                skipFile = true;
                                break;
                            }

                            matchingFileNames.Add(Path.GetFileName(filePath));
                        }
                    }

                    if (skipFile)
                    {
                        continue;
                    }
                }
            }

            if (matchingFileNames.Count > 0)
            {
                string fileNamesToCopy = string.Join(Environment.NewLine, matchingFileNames);
                Clipboard.SetText(fileNamesToCopy);
            }

            handleGroup.Text = "处理完成";
        }

        // 特殊目的用方法
        public static void CopyTranslationsToClipboard(string fileNamesPath, string translationsPath)
        {
            try
            {
                string[] fileNames = File.ReadAllLines(fileNamesPath);

                Dictionary<string, string> translations = new Dictionary<string, string>();

                string[] translationLines = File.ReadAllLines(translationsPath);
                foreach (string line in translationLines)
                {
                    string[] parts = line.Split(new char[] { ' ' }, 2);
                    if (parts.Length == 2)
                    {
                        translations[parts[0]] = parts[1];
                    }
                }

                StringBuilder clipboardText = new StringBuilder();

                foreach (string fileName in fileNames)
                {
                    if (translations.ContainsKey(fileName))
                    {
                        clipboardText.AppendLine($"{fileName} {translations[fileName]}");
                    }
                }

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
}