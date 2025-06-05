using System.Collections.Concurrent;
using System.Text;
using TinyPinyin;

namespace FFXIVIMDicGenerator.Services;

/// <summary>
/// 拼音转换服务实现
/// </summary>
public class PinyinConversionService : IPinyinConversionService
{
    public string ConvertToPinyin(string chineseText)
    {
        if (string.IsNullOrWhiteSpace(chineseText))
            return string.Empty;

        var pinyin = PinyinHelper.GetPinyin(chineseText, "'");
        return $"'{pinyin.ToLower()}";
    }

    public Dictionary<string, string> ConvertToPinyinBatch(IEnumerable<string> chineseTexts)
    {
        var stringBuilder = new StringBuilder();
        
        var pinyinMap = chineseTexts.AsParallel().ToDictionary(
            name => name,
            name =>
            {
                var pinyin = PinyinHelper.GetPinyin(name, "'");
                stringBuilder.Clear();
                stringBuilder.Append('\'').Append(pinyin.ToLower());
                return stringBuilder.ToString();
            }
        );

        return new Dictionary<string, string>(pinyinMap);
    }

    public List<string> FormatPinyinDictionary(Dictionary<string, string> pinyinDictionary)
    {
        return pinyinDictionary.Select(kvp => $"{kvp.Value} {kvp.Key}").ToList();
    }
} 