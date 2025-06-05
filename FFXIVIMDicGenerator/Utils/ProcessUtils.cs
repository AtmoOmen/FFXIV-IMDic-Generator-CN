using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFXIVIMDicGenerator.Utils
{
    /// <summary>
    /// 进程相关工具类
    /// </summary>
    public static class ProcessUtils
    {
        /// <summary>
        /// 打开指定URL
        /// </summary>
        /// <param name="url">要打开的URL</param>
        public static void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
            }
            catch (Exception ex)
            {
                // 静默处理异常，避免影响主程序运行
                Console.WriteLine($"无法打开URL {url}: {ex.Message}");
            }
        }

        /// <summary>
        /// 打开指定文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void OpenFile(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法打开文件 {filePath}: {ex.Message}");
            }
        }
    }
} 
