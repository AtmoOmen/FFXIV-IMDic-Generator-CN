using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;
using static System.String;

namespace UpdateProgram
{
    public partial class Update : Form
    {
        private const string MutexName = "FFXIVIMDICGENERATORUpdateMutex";
        private static readonly HttpClient HttpClient = new();

        public Update()
        {
            InitializeForm();
            RunSingleInstance();
        }

        private void InitializeForm()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void RunSingleInstance()
        {
            using var mutex = new Mutex(true, MutexName, out var isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("程序已经在运行中。");
                Application.Exit();
                return;
            }
            Task.Run(AutoUpdate);
        }

        public async Task AutoUpdate()
        {
            try
            {
                var localVersion = await ReadLocalVersionAsync();
                var response = await SendGitHubApiRequest().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"无法获取更新信息，HTTP响应状态码：{response.StatusCode}");
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                await ProcessUpdateInfoAsync(responseBody, localVersion).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生异常：{ex.Message}");
            }
        }

        private static async Task<HttpResponseMessage> SendGitHubApiRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/AtmoOmen/FFXIV-IMDic-Generator-CN/releases/latest");
            request.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("FFXIVIMDicGenerator", "1.6"));
            return await HttpClient.SendAsync(request);
        }


        private static async Task<string?> ReadLocalVersionAsync()
        {
            var pipeClient = new NamedPipeClientStream(".", "FFXIVIMDICGENERATORLocalVersionPipe", PipeDirection.In);
            await using var _ = pipeClient.ConfigureAwait(false);
            await pipeClient.ConnectAsync();
            using var reader = new StreamReader(pipeClient);
            return await reader.ReadLineAsync().ConfigureAwait(false);
        }

        private static async Task ProcessUpdateInfoAsync(string responseBody, string? localVersion)
        {
            var releaseInfo = JObject.Parse(responseBody);
            var remoteVersion = releaseInfo["tag_name"]?.ToString();
            if (Compare(remoteVersion, localVersion) <= 0)
            {
                Application.Exit();
                return;
            }

            var message = $"发现新版本：{remoteVersion}\n\n更新内容：\n{releaseInfo["body"]}";
            if (MessageBox.Show(message, "新版本可用", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                Application.Exit();
                return;
            }

            await DownloadAndUpdateAsync(releaseInfo["assets"][0]["browser_download_url"]?.ToString()).ConfigureAwait(false);
        }

        private static async Task DownloadAndUpdateAsync(string downloadUrl)
        {
            var zipData = await HttpClient.GetByteArrayAsync(downloadUrl).ConfigureAwait(false);
            var tempZipPath = Path.Combine(Path.GetTempPath(), "update.zip");
            File.WriteAllBytes(tempZipPath, zipData);

            var batFilePath = Path.Combine(Path.GetTempPath(), "update.bat");
            using (var batFile = new StreamWriter(batFilePath))
            {
                batFile.WriteLine("@echo off");
                batFile.WriteLine("timeout /t 5 /nobreak");
                batFile.WriteLine($"del \"{Path.Combine(Application.StartupPath, "FFXIVIMDicGenerator.exe")}\"");
                batFile.WriteLine($"expand \"{tempZipPath}\" -F:* \"{Path.GetDirectoryName(Application.ExecutablePath)}\"");
                batFile.WriteLine($"del \"{tempZipPath}\"");
                batFile.WriteLine($"start \"\" \"{Path.Combine(Application.StartupPath, "FFXIVIMDicGenerator.exe")}\"");
                batFile.WriteLine($"del \"%~f0\"");
            }

            CloseOriginalProgram();
            Process.Start(batFilePath);
            Application.Exit();
        }

        private static void CloseOriginalProgram()
        {
            foreach (var process in Process.GetProcessesByName("FFXIVIMDicGenerator"))
            {
                process.CloseMainWindow();
                if (!process.WaitForExit(5000)) process.Kill();
            }
        }
    }
}
