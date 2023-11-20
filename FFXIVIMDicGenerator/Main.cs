using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVIMDicGenerator
{
#pragma warning disable CS8600, CS8602, CS8603, CS8604, CS8622, CS4014

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            Text += localVersion;

            InitializeCheckedBox();

            StartUpdateProgram();

            AnalyzeDomains();
        }

        // ��������ļ���
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

        // ����ת������
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

            List<string> allData = new();

            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");
            if (csvFiles.Length == 0)
            {
                MessageBox.Show("�ļ�����û���ҵ��κ� CSV �ļ�");
                return;
            }

            disableAllbtns();

            var tasks = csvFiles.Select(csvFile => ProcessCsvFile(csvFile, allData)).ToArray();
            await Task.WhenAll(tasks);

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

        // �����ļ����ɹ���
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

            onlineLinksFromFile = onlineLinksFromFile.Where(link => !string.IsNullOrEmpty(link)).ToList();

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

        // �������ʼ��
        private void Main_Load(object sender, EventArgs e)
        {
        }

        // �˵�������ع���/���� ������ʼ����
        private void ffxivdataminingcnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/thewakingsands/ffxiv-datamining-cn");
        }

        private void �����ʿ�ת��ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/studyzy/imewlconverter");
        }

        private void csv�ļ����ݲο�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/Souma-Sumire/FFXIVChnTextPatch-Souma/wiki/CSV%E6%96%87%E4%BB%B6");
        }

        // �˵�������ع���/���� ������������

        // �����ļ�����-�༭ Links.txt ����
        private void onlineFileLinkEdit_Click(object sender, EventArgs e)
        {
            OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
        }

        // ˢ�¸����������ļ����ɹ�����ص����
        private void RefreshOnlineRelatedComponents(int param = -1)
        {
            onlineFileList.ItemCheck -= onlineFileList_ItemCheck;

            string fileContent = File.ReadAllText(LinksFilePath);
            LinksName = GetFileNamesFromLinksFile(LinksFilePath);
            string[] lines = File.ReadAllLines(LinksFilePath);

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

        // �б����ѡ��״̬����
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

        // ���������ļ������ļ�
        private void btnReloadOnline_Click(object sender, EventArgs e)
        {
            GetDefaultLinksList();

            try
            {
                File.WriteAllLines(LinksFilePath, onlineItemFileLinks);
                MessageBox.Show("���óɹ�");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"����ʱ��������: {ex.Message}");
            }

            RefreshOnlineRelatedComponents();
        }

        // ���������������ǩ�����ø������״̬
        private void onlineLinkCountLabel_Click(object sender, EventArgs e)
        {
            if (!File.Exists(LinksFilePath))
            {
                onlineLinkstextbox.Text = "��ȡ���������ã�";
                return;
            }

            RefreshOnlineRelatedComponents();
        }

        // �滻Ϊ���ھ������� (gitmirror)
        private void ���ھ�������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceDomainInFile();
        }

        // ����/���� ���а�ť (��ֹ�����)
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

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN");
        }
    }
}