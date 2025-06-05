using System.IO;
using System.Text.RegularExpressions;
using FFXIVIMDicGenerator.Configuration;

namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// 配置管理服务实现
/// </summary>
public class ConfigurationService(IFileOperationService fileOperationService) : IConfigurationService
{
    private readonly string linksFilePath = Path.Combine(Environment.CurrentDirectory, ApplicationConstants.LinksFileName);

    public List<string> GetOnlineLinks()
    {
        if (!fileOperationService.FileExists(linksFilePath))
        {
            CreateDefaultLinksFile();
        }

        try
        {
            var lines = File.ReadAllLines(linksFilePath);
            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"读取链接文件时发生错误: {ex.Message}", ex);
        }
    }

    public void UpdateOnlineLinks(List<string> links)
    {
        try
        {
            File.WriteAllLines(linksFilePath, links);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"更新链接文件时发生错误: {ex.Message}", ex);
        }
    }

    public List<string> GetFileNamesFromLinks(List<string> links)
    {
        var fileNames = new List<string>();

        foreach (var link in links)
        {
            var parts = link.Split('/');
            if (parts.Length > 0)
            {
                fileNames.Add(parts[^1]);
            }
        }

        return fileNames;
    }

    public void CreateDefaultLinksFile()
    {
        try
        {
            File.WriteAllLines(linksFilePath, DefaultUrls.OnlineItemFileLinks);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"创建默认链接文件时发生错误: {ex.Message}", ex);
        }
    }

    public bool AddLinkIfNotExists(string fileName)
    {
        try
        {
            var lines = GetOnlineLinks();
            var linkToAdd = DefaultUrls.OnlineItemFileLinks.FirstOrDefault(link => link.EndsWith(fileName));
            
            if (linkToAdd != null && !lines.Contains(linkToAdd))
            {
                lines.Add(linkToAdd);
                UpdateOnlineLinks(lines);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"添加链接时发生错误: {ex.Message}", ex);
        }
    }

    public bool RemoveLinkIfExists(string fileName)
    {
        try
        {
            var lines = GetOnlineLinks();
            var linkToRemove = lines.FirstOrDefault(link => link.EndsWith(fileName));
            
            if (linkToRemove != null)
            {
                lines.Remove(linkToRemove);
                UpdateOnlineLinks(lines);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"移除链接时发生错误: {ex.Message}", ex);
        }
    }

    public void ReplaceDomainInFile(string oldDomain, string newDomain)
    {
        try
        {
            var fileContent = File.ReadAllText(linksFilePath);
            var updatedContent = fileContent.Replace(oldDomain, newDomain);
            File.WriteAllText(linksFilePath, updatedContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"替换域名时发生错误: {ex.Message}", ex);
        }
    }

    public Dictionary<string, object> GetOnlineLinksConfiguration()
    {
        var config = new Dictionary<string, object>
        {
            { "FileTypeNames", FfxivDataSources.FileTypeNames },
            { "OnlineLinks", FfxivDataSources.OnlineLinks },
            { "DefaultLinks", DefaultUrls.OnlineItemFileLinks }
        };
        return config;
    }

    public string GetMirrorLink(string originalLink) => 
        originalLink.Contains("raw.githubusercontent.com") ? originalLink.Replace("raw.githubusercontent.com", "fastgit.org") : originalLink;

    public void UpdateMirrorSetting(string setting, bool enabled) => 
        Console.WriteLine($"镜像设置 {setting} 已{(enabled ? "启用" : "禁用")}");

    public void UpdateMirrorSetting(bool enabled) => 
        Console.WriteLine($"镜像已{(enabled ? "启用" : "禁用")}");

    public bool GetMirrorSetting() => false;

    public dynamic LoadOnlineLinksFromFile()
    {
        var links = GetOnlineLinks();
        var names = GetFileNamesFromLinks(links);
        
        return new 
        {
            Links = links,
            Names = names,
            FilePath = linksFilePath,
            LinkCount = links.Count
        };
    }
} 
