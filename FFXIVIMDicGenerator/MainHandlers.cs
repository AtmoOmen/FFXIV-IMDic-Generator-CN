using System.Diagnostics;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TinyPinyin;
using System.Net;

#pragma warning disable CS8600, CS8603, CS8604, CS8622, CS8602

namespace FFXIVIMDicGenerator
{
    public partial class Main : Form
    {
        private void StartUpdateProgram()
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, "Update", "UpdateProgram.exe"));
        }



        private async Task ProcessCsvFile(string filePath, List<string> allData)
        {
            handleGroup.Text = $"处理中: {Path.GetFileName(filePath)}";

            List<string[]> rows = await ReadCsvFile(filePath);

            if (rows == null || rows.Count < 2)
            {
                handleGroup.Text = $"处理完成: {Path.GetFileName(filePath)}";
                return;
            }

            Dictionary<string, string> pinyinDictionary = new Dictionary<string, string>();
            List<Task> tasks = new List<Task>();

            foreach (var keyword in keywords)
            {
                int columnIndex = FindColumnIndex(rows[1], keyword);

                if (columnIndex != -1)
                {
                    List<string> names = ExtractNames(rows, columnIndex);

                    foreach (string name in names)
                    {
                        string pinyin = PinyinHelper.GetPinyin(name, "'");
                        pinyin = "'" + pinyin.ToLower();
                        pinyinDictionary[name] = pinyin;
                    }
                }

                tasks.Add(Task.Delay(100));
            }

            await Task.WhenAll(tasks);

            foreach (var kvp in pinyinDictionary)
            {
                allData.Add($"{kvp.Value} {kvp.Key}");
            }

            handleGroup.Text = $"处理完成: {Path.GetFileName(filePath)}";
        }

        private async Task<List<string[]>> ReadCsvFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            try
            {
                if (filePath.StartsWith("http") || filePath.StartsWith("https"))
                {
                    using (var httpClient = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 10 }))
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(filePath);
                        if (response.IsSuccessStatusCode)
                        {
                            using (Stream stream = await response.Content.ReadAsStreamAsync())
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                var rows = new List<string[]>();
                                string? line;
                                long bytesRead = 0;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                    bytesRead += Encoding.UTF8.GetByteCount(line);
                                    rows.Add(line.Split(','));

                                }
                                return rows;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"下载文件时发生错误: {response.ReasonPhrase}");
                        }
                    }
                }
                else
                {
                    var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
                    var rows = new List<string[]>(lines.Length);
                    foreach (var line in lines)
                    {
                        rows.Add(line.Split(','));
                    }
                    return rows;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取CSV文件时发生错误: {ex.Message}");
            }

            return null;
        }

        private List<string> ExtractNames(List<string[]> rows, int columnIndex)
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

            return names;
        }

        private int FindColumnIndex(string[] headerRow, string columnName)
        {
            for (int i = 0; i < headerRow.Length; i++)
            {
                if (headerRow[i].Trim() == columnName)
                {
                    return i;
                }
            }

            return -1;
        }

        private int RemoveDuplicates(string filePath)
        {
            int removedItemCount = 0;

            try
            {
                List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();
                HashSet<string> uniqueLines = new HashSet<string>(lines);

                if (lines.Count != uniqueLines.Count)
                {
                    removedItemCount = lines.Count - uniqueLines.Count;

                    lines = uniqueLines.ToList();
                    File.WriteAllLines(filePath, lines, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理重复项时发生错误: {ex.Message}");
            }

            return removedItemCount;
        }

        private void OpenFolder(string folderPath)
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
            return desTypes.TryGetValue(sourceName, out string? desType) ? desType : "未知";
        }

        private List<string> GetFileNamesFromLinksFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                try
                {
                    GetDefaultLinksList();
                    File.WriteAllLines(filePath, onlineItemFileLinks);
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
            foreach (var kvp in fileTypeNames)
            {
                onlineFileList.Items.Add(kvp.Value);
            }

            LinksName = GetFileNamesFromLinksFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));

            foreach (var kvp in LinksName)
            {
                List<string> keys = fileTypeNames.Keys.ToList();

                var index = keys.IndexOf(kvp);

                if (index != -1)
                {
                    onlineFileList.SetItemChecked(index, true);
                }
            }
            onlineFileList.ItemCheck += onlineFileList_ItemCheck;
        }

        private static bool AddLinkToFileIfNotExists(string fileName)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");
            var link = "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/" + fileName;

            try
            {
                string pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
                Regex regex = new Regex(pattern);
                List<string> lines = File.ReadAllLines(filePath).ToList();

                if (!lines.Any(line => regex.IsMatch(line)))
                {
                    lines.Add(link);

                    File.WriteAllLines(filePath, lines);

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
            var filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

            try
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();

                string pattern = $@"https?:\/\/.*?\/{Regex.Escape(fileName)}$";
                Regex regex = new Regex(pattern);

                int indexToRemove = lines.FindIndex(line => regex.IsMatch(line));

                if (indexToRemove != -1)
                {
                    lines.RemoveAt(indexToRemove);

                    File.WriteAllLines(filePath, lines);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误: " + ex.Message);
            }

            return false;
        }

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
                onlineItemFileLinks = onlineContents;
            }
        }

        static void ReplaceDomainInFile()
        {
            try
            {
                var newDomain = "raw.gitmirror.com";
                var oldDomain = "raw.githubusercontent.com";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

                var fileContent = File.ReadAllText(filePath);

                var pattern = @"https://(.*?)" + Regex.Escape(oldDomain);
                var replacedContent = Regex.Replace(fileContent, pattern, $"https://${{1}}{newDomain}");

                File.WriteAllText(filePath, replacedContent);

                MessageBox.Show("域名替换完成并写入文件成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
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

                    foreach (var keyword in keywords)
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