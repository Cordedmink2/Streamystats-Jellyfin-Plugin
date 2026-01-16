using System;
using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.Streamystats.Api;

internal static class StreamystatsAuthToken
{
    internal static string? GetToken(IHeaderDictionary? headers)
    {
        if (headers == null)
        {
            return null;
        }

        var token = headers["X-Emby-Token"].ToString();
        if (!string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        token = headers["X-MediaBrowser-Token"].ToString();
        if (!string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        var authorization = headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            return null;
        }

        const string mediaBrowserPrefix = "MediaBrowser Token=\"";
        if (authorization.StartsWith(mediaBrowserPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var start = mediaBrowserPrefix.Length;
            var end = authorization.IndexOf('"', start);
            if (end > start)
            {
                return authorization.Substring(start, end - start);
            }
        }

        const string bearerPrefix = "Bearer ";
        if (authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return authorization.Substring(bearerPrefix.Length).Trim();
        }

        return null;
    }
}
