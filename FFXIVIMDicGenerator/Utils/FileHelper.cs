using System.IO;
using System.Text;

namespace FFXIVIMDicGenerator.Utils;

/// <summary>
/// 文件操作辅助类
/// </summary>
public static class FileHelper
{
    public static async Task<string> ReadAllTextAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath, Encoding.UTF8);
    }

    public static async Task WriteAllTextAsync(string filePath, string content)
    {
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
    }

    public static string GetSafeFileName(string fileName)
    {
        return Path.GetInvalidFileNameChars()
            .Aggregate(fileName, (current, c) => current.Replace(c, '_'));
    }

    public static string EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        return directoryPath;
    }

    public static long GetFileSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

    public static string GetRelativePath(string basePath, string fullPath)
    {
        return Path.GetRelativePath(basePath, fullPath);
    }

    public static bool IsValidPath(string path)
    {
        try
        {
            return !string.IsNullOrWhiteSpace(path) && 
                   Path.IsPathFullyQualified(path) && 
                   !Path.GetInvalidPathChars().Any(path.Contains);
        }
        catch
        {
            return false;
        }
    }
} 