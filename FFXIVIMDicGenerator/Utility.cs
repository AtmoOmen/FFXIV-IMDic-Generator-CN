using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVIMDicGenerator;

public static class Utility
{
    private static readonly HttpClient HttpClient = new(new HttpClientHandler { MaxConnectionsPerServer = 32 });

    public static void OpenFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("路径不能为空", nameof(filePath));

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                }
            };
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"尝试打开 {filePath} 时发生错误: {ex.Message}");
        }
    }
    
    public static async IAsyncEnumerable<string[]> ReadCsvFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("路径不能为空", nameof(filePath));

        Stream?       stream = null;
        StreamReader? reader = null;

        try
        {
            stream = await GetStreamAsync(filePath);
            reader = new StreamReader(stream, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"打开 {filePath} 文件时发生错误: {ex.Message}");
            yield break;
        }

        await using (stream)
        using (reader)
        {
            while (true)
            {
                string? line;
                try
                {
                    line = await reader.ReadLineAsync();
                    if (line == null) break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"读取 {filePath} 文件时发生错误: {ex.Message}");
                    yield break;
                }

                yield return line.Split(',');
            }
        }
    }

    private static Task<Stream> GetStreamAsync(string filePath) =>
        filePath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? HttpClient.GetStreamAsync(filePath)
            : Task.FromResult<Stream>(File.OpenRead(filePath));

    public static int FindColumnIndex(string[] headerRow, string columnName) 
        => Array.FindIndex(headerRow, header => string.Equals(header.Trim(), columnName, StringComparison.OrdinalIgnoreCase));
    
    public static IEnumerable<string> ExtractChineseNamesFromCsvRows(IEnumerable<string[]> rows, int columnIndex)
    {
        if (rows == null) throw new ArgumentNullException(nameof(rows));
        if (columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(columnIndex), "列索引必须为非负整数");

        return rows.Where(row => row.Length > columnIndex)
            .Select(row => Regex.Replace(row[columnIndex].Trim(), @"[^\u4e00-\u9fa5]", ""))
            .Where(name => !string.IsNullOrWhiteSpace(name) && name.Length > 1);
    }
}