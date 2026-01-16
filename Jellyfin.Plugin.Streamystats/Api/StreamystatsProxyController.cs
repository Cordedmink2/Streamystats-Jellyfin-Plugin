using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamystats.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Proxies Streamystats API calls using Jellyfin auth.
/// </summary>
[ApiController]
[Route("Streamystats")]
public class StreamystatsProxyController : ControllerBase
{
    private readonly ILogger<StreamystatsProxyController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamystatsProxyController"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public StreamystatsProxyController(ILogger<StreamystatsProxyController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets promoted watchlists from Streamystats.
    /// </summary>
    /// <param name="httpClient">Streamystats HTTP client.</param>
    /// <param name="serverIdResolver">Server id resolver.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Promoted watchlists response.</returns>
    [HttpGet("Watchlists/Promoted")]
    public async Task<ActionResult<string>> GetPromotedWatchlists(
        [FromServices] StreamystatsHttpClient httpClient,
        [FromServices] StreamystatsServerIdResolver serverIdResolver,
        CancellationToken cancellationToken)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Configuration == null)
        {
            return Ok("{\"data\":[]}");
        }

        PluginConfiguration config = plugin.Configuration;
        if (!config.Enabled || !config.StreamystatsPromotedWatchlists)
        {
            return Ok("{\"data\":[]}");
        }

        if (!StreamystatsUrlValidator.TryNormalize(config.StreamystatsBaseUrl, out var baseUrl))
        {
            _logger.LogWarning("Streamystats URL is invalid or missing.");
            return Ok("{\"data\":[]}");
        }

        var serverId = serverIdResolver.GetPublicServerId();
        if (string.IsNullOrWhiteSpace(serverId))
        {
            _logger.LogWarning("Jellyfin public server id is missing. Skipping Streamystats requests.");
            return Ok("{\"data\":[]}");
        }

        var url = $"{baseUrl}/api/watchlists/promoted?jellyfinServerId={Uri.EscapeDataString(serverId)}&format=full";
        var token = HttpContext?.Request?.Headers["X-Emby-Token"].ToString();
        var json = await httpClient.GetAsync(url, token, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
        {
            return Ok("{\"data\":[]}");
        }

        return Content(json, "application/json", Encoding.UTF8);
    }
}
