namespace FFXIVIMDicGenerator.Configuration;

/// <summary>
/// 应用程序基础常量配置
/// </summary>
public static class ApplicationConstants
{
    public const string LocalVersion      = "1.0.7.0";
    public const string OutputFileName    = "output.txt";
    public const string LinksFileName     = "Links.txt";

    public const int MaxHttpConnections = 32;
    
    public static readonly string[] CsvKeywords = ["Name", "Singular"];
} 
