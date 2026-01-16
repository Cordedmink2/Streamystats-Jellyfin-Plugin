using System;
using System.Net.Http;
using System.Text.Json;
using Jellyfin.Plugin.Streamystats.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.System;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Resolves Jellyfin server identity for Streamystats requests.
/// </summary>
public sealed class StreamystatsServerIdResolver
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IConfigurationManager _configurationManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StreamystatsServerIdResolver> _logger;
    private string? _cachedServerId;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamystatsServerIdResolver"/> class.
    /// </summary>
    /// <param name="configurationManager">Configuration manager.</param>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="logger">Logger.</param>
    public StreamystatsServerIdResolver(
        IConfigurationManager configurationManager,
        IHttpClientFactory httpClientFactory,
        ILogger<StreamystatsServerIdResolver> logger)
    {
        _configurationManager = configurationManager;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Gets the public server id used by Streamystats.
    /// </summary>
    /// <returns>The public server id or empty string.</returns>
    public string GetPublicServerId()
    {
        if (!string.IsNullOrWhiteSpace(_cachedServerId))
        {
            return _cachedServerId;
        }

        try
        {
            var port = 8096;
            if (_configurationManager.GetConfiguration("network") is NetworkConfiguration networkConfig &&
                networkConfig.InternalHttpPort > 0)
            {
                port = networkConfig.InternalHttpPort;
            }

            using var client = _httpClientFactory.CreateClient();
            var url = $"http://127.0.0.1:{port}/System/Info/Public";
            var json = client.GetStringAsync(url).GetAwaiter().GetResult();
            var publicInfo = JsonSerializer.Deserialize<PublicSystemInfo>(json, JsonOptions);
            if (!string.IsNullOrWhiteSpace(publicInfo?.Id))
            {
                _cachedServerId = publicInfo.Id;
                return publicInfo.Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read public server id from local API.");
        }

        try
        {
            var config = _configurationManager.GetConfiguration("system/public");
            if (config is PublicSystemInfo publicInfo && !string.IsNullOrWhiteSpace(publicInfo.Id))
            {
                _cachedServerId = publicInfo.Id;
                return publicInfo.Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read public server id from configuration.");
        }

        return string.Empty;
    }
}
