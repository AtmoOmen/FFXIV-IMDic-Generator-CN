namespace FFXIVIMDicGenerator.Models;

/// <summary>
/// CSV处理选项模型
/// </summary>
public class CsvProcessingOptions
{
    public string[] Keywords                { get; set; } = [];
    public bool     IncludeChineseOnly      { get; set; } = true;
    public int      MinimumWordLength       { get; set; } = 1;
    public bool     RemoveSpecialCharacters { get; set; } = true;
} 
