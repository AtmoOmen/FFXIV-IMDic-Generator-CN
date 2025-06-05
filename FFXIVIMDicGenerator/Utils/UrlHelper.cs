using System.IO;
using System.Text.RegularExpressions;

namespace FFXIVIMDicGenerator.Utils;

/// <summary>
/// URL处理辅助类
/// </summary>
public static class UrlHelper
{
    private static readonly Regex UrlPattern = new(@"(http://|https://)\S+", RegexOptions.Compiled);

    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    public static List<string> ExtractUrlsFromText(string text)
    {
        var matches = UrlPattern.Matches(text);
        return matches.Select(match => match.Value).ToList();
    }

    public static string GetFileNameFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            return Path.GetFileName(uri.LocalPath);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string GetDomainFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Host;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string ReplaceUrlDomain(string url, string oldDomain, string newDomain)
    {
        try
        {
            var uri = new Uri(url);
            if (uri.Host.Equals(oldDomain, StringComparison.OrdinalIgnoreCase))
            {
                var builder = new UriBuilder(uri)
                {
                    Host = newDomain
                };
                return builder.ToString();
            }
            return url;
        }
        catch
        {
            return url;
        }
    }

    public static bool IsGitHubUrl(string url)
    {
        return IsValidUrl(url) && GetDomainFromUrl(url).Contains("github", StringComparison.OrdinalIgnoreCase);
    }
} 
