using System;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Validates and normalizes Streamystats URLs.
/// </summary>
public static class StreamystatsUrlValidator
{
    /// <summary>
    /// Attempts to normalize a Streamystats base URL.
    /// </summary>
    /// <param name="rawUrl">Raw input URL.</param>
    /// <param name="normalizedUrl">Normalized absolute URL without trailing slash.</param>
    /// <returns>True when the URL is valid and normalized.</returns>
    public static bool TryNormalize(string? rawUrl, out string normalizedUrl)
    {
        normalizedUrl = string.Empty;

        if (string.IsNullOrWhiteSpace(rawUrl))
        {
            return false;
        }

        var trimmed = rawUrl.Trim();
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        normalizedUrl = uri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
        if (string.IsNullOrWhiteSpace(normalizedUrl))
        {
            return false;
        }

        return true;
    }
}
