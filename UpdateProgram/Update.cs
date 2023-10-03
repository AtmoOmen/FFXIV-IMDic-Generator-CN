using System.Diagnostics;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.IO.Pipes;

namespace UpdateProgram
{
    public partial class Update : Form
    {
        public Update()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            AutoUpdate();
        }

        public async Task AutoUpdate()
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

                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "");

                    client.Timeout = TimeSpan.FromSeconds(60);

                    HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                    if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.RedirectKeepVerb || response.StatusCode == HttpStatusCode.RedirectMethod)
                    {
                        string redirectUrl = response.Headers.Location?.ToString();

                        if (!string.IsNullOrEmpty(redirectUrl))
                        {
                            response = await client.GetAsync(redirectUrl);
                        }
                    }

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

                                    string targetFolder = Path.Combine(Environment.CurrentDirectory);
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
                catch (Exception ex)
                {
                    MessageBox.Show("�����쳣��" + ex.Message);
                }
            }
        }

        static void CloseOriginalProgram()
        {
            // ��ȡԭ����Ľ�����������ʵ������޸ģ�
            string originalProgramName = "FFXIVIMDicGenerator.exe";

            // ���Ҳ��ر�ԭ����Ľ���
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(originalProgramName));
            foreach (Process process in processes)
            {
                process.CloseMainWindow(); // ���Թر�������
                if (!process.WaitForExit(5000)) // �ȴ����5��
                {
                    // �����ʱ��ǿ����ֹ����
                    process.Kill();
                }
            }
        }

        static void RestartOriginalProgram()
        {
            // ����Դ����
            string originalProgramPath = "FFXIVIMDicGenerator.exe"; // Դ�����·��
            Process.Start(originalProgramPath);

            // �˳���ǰ���³���
            Environment.Exit(0);
        }

    }
}