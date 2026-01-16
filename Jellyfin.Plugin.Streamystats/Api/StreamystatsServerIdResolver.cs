using Jellyfin.Plugin.Streamystats.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.System;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Resolves Jellyfin server identity for Streamystats requests.
/// </summary>
public sealed class StreamystatsServerIdResolver
{
    private readonly IConfigurationManager _configurationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamystatsServerIdResolver"/> class.
    /// </summary>
    /// <param name="configurationManager">Configuration manager.</param>
    public StreamystatsServerIdResolver(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    /// <summary>
    /// Gets the public server id used by Streamystats.
    /// </summary>
    /// <returns>The public server id or empty string.</returns>
    public string GetPublicServerId()
    {
        var config = _configurationManager.GetConfiguration("system/public");
        if (config is PublicSystemInfo publicInfo && !string.IsNullOrWhiteSpace(publicInfo.Id))
        {
            return publicInfo.Id;
        }

        return string.Empty;
    }
}
