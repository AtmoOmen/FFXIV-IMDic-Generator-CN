using System.Text;
using System.Text.RegularExpressions;

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

    // ��������ļ���
    private void btnBrowseFolder_Click(object sender, EventArgs e)
    {
        using var folderDialog = new FolderBrowserDialog();
        DisableAllbtns();
        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtFolderPath.Text = folderDialog.SelectedPath;
            var csvFileCount = Directory
                .EnumerateFiles(folderDialog.SelectedPath, "*.csv", SearchOption.AllDirectories).Count();
            localFileCountLabel.Text = $"��ǰ�ļ���: {csvFileCount}";
        }

        EnableAllbtns();
    }

    // ����ת������
    private async void btnConvert_Click(object sender, EventArgs e)
    {
        if (!IsValidFolderPath(txtFolderPath.Text))
        {
            MessageBox.Show("��ѡ����Ч���ļ���");
            return;
        }

        var format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
        if (string.IsNullOrEmpty(format) || format == "δ֪")
        {
            MessageBox.Show("��ѡ����Ч�ĸ�ʽת������");
            return;
        }

        try
        {
            DisableAllbtns();
            await ProcessFolder(txtFolderPath.Text, format);
            MessageBox.Show("������ɣ�");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"����ʱ��������: {ex.Message}");
        }
        finally
        {
            EnableAllbtns();
            handleGroup.Text = "����";
        }
    }

    private bool IsValidFolderPath(string folderPath)
    {
        return !string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath);
    }

    private async Task ProcessFolder(string folderPath, string format)
    {
        var allData = new List<string>();
        var csvFiles = Directory.GetFiles(folderPath, "*.csv");

        if (csvFiles.Length == 0) throw new InvalidOperationException("�ļ�����û���ҵ��κ� CSV �ļ�");

        var tasks = csvFiles.Select(csvFile => ProcessCsvFile(csvFile, allData)).ToArray();
        await Task.WhenAll(tasks);

        await File.WriteAllLinesAsync(_outputFilePath, allData, Encoding.UTF8);
        OpenConvertCmd(format);
        OpenFolder(Environment.CurrentDirectory);
    }


    // �����ļ����ɹ���
    private async void btnBrowseOnlineFiles_Click(object sender, EventArgs e)
    {
        if (!IsValidFormatSelected())
        {
            MessageBox.Show("��ѡ����Ч�ĸ�ʽת������");
            return;
        }

        try
        {
            DisableAllbtns();
            progressBar.Maximum = _onlineLinksFromFile.Count;
            progressBar.Value = 0;

            var allData = await ProcessOnlineFiles();

            await File.WriteAllLinesAsync(_outputFilePath, allData, Encoding.UTF8);
            OpenConvertCmd(GetDesConvertType(desFormatCombo.SelectedItem.ToString()));
            MessageBox.Show($"������ɣ��� {allData.Count} ��\n����ļ�λ��: {_outputFilePath}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"����ʱ��������: {ex.Message}");
        }
        finally
        {
            EnableAllbtns();
            handleGroup.Text = "����";
            OpenFolder(Environment.CurrentDirectory);
        }
    }

    private bool IsValidFormatSelected()
    {
        var format = GetDesConvertType(desFormatCombo.SelectedItem.ToString());
        return !string.IsNullOrEmpty(format) && format != "δ֪";
    }

    private async Task<List<string>> ProcessOnlineFiles()
    {
        var allData = new List<string>();

        var tasks = _onlineLinksFromFile.Where(link => !string.IsNullOrEmpty(link))
            .Select(link => ProcessCsvFile(link, allData))
            .ToList();

        await Task.WhenAll(tasks);

        progressBar.Value = _onlineLinksFromFile.Count;

        return allData;
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
    private void OnlineFileLinkEdit_Click(object sender, EventArgs e)
    {
        OpenFile(Path.Combine(Environment.CurrentDirectory, "Links.txt"));
    }

    // ˢ�¸����������ļ����ɹ�����ص����
    private void RefreshOnlineRelatedComponents(int param = -1)
    {
        onlineFileList.ItemCheck -= onlineFileList_ItemCheck;

        var fileContent = File.ReadAllText(LinksFilePath);
        _linksName = GetFileNamesFromLinksFile(LinksFilePath);
        var lines = File.ReadAllLines(LinksFilePath);

        const string pattern = @"(http://|https://)\S+";
        var matches = Regex.Matches(fileContent, pattern);

        // ��������ǩ����
        onlineLinkCountLabel.Text = $"��ǰ������: {matches.Count}";
        onlineLinkstextbox.Text = "./Links.txt";

        // ������������
        _onlineLinksFromFile.Clear();
        _onlineLinksFromFile.AddRange(lines);

        // �����б��
        if (param == -1)
        {
            for (var i = 0; i < onlineFileList.Items.Count; i++) onlineFileList.SetItemChecked(i, false);

            foreach (var index in from kvp in _linksName let keys = _fileTypeNames.Keys.ToList() select keys.IndexOf(kvp) into index where index != -1 select index)
            {
                onlineFileList.SetItemChecked(index, true);
            }
        }

        onlineFileList.ItemCheck += onlineFileList_ItemCheck;
    }

    // �б����ѡ��״̬����
    private void onlineFileList_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        var selectedItem = onlineFileList.Items[e.Index].ToString();
        var itemText = onlineFileList.Items[e.Index].ToString();

        if (itemText.StartsWith("����"))
        {
            e.NewValue = e.CurrentValue;
            return;
        }

        var isChecked = e.NewValue == CheckState.Checked;
        if (isChecked)
        {
            var isAdded = AddLinkToFileIfNotExists(_fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
            if (!isAdded) MessageBox.Show("���ʧ��");
        }
        else
        {
            var isRemoved = RemoveLinkFromFileIfExists(_fileTypeNames.FirstOrDefault(x => x.Value == selectedItem).Key);
            if (!isRemoved) MessageBox.Show("ɾ��ʧ��");
        }

        RefreshOnlineRelatedComponents(0);
    }

    // ���������ļ������ļ�
    private void btnReloadOnline_Click(object sender, EventArgs e)
    {
        GetDefaultLinksList();

        try
        {
            File.WriteAllLines(LinksFilePath, _onlineItemFileLinks);
            MessageBox.Show("���óɹ�");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"����ʱ��������: {ex.Message}");
        }

        RefreshOnlineRelatedComponents();
    }

    // ���������������ǩ�����ø������״̬
    private void OnlineLinkCountLabel_Click(object sender, EventArgs e)
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
    private void DisableAllbtns()
    {
        btnConvert.Enabled = false;
        btnBrowseFolder.Enabled = false;
        btnBrowseOnlineFiles.Enabled = false;
        desFormatCombo.Enabled = false;
        onlineFileLinkEdit.Enabled = false;
    }

    private void EnableAllbtns()
    {
        btnConvert.Enabled = true;
        btnBrowseFolder.Enabled = true;
        btnBrowseOnlineFiles.Enabled = true;
        desFormatCombo.Enabled = true;
        onlineFileLinkEdit.Enabled = true;
    }

    private void GitHubToolStripMenuItem_Click(object sender, EventArgs e)
    {
        OpenUrl("https://github.com/AtmoOmen/FFXIV-IMDic-Generator-CN");
    }
}