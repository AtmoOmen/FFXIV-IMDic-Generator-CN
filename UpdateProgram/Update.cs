using System.Diagnostics;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;

#pragma warning disable CS8600, CS8602, CS8604
namespace UpdateProgram
{
    public partial class Update : Form
    {
        private static readonly string MutexName = "FFXIVIMDICGENERATORUpdateMutex";

        public Update()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            bool isNewInstance;
            Mutex mutex = new Mutex(true, MutexName, out isNewInstance);

            if (!isNewInstance)
            {
                // ��������Ѿ������У����˳�
                MessageBox.Show("�����Ѿ��������С�");
                Application.Exit();
            }
            else
            {
                // �첽����AutoUpdate����
                Task.Run(() => AutoUpdate());
            }
        }

        public async Task AutoUpdate()
        {
            try
            {
                string localVersion;

                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "FFXIVIMDICGENERATORLocalVersionPipe", PipeDirection.In))
                {
                    pipeClient.Connect();

                    using (StreamReader reader = new StreamReader(pipeClient))
                    {
                        localVersion = reader.ReadLine();
                    }
                }

                string apiUrl = "https://api.github.com/repos/AtmoOmen/FFXIV-IMDic-Generator-CN/releases/latest";

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
                    client.Timeout = TimeSpan.FromSeconds(60);

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject releaseInfo = JObject.Parse(responseBody);
                        string remoteVersion = releaseInfo["tag_name"]?.ToString();
                        JArray assets = (JArray)releaseInfo["assets"];
                        string downloadUrl = assets[0]["browser_download_url"]?.ToString();
                        string releaseNotes = releaseInfo["body"]?.ToString();

                        if (!string.IsNullOrEmpty(remoteVersion) && string.Compare(remoteVersion, localVersion) > 0)
                        {
                            string message = $"�����°汾��{remoteVersion}\n\n�������ݣ�\n{releaseNotes}";
                            DialogResult result = MessageBox.Show(message, "�°汾����", MessageBoxButtons.OKCancel);

                            if (result == DialogResult.OK)
                            {
                                CloseOriginalProgram();
                                using (HttpClient downloadClient = new HttpClient())
                                {
                                    byte[] zipData = await downloadClient.GetByteArrayAsync(downloadUrl);
                                    string tempZipPath = Path.Combine(Path.GetTempPath(), "update.zip");

                                    File.WriteAllBytes(tempZipPath, zipData);

                                    string targetFolder = Path.GetDirectoryName(Application.ExecutablePath);
                                    ZipFile.ExtractToDirectory(tempZipPath, targetFolder, true);
                                    File.Delete(tempZipPath);

                                    RestartOriginalProgram();
                                }
                            }
                            else
                            {
                                Application.Exit();
                            }
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                    else
                    {
                        MessageBox.Show("�޷���ȡ������Ϣ��HTTP��Ӧ״̬�룺" + response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("�����쳣��" + ex.Message);
            }
        }

        static void CloseOriginalProgram()
        {
            try
            {
                string originalProgramName = "FFXIVIMDicGenerator.exe";

                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(originalProgramName));
                foreach (Process process in processes)
                {
                    process.CloseMainWindow();
                    if (!process.WaitForExit(5000))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("�ر�ԭ����ʱ�����쳣��" + ex.Message);
            }
        }

        static void RestartOriginalProgram()
        {
            try
            {
                string originalProgramPath = Path.Combine(Application.StartupPath, "FFXIVIMDicGenerator.exe");
                Process.Start(originalProgramPath);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("��������ԭ����ʱ�����쳣��" + ex.Message);
            }
        }
    }
}
