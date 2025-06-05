namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// 拼音转换服务接口
/// </summary>
public interface IPinyinConversionService
{
    string ConvertToPinyin(string chineseText);
    Dictionary<string, string> ConvertToPinyinBatch(IEnumerable<string> chineseTexts);
    List<string> FormatPinyinDictionary(Dictionary<string, string> pinyinDictionary);
} 