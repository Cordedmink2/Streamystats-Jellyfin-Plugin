using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// HTTP client wrapper for Streamystats requests.
/// </summary>
public sealed class StreamystatsHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StreamystatsHttpClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamystatsHttpClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="logger">Logger.</param>
    public StreamystatsHttpClient(IHttpClientFactory httpClientFactory, ILogger<StreamystatsHttpClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Sends a GET request to Streamystats.
    /// </summary>
    /// <param name="url">Absolute URL.</param>
    /// <param name="token">Jellyfin access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response body or null when failed.</returns>
    public async Task<string?> GetAsync(string url, string? token, CancellationToken cancellationToken)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Authorization", $"MediaBrowser Token=\"{token}\"");
                request.Headers.Add("X-Emby-Token", token);
                request.Headers.Add("X-MediaBrowser-Token", token);
            }

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Streamystats request failed with status {StatusCode} for {Url}", response.StatusCode, url);
                return null;
            }

            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Streamystats request failed for {Url}", url);
            return null;
        }
    }
}
