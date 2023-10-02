using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FFXIVIMDicGenerator
{
#pragma warning disable CS8600, CS8602, CS8603, CS8604, CS8622

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            InitializeCheckedBox();
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
                    localFileCountLabel.Text = $"��ǰ�ļ���: {csvFileCount}";
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

            var Format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
            if (string.IsNullOrEmpty(Format) || Format == "δ֪")
            {
                MessageBox.Show("��ѡ����Ч�ĸ�ʽת������");
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
                OpenConvertCmd(Format);
                MessageBox.Show($"������ɣ��� {allData.Count - removedData} ��\n" +
                    $"����ļ�λ��: {outputFilePath}");
                enableAllbtns();
                handleGroup.Text = "����";
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�����ļ�ʱ��������: {ex.Message}");
                enableAllbtns();
                handleGroup.Text = "����";
            }
        }

        private async void btnBrowseOnlineFiles_Click(object sender, EventArgs e)
        {
            disableAllbtns();
            List<string> allData = new List<string>();

            var Format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
            if (string.IsNullOrEmpty(Format) || Format == "δ֪")
            {
                MessageBox.Show("��ѡ����Ч�ĸ�ʽת������");
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
                MessageBox.Show($"������ɣ��� {allData.Count - removedData} ��\n" +
                    $"����ļ�λ��: {outputFilePath}");
                enableAllbtns();
                handleGroup.Text = "����";
                OpenFolder(Environment.CurrentDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�����ļ�ʱ��������: {ex.Message}");
                enableAllbtns();
                handleGroup.Text = "����";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��ȡ�ļ�ʱ��������: {ex.Message}");
            }

            onlineLinkstextbox.Text = "./Links.txt";

            string fileContent = File.ReadAllText(filePath);

            string pattern = @"(http://|https://)\S+";
            MatchCollection matches = Regex.Matches(fileContent, pattern);

            onlineLinkCountLabel.Text = $"��ǰ������: {matches.Count}";

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
            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");
            GetDefaultLinksList();

            try
            {
                File.WriteAllLines(filePath, onlineItemFileLinks);
                MessageBox.Show("���óɹ�");

                RefreshOnlineRelatedComponents();
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

        private void onlineFileLinkEdit_Click(object sender, EventArgs e)
        {
            OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
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

        private void RefreshOnlineRelatedComponents(int param = -1)
        {
            onlineFileList.ItemCheck -= onlineFileList_ItemCheck;
            string filePath = Path.Combine(Environment.CurrentDirectory, "Links.txt");

            string fileContent = File.ReadAllText(filePath);
            LinksName = GetFileNamesFromLinksFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
            string[] lines = File.ReadAllLines(filePath);

            string pattern = @"(http://|https://)\S+";
            MatchCollection matches = Regex.Matches(fileContent, pattern);

            // ��������ǩ����
            onlineLinkCountLabel.Text = $"��ǰ������: {matches.Count}";
            onlineLinkstextbox.Text = "./Links.txt";

            // ������������
            onlineLinksFromFile.Clear();
            onlineLinksFromFile.AddRange(lines);

            // �����б��
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

        private void csv�ļ����ݲο�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/Souma-Sumire/FFXIVChnTextPatch-Souma/wiki/CSV%E6%96%87%E4%BB%B6");
        }

        private void onlineFileList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string selectedItem = onlineFileList.Items[e.Index].ToString();
            string itemText = onlineFileList.Items[e.Index].ToString();

            if (itemText.StartsWith("����"))
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
                    MessageBox.Show("���ʧ��");
                }
            }
            else
            {
                bool isRemoved = RemoveLinkFromFileIfExists(fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
                if (!isRemoved)
                {
                    MessageBox.Show("ɾ��ʧ��");
                }
            }

            RefreshOnlineRelatedComponents(0);
        }
    }
}