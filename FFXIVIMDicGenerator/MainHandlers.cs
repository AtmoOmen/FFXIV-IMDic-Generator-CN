using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TinyPinyin;

#pragma warning disable CS8600, CS8603, CS8604
namespace FFXIVIMDicGenerator
{
    public partial class Main : Form
    {
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