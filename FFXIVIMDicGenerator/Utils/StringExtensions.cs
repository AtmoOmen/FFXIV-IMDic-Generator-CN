using System.Text.RegularExpressions;

namespace FFXIVIMDicGenerator.Utils;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string ToSafeString(this string? value)
    {
        return value ?? string.Empty;
    }

    public static bool ContainsChinese(this string text)
    {
        return Regex.IsMatch(text, @"[\u4e00-\u9fa5]");
    }

    public static string ExtractChinese(this string text)
    {
        return Regex.Replace(text, @"[^\u4e00-\u9fa5]", "");
    }

    public static bool IsValidChinese(this string text, int minLength = 1)
    {
        var chineseText = text.ExtractChinese();
        return !chineseText.IsNullOrWhiteSpace() && chineseText.Length >= minLength;
    }

    public static string ToCamelCase(this string text)
    {
        if (text.IsNullOrEmpty())
            return text;

        var words = text.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
            return text;

        var result = words[0].ToLower();
        for (int i = 1; i < words.Length; i++)
        {
            result += char.ToUpper(words[i][0]) + words[i][1..].ToLower();
        }

        return result;
    }

    public static string ToPascalCase(this string text)
    {
        if (text.IsNullOrEmpty())
            return text;

        var words = text.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
            return text;

        var result = string.Empty;
        foreach (var word in words)
        {
            result += char.ToUpper(word[0]) + word[1..].ToLower();
        }

        return result;
    }

    public static string Truncate(this string text, int maxLength, string suffix = "...")
    {
        if (text.IsNullOrEmpty() || text.Length <= maxLength)
            return text;

        return text[..(maxLength - suffix.Length)] + suffix;
    }
} 