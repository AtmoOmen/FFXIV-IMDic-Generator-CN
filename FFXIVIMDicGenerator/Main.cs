using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TinyPinyin;
using static System.Windows.Forms.LinkLabel;

namespace FFXIVIMDicGenerator
{
#pragma warning disable CS8600, CS8603, CS8604
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

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

            List<string> allData = new List<string>();

            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");
            if (csvFiles.Length == 0)
            {
                MessageBox.Show("文件夹中没有找到任何 CSV 文件");
                return;
            }

            disableAllbtns();

            foreach (string csvFile in csvFiles)
            {
                await ProcessCsvFile(csvFile, allData);
            }

            string outputFilePath = Path.Combine(Environment.CurrentDirectory, "output.txt");
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

            string outputFilePath = Path.Combine(Environment.CurrentDirectory, "output.txt");
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

        private async Task ProcessCsvFile(string filePath, List<string> allData)
        {
            handleGroup.Text = $"正在处理文件: {Path.GetFileName(filePath)}";
            List<string[]> rows = await ReadCsvFile(filePath);

            if (rows == null || rows.Count < 2)
            {
                return;
            }

            int nameColumnIndex = FindColumnIndex(rows[1], "Name");

            if (nameColumnIndex == -1)
            {
                return;
            }

            List<string> names = ExtractNames(rows, nameColumnIndex);

            int singularColumnIndex = FindColumnIndex(rows[1], "Singular");

            if (singularColumnIndex != -1)
            {
                names.AddRange(ExtractNames(rows, singularColumnIndex));
            }

            Dictionary<string, string> pinyinDictionary = new Dictionary<string, string>();

            foreach (string name in names)
            {
                string pinyin = PinyinHelper.GetPinyin(name, "'");
                pinyin = "'" + pinyin.ToLower();
                pinyinDictionary[name] = pinyin;
            }

            foreach (var kvp in pinyinDictionary)
            {
                allData.Add($"{kvp.Value} {kvp.Key}");
            }

            handleGroup.Text = $"处理完成: {Path.GetFileName(filePath)}";
        }

        private async Task<List<string[]>> ReadCsvFile(string filePath)
        {
            try
            {
                if (filePath.StartsWith("http") || filePath.StartsWith("https"))
                {
                    var httpClientHandler = new HttpClientHandler()
                    {
                        MaxConnectionsPerServer = 10,
                    };

                    using (HttpClient httpClient = new HttpClient(httpClientHandler))
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(filePath);
                        if (response.IsSuccessStatusCode)
                        {
                            long fileSize = response.Content.Headers.ContentLength ?? -1;
                            using (Stream stream = await response.Content.ReadAsStreamAsync())
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                List<string[]> rows = new List<string[]>();
                                string line;
                                long bytesRead = 0;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                    bytesRead += Encoding.UTF8.GetByteCount(line);
                                    rows.Add(line.Split(','));

                                    if (fileSize != -1)
                                    {
                                        int progressPercentage = (int)(((double)bytesRead / fileSize) * 100);
                                    }
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
                    return File.ReadAllLines(filePath, Encoding.UTF8)
                               .Select(line => line.Split(','))
                               .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取 CSV 文件时发生错误: {ex.Message}");
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

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("注意事项:\n" +
                "1.本地/在线数据源均采用 ffxiv-datamining-cn 项目提供的 .csv 文件\n" +
                "2.从在线文件生成 功能不需要选择本地文件夹目录\n" +
                "3.如果在线获取失败/需要修改生成词库包含的范围，请修改本程序同一目录下的 Links.txt 文件中的网址\n" +
                "4.程序默认生成的为 搜狗拼音txt 词库，如需其他词库请自行使用 深蓝词库转换 软件进行转换");

            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

            if (!File.Exists(filePath))
            {
                try
                {
                    File.WriteAllLines(filePath, onlineItemFileLinks);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"写入文件时发生错误: {ex.Message}");
                }
            }

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    onlineLinksFromFile.AddRange(lines);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取文件时发生错误: {ex.Message}");
            }

            onlineLinkstextbox.Text = "./Links.txt";

            string fileContent = File.ReadAllText(filePath);

            string pattern = @"(http://|https://)\S+";
            MatchCollection matches = Regex.Matches(fileContent, pattern);

            onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";

            sourceFormatCombo.SelectedIndex = 0;
            desFormatCombo.SelectedIndex = 0;

            MaximizeBox = false;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
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

        private void ffxivdataminingcnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/thewakingsands/ffxiv-datamining-cn");
        }

        private void 深蓝词库转换ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/studyzy/imewlconverter");
        }

        private void linkstxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

            try
            {
                File.WriteAllLines(filePath, onlineItemFileLinks);
                MessageBox.Show("重置成功");

                string fileContent = File.ReadAllText(filePath);

                string pattern = @"(http://|https://)\S+";
                MatchCollection matches = Regex.Matches(fileContent, pattern);

                onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";
                onlineLinkstextbox.Text = "./Links.txt";

                string[] lines = File.ReadAllLines(filePath);
                onlineLinksFromFile.Clear();
                onlineLinksFromFile.AddRange(lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重置时发生错误: {ex.Message}");
            }
        }

        private void outputtxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile(Path.Combine(Environment.CurrentDirectory, "output.txt"));
        }

        private void disableAllbtns()
        {
            btnConvert.Enabled = false;
            btnBrowseFolder.Enabled = false;
            btnBrowseOnlineFiles.Enabled = false;
        }

        private void enableAllbtns()
        {
            btnConvert.Enabled = true;
            btnBrowseFolder.Enabled = true;
            btnBrowseOnlineFiles.Enabled = true;
        }

        private void onlineFileLinkEdit_Click(object sender, EventArgs e)
        {
            OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
        }

        private void onlineLinkCountLabel_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

            try
            {
                if (File.Exists(filePath))
                {
                    string fileContent = File.ReadAllText(filePath);
                    string[] lines = File.ReadAllLines(filePath);
                    onlineLinksFromFile.Clear();
                    onlineLinksFromFile.AddRange(lines);

                    string pattern = @"(http://|https://)\S+";
                    MatchCollection matches = Regex.Matches(fileContent, pattern);

                    onlineLinkCountLabel.Text = $"当前链接数: {matches.Count}";
                    onlineLinkstextbox.Text = "./Links.txt";
                }
                else
                {
                    onlineLinkCountLabel.Text = $"当前链接数:";
                    onlineLinkstextbox.Text = "Links.txt 文件读取错误, 请重置";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取文件时发生错误: {ex.Message}");
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
    }
}