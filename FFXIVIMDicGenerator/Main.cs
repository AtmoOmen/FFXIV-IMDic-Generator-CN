using System.Text;
using System.Text.RegularExpressions;

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

        private void Form1_Load(object sender, EventArgs e)
        {
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
    }
}