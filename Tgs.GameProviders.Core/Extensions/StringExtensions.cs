using System;

namespace Tgs.GameProviders.Core.Extensions;

public static class StringExtensions
{
    public static string AppendQueryString(this string url, string queryString)
    {
        ArgumentNullException.ThrowIfNull(url);

        if (string.IsNullOrWhiteSpace(queryString))
        {
            return url;
        }

        if (queryString.StartsWith("/"))
        {
            return $"{url}{queryString}";
        }

        if (queryString.StartsWith("&") || queryString.StartsWith("?"))
        {
            queryString = queryString[1..];
        }

        var prefix = url.IndexOf('?') >= 0 ? "&" : "?";
        return $"{url}{prefix}{queryString}";
    }

    public static string NoLongerThan(this string str, int chars)
        => string.IsNullOrEmpty(str) || str.Length <= chars ? str : str[..chars];

    public static string[] SplitByComma(this string s, StringSplitOptions options = StringSplitOptions.None)
       => s.Split(',', options);

    public static string TrimEndSlash(this string str)
        => str.TrimEnd('/');
}
