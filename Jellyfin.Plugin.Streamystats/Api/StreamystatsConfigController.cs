using Jellyfin.Plugin.Streamystats.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Exposes configuration to the web UI.
/// </summary>
[ApiController]
[Route("Streamystats")]
public class StreamystatsConfigController : ControllerBase
{
    private readonly ILogger<StreamystatsConfigController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamystatsConfigController"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public StreamystatsConfigController(ILogger<StreamystatsConfigController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the current configuration for the web client.
    /// </summary>
    /// <returns>Configuration response.</returns>
    [HttpGet("Config")]
    public ActionResult<StreamystatsConfigResponse> GetConfig()
    {
        var plugin = Plugin.Instance;
        if (plugin == null)
        {
            _logger.LogWarning("Streamystats plugin instance unavailable.");
            return new StreamystatsConfigResponse
            {
                Enabled = false,
                StreamystatsBaseUrl = null,
                StreamystatsMovieRecommendations = false,
                StreamystatsSeriesRecommendations = false,
                StreamystatsPromotedWatchlists = false
            };
        }

        PluginConfiguration config = plugin.Configuration ?? new PluginConfiguration();
        if (!StreamystatsUrlValidator.TryNormalize(config.StreamystatsBaseUrl, out var normalizedUrl))
        {
            normalizedUrl = config.StreamystatsBaseUrl;
        }

        return new StreamystatsConfigResponse
        {
            Enabled = config.Enabled,
            StreamystatsBaseUrl = normalizedUrl,
            StreamystatsMovieRecommendations = config.StreamystatsMovieRecommendations,
            StreamystatsSeriesRecommendations = config.StreamystatsSeriesRecommendations,
            StreamystatsPromotedWatchlists = config.StreamystatsPromotedWatchlists
        };
    }
}
