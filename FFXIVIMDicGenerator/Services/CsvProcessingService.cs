using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using FFXIVIMDicGenerator.Configuration;
using FFXIVIMDicGenerator.Models;

namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// CSV文件处理服务实现
/// </summary>
public class CsvProcessingService : ICsvProcessingService
{
    private static readonly HttpClient HttpClient = new(new HttpClientHandler 
    { 
        MaxConnectionsPerServer = ApplicationConstants.MaxHttpConnections 
    });

    public async Task<ProcessingResult> ProcessCsvFileAsync(string filePath, CsvProcessingOptions options)
    {
        try
        {
            var rows = await ReadCsvFileAsync(filePath);
            if (rows.Count < 2)
            {
                return new ProcessingResult
                {
                    IsSuccess = false,
                    Message = "CSV文件数据不足，至少需要2行数据"
                };
            }

            var columnIndices = options.Keywords
                .Select(keyword => FindColumnIndex(rows[1], keyword))
                .Where(columnIndex => columnIndex != -1)
                .ToList();

            var uniqueNames = new HashSet<string>();
            foreach (var columnIndex in columnIndices)
            {
                uniqueNames.UnionWith(ExtractNames(rows, columnIndex));
            }

            return new ProcessingResult
            {
                IsSuccess = true,
                ProcessedCount = uniqueNames.Count,
                ProcessedData = uniqueNames.ToList(),
                Message = $"成功处理文件 {Path.GetFileName(filePath)}，提取 {uniqueNames.Count} 个词条"
            };
        }
        catch (Exception ex)
        {
            return new ProcessingResult
            {
                IsSuccess = false,
                Message = $"处理文件 {filePath} 时发生错误",
                Exception = ex
            };
        }
    }

    public async Task<ProcessingResult> ProcessCsvFilesAsync(IEnumerable<string> filePaths, CsvProcessingOptions options)
    {
        var allData = new ConcurrentBag<string>();
        var processedCount = 0;

        try
        {
            var tasks = filePaths.Select(async filePath =>
            {
                var result = await ProcessCsvFileAsync(filePath, options);
                if (result.IsSuccess)
                {
                    foreach (var item in result.ProcessedData)
                    {
                        allData.Add(item);
                    }
                    Interlocked.Add(ref processedCount, result.ProcessedCount);
                }
                return result;
            });

            var results = await Task.WhenAll(tasks);

            return new ProcessingResult
            {
                IsSuccess = results.All(r => r.IsSuccess),
                ProcessedCount = processedCount,
                ProcessedData = allData.Distinct().ToList(),
                Message = $"批量处理完成，共处理 {results.Length} 个文件，提取 {allData.Distinct().Count()} 个唯一词条"
            };
        }
        catch (Exception ex)
        {
            return new ProcessingResult
            {
                IsSuccess = false,
                Message = "批量处理文件时发生错误",
                Exception = ex
            };
        }
    }

    public async Task<List<string[]>> ReadCsvFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) 
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        try
        {
            await using var stream = filePath.StartsWith("http")
                ? await HttpClient.GetStreamAsync(filePath)
                : File.OpenRead(filePath);
            
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var rows = new List<string[]>();
            
            while (await reader.ReadLineAsync() is { } line)
            {
                rows.Add(line.Split(','));
            }
            
            return rows;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"读取CSV文件失败: {filePath}", ex);
        }
    }

    public IEnumerable<string> ExtractNames(IEnumerable<string[]> rows, int columnIndex)
    {
        return rows.Where(row => row.Length > columnIndex)
            .Select(row => Regex.Replace(row[columnIndex].Trim(), @"[^\u4e00-\u9fa5]", ""))
            .Where(name => !string.IsNullOrWhiteSpace(name) && name.Length > 1)
            .Distinct();
    }

    public int FindColumnIndex(string[] headerRow, string columnName)
    {
        return Array.FindIndex(headerRow, header => header.Trim() == columnName);
    }
} 
