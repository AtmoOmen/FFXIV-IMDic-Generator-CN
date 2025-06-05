using FFXIVIMDicGenerator.Models;

namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// CSV文件处理服务接口
/// </summary>
public interface ICsvProcessingService
{
    Task<ProcessingResult> ProcessCsvFileAsync(string filePath, CsvProcessingOptions options);
    Task<ProcessingResult> ProcessCsvFilesAsync(IEnumerable<string> filePaths, CsvProcessingOptions options);
    Task<List<string[]>> ReadCsvFileAsync(string filePath);
    IEnumerable<string> ExtractNames(IEnumerable<string[]> rows, int columnIndex);
    int FindColumnIndex(string[] headerRow, string columnName);
} 