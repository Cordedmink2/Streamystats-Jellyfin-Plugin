using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamystats.Configuration;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Jellyfin-facing recommendations based on Streamystats.
/// </summary>
[ApiController]
[Route("Streamystats")]
[Authorize]
public class StreamystatsRecommendationsController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ILogger<StreamystatsRecommendationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamystatsRecommendationsController"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public StreamystatsRecommendationsController(ILogger<StreamystatsRecommendationsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets movie recommendations as Jellyfin items.
    /// </summary>
    /// <param name="httpClient">Streamystats HTTP client.</param>
    /// <param name="serverIdResolver">Server id resolver.</param>
    /// <param name="libraryManager">Library manager.</param>
    /// <param name="dtoService">DTO service.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query result of recommended items.</returns>
    [HttpGet("Recommendations/Movies/Items")]
    public async Task<ActionResult<QueryResult<BaseItemDto>>> GetMovieRecommendationsItems(
        [FromServices] StreamystatsHttpClient httpClient,
        [FromServices] StreamystatsServerIdResolver serverIdResolver,
        [FromServices] ILibraryManager libraryManager,
        [FromServices] IDtoService dtoService,
        CancellationToken cancellationToken)
    {
        return await GetRecommendationItems("Movie", httpClient, serverIdResolver, libraryManager, dtoService, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets series recommendations as Jellyfin items.
    /// </summary>
    /// <param name="httpClient">Streamystats HTTP client.</param>
    /// <param name="serverIdResolver">Server id resolver.</param>
    /// <param name="libraryManager">Library manager.</param>
    /// <param name="dtoService">DTO service.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query result of recommended items.</returns>
    [HttpGet("Recommendations/Series/Items")]
    public async Task<ActionResult<QueryResult<BaseItemDto>>> GetSeriesRecommendationsItems(
        [FromServices] StreamystatsHttpClient httpClient,
        [FromServices] StreamystatsServerIdResolver serverIdResolver,
        [FromServices] ILibraryManager libraryManager,
        [FromServices] IDtoService dtoService,
        CancellationToken cancellationToken)
    {
        return await GetRecommendationItems("Series", httpClient, serverIdResolver, libraryManager, dtoService, cancellationToken).ConfigureAwait(false);
    }

    private async Task<ActionResult<QueryResult<BaseItemDto>>> GetRecommendationItems(
        string type,
        StreamystatsHttpClient httpClient,
        StreamystatsServerIdResolver serverIdResolver,
        ILibraryManager libraryManager,
        IDtoService dtoService,
        CancellationToken cancellationToken)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Configuration == null)
        {
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        PluginConfiguration config = plugin.Configuration;
        if (!config.Enabled)
        {
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        if (string.Equals(type, "Movie", StringComparison.OrdinalIgnoreCase) && !config.StreamystatsMovieRecommendations)
        {
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        if (string.Equals(type, "Series", StringComparison.OrdinalIgnoreCase) && !config.StreamystatsSeriesRecommendations)
        {
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        if (!StreamystatsUrlValidator.TryNormalize(config.StreamystatsBaseUrl, out var baseUrl))
        {
            _logger.LogWarning("Streamystats URL is invalid or missing.");
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        var serverId = serverIdResolver.GetPublicServerId();
        if (string.IsNullOrWhiteSpace(serverId))
        {
            _logger.LogWarning("Jellyfin public server id is missing. Skipping Streamystats requests.");
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        var url = $"{baseUrl}/api/recommendations?jellyfinServerId={Uri.EscapeDataString(serverId)}&type={Uri.EscapeDataString(type)}&format=ids";
        var token = StreamystatsAuthToken.GetToken(HttpContext?.Request?.Headers);
        var json = await httpClient.GetAsync(url, token, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        StreamystatsRecommendationIdsResponse? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<StreamystatsRecommendationIdsResponse>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse Streamystats recommendations response.");
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        var ids = GetIdsForType(parsed, type);
        if (ids.Count == 0)
        {
            return new QueryResult<BaseItemDto> { Items = Array.Empty<BaseItemDto>(), TotalRecordCount = 0 };
        }

        var items = new List<BaseItem>();
        foreach (var id in ids)
        {
            if (Guid.TryParse(id, out var guid))
            {
                var item = libraryManager.GetItemById(guid);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        var dtoOptions = new DtoOptions();
        var dtoItems = items.Select(item => dtoService.GetBaseItemDto(item, dtoOptions, null, null)).ToArray();

        return new QueryResult<BaseItemDto>
        {
            Items = dtoItems,
            TotalRecordCount = dtoItems.Length
        };
    }

    private static List<string> GetIdsForType(StreamystatsRecommendationIdsResponse? response, string type)
    {
        if (response?.Data == null)
        {
            return new List<string>();
        }

        if (string.Equals(type, "Movie", StringComparison.OrdinalIgnoreCase))
        {
            return response.Data.Movies?.ToList() ?? new List<string>();
        }

        if (string.Equals(type, "Series", StringComparison.OrdinalIgnoreCase))
        {
            return response.Data.Series?.ToList() ?? new List<string>();
        }

        return new List<string>();
    }
}
