namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// 文件操作服务接口
/// </summary>
public interface IFileOperationService
{
    void OpenFolder(string folderPath);
    void OpenFile(string filePath);
    void OpenUrl(string url);
    Task WriteAllLinesAsync(string filePath, IEnumerable<string> lines);
    Task<string[]> ReadAllLinesAsync(string filePath);
    bool DirectoryExists(string path);
    bool FileExists(string path);
    int CountCsvFiles(string folderPath);
} 