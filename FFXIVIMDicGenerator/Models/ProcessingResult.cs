namespace FFXIVIMDicGenerator.Models;

/// <summary>
/// 处理结果模型
/// </summary>
public class ProcessingResult
{
    public bool         IsSuccess      { get; set; }
    public string       Message        { get; set; } = string.Empty;
    public int          ProcessedCount { get; set; }
    public List<string> ProcessedData  { get; set; } = new();
    public Exception?   Exception      { get; set; }
} 
