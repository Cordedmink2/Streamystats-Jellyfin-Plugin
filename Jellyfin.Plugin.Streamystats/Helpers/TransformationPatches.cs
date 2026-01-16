using System;
using Jellyfin.Plugin.Streamystats.Model;
using MediaBrowser.Common.Net;

namespace Jellyfin.Plugin.Streamystats.Helpers;

/// <summary>
/// File Transformation callbacks.
/// </summary>
public static class TransformationPatches
{
    /// <summary>
    /// Injects Streamystats assets into the Jellyfin web UI.
    /// </summary>
    /// <param name="content">Patch payload from File Transformation.</param>
    /// <returns>Updated HTML contents.</returns>
    public static string IndexHtml(PatchRequestPayload content)
    {
        if (string.IsNullOrWhiteSpace(content.Contents))
        {
            return content.Contents ?? string.Empty;
        }

        var html = content.Contents;
        if (html.Contains("streamystats.js", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("streamystats.css", StringComparison.OrdinalIgnoreCase))
        {
            return html;
        }

        var rootPath = string.Empty;
        var configManager = Plugin.Instance?.ConfigurationManager;
        if (configManager?.GetConfiguration("network") is NetworkConfiguration networkConfig &&
            !string.IsNullOrWhiteSpace(networkConfig.BaseUrl))
        {
            rootPath = "/" + networkConfig.BaseUrl.Trim('/');
        }

        var cssUrl = $"{rootPath}/web/ConfigurationPage?name=streamystats.css";
        var jsUrl = $"{rootPath}/web/ConfigurationPage?name=streamystats.js";

        var cssTag = $"<link rel=\"stylesheet\" href=\"{cssUrl}\" />";
        var jsTag = $"<script type=\"text/javascript\" plugin=\"Jellyfin.Plugin.Streamystats\" src=\"{jsUrl}\" defer></script>";

        return html
            .Replace("</head>", $"{cssTag}</head>", StringComparison.OrdinalIgnoreCase)
            .Replace("</body>", $"{jsTag}</body>", StringComparison.OrdinalIgnoreCase);
    }
}
