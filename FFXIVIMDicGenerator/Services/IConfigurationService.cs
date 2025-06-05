namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigurationService
{
    List<string> GetOnlineLinks();
    void UpdateOnlineLinks(List<string> links);
    List<string> GetFileNamesFromLinks(List<string> links);
    void CreateDefaultLinksFile();
    bool AddLinkIfNotExists(string fileName);
    bool RemoveLinkIfExists(string fileName);
    void ReplaceDomainInFile(string oldDomain, string newDomain);
    
    // 新增方法
    Dictionary<string, object> GetOnlineLinksConfiguration();
    string GetMirrorLink(string originalLink);
    void UpdateMirrorSetting(string setting, bool enabled);
    void UpdateMirrorSetting(bool enabled);
    bool GetMirrorSetting();
    dynamic LoadOnlineLinksFromFile();
} 