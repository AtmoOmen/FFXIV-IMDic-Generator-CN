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
                MessageBox.Show("��ѡ����Ч���ļ���");
                return;
            }

            List<string> allData = new List<string>();

            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");
            if (csvFiles.Length == 0)
            {
                MessageBox.Show("�ļ�����û���ҵ��κ� CSV �ļ�");
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
                MessageBox.Show($"������ɣ��� {allData.Count - removedData} ��\n" +
                    $"����ļ�λ��: {outputFilePath}");
                enableAllbtns();
                processLabel.Text = string.Empty;
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�����ļ�ʱ��������: {ex.Message}");
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
                MessageBox.Show($"������ɣ��� {allData.Count - removedData} ��\n" +
                    $"����ļ�λ��: {outputFilePath}");
                enableAllbtns();
                processLabel.Text = string.Empty;
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�����ļ�ʱ��������: {ex.Message}");
                enableAllbtns();
                processLabel.Text = string.Empty;
            }
        }

        private async Task ProcessCsvFile(string filePath, List<string> allData)
        {
            processLabel.Text = $"���ڴ����ļ�: {Path.GetFileName(filePath)}";
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

            processLabel.Text = $"�������: {Path.GetFileName(filePath)}";
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
                            MessageBox.Show($"�����ļ�ʱ��������: {response.ReasonPhrase}");
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
                MessageBox.Show($"��ȡ CSV �ļ�ʱ��������: {ex.Message}");
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
                MessageBox.Show($"�����ظ���ʱ��������: {ex.Message}");
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
                    MessageBox.Show($"�ļ��� '{folderPath}' �����ڡ�");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"���ļ���ʱ��������: {ex.Message}");
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
                    Console.WriteLine($"�ļ� '{filePath}' �����ڡ�");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"���ļ�ʱ��������: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("ע������:\n" +
                "1.����/��������Դ������ ffxiv-datamining-cn ��Ŀ�ṩ�� .csv �ļ�\n" +
                "2.�������ļ����� ���ܲ���Ҫѡ�񱾵��ļ���Ŀ¼\n" +
                "3.������߻�ȡʧ��/��Ҫ�޸����ɴʿ�����ķ�Χ�����޸ı�����ͬһĿ¼�µ� Links.txt �ļ��е���ַ\n" +
                "4.����Ĭ�����ɵ�Ϊ �ѹ�ƴ��txt �ʿ⣬���������ʿ�������ʹ�� �����ʿ�ת�� �������ת��");


            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

            if (!Directory.Exists(filePath))
            {
                try
                {
                    File.WriteAllLines(filePath, onlineItemFileLinks);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"д���ļ�ʱ��������: {ex.Message}");
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
                MessageBox.Show($"��ȡ�ļ�ʱ��������: {ex.Message}");
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
                Console.WriteLine($"����ַʱ��������: {ex.Message}");
            }
        }

        private void fileText_Click(object sender, EventArgs e)
        {

        }

        private void ffxivdataminingcnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/thewakingsands/ffxiv-datamining-cn");
        }

        private void �����ʿ�ת��ToolStripMenuItem1_Click(object sender, EventArgs e)
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
                MessageBox.Show("���óɹ�");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"����ʱ��������: {ex.Message}");
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
