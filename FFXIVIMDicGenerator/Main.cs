using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinyPinyin;

namespace FFXIVIMDicGenerator
{
    public partial class Main : Form
    {
        private readonly List<string> onlineItemFileLinks = new List<string>
        {
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Action.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/BaseParam.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/BuddyEquip.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ChocoboRaceAbility.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ChocoboRaceItem.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ClassJob.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Companion.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/CompanyAction.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/CompanyCraftDraft.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ContentGauge.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/ContentsTutorial.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/CraftAction.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/PlaceName.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/AnimaWeapon5PatternGroup.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/AOZScore.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/BeastTribe.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Quest.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Item.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/DeepDungeonItem.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/DeepDungeonMagicStone.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/DynamicEvent.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/DynamicEventEnemyType.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Emote.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/EurekaAetherItem.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/EventItem.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/GrandCompany.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/HousingPreset.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Pet.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/PetAction.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/PetMirage.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/World.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Title.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/TripleTriadCard.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/TripleTriadCardType.csv",
            "https://raw.githubusercontent.com/thewakingsands/ffxiv-datamining-cn/master/Weather.csv",
        };
        private List<string> onlineLinksFromFile = new List<string>();

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
                MessageBox.Show($"处理完成，共 {allData.Count - removedData} 条\n" +
                    $"输出文件位于: {outputFilePath}");
                enableAllbtns();
                processLabel.Text = string.Empty;
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时发生错误: {ex.Message}");
                enableAllbtns();
                processLabel.Text = string.Empty;
            }
        }

        private async void btnBrowseOnlineFiles_Click(object sender, EventArgs e)
        {
            disableAllbtns();
            List<string> allData = new List<string>();

            foreach (string onlineLink in onlineLinksFromFile)
            {
                await ProcessCsvFile(onlineLink, allData);
            }

            string outputFilePath = Path.Combine(Environment.CurrentDirectory, "output.txt");
            try
            {
                File.WriteAllLines(outputFilePath, allData, Encoding.UTF8);
                var removedData = RemoveDuplicates(outputFilePath);
                MessageBox.Show($"处理完成，共 {allData.Count - removedData} 条\n" +
                    $"输出文件位于: {outputFilePath}");
                enableAllbtns();
                processLabel.Text = string.Empty;
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时发生错误: {ex.Message}");
                enableAllbtns();
                processLabel.Text = string.Empty;
            }
        }

        private async Task ProcessCsvFile(string filePath, List<string> allData)
        {
            processLabel.Text = $"正在处理文件: {Path.GetFileName(filePath)}";
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

            processLabel.Text = $"处理完成: {Path.GetFileName(filePath)}";
        }

        private async Task<List<string[]>> ReadCsvFile(string filePath)
        {
            try
            {
                if (filePath.StartsWith("http") || filePath.StartsWith("https"))
                {
                    using (HttpClient httpClient = new HttpClient())
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

            if (!Directory.Exists(filePath))
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

        private void fileText_Click(object sender, EventArgs e)
        {

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
            try
            {
                File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "Links.txt"), onlineItemFileLinks);
                MessageBox.Show("重置成功");
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
    }
}
