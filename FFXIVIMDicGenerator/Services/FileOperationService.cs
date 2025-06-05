using System.Diagnostics;
using System.IO;
using System.Text;

namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// 文件操作服务实现
/// </summary>
public class FileOperationService : IFileOperationService
{
    public void OpenFolder(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                throw new DirectoryNotFoundException($"文件夹 '{folderPath}' 不存在");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"打开文件夹时发生错误: {ex.Message}", ex);
        }
    }

    public void OpenFile(string filePath)
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
                throw new FileNotFoundException($"文件 '{filePath}' 不存在");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"打开文件时发生错误: {ex.Message}", ex);
        }
    }

    public void OpenUrl(string url)
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
            throw new InvalidOperationException($"打开网址时发生错误: {ex.Message}", ex);
        }
    }

    public async Task WriteAllLinesAsync(string filePath, IEnumerable<string> lines)
    {
        try
        {
            await File.WriteAllLinesAsync(filePath, lines, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"写入文件 '{filePath}' 时发生错误: {ex.Message}", ex);
        }
    }

    public async Task<string[]> ReadAllLinesAsync(string filePath)
    {
        try
        {
            return await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"读取文件 '{filePath}' 时发生错误: {ex.Message}", ex);
        }
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public int CountCsvFiles(string folderPath)
    {
        try
        {
            return Directory.EnumerateFiles(folderPath, "*.csv", SearchOption.AllDirectories).Count();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"统计CSV文件数量时发生错误: {ex.Message}", ex);
        }
    }
} 