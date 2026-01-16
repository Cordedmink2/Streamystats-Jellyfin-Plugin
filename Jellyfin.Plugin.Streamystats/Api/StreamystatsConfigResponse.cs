namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// DTO for exposing Streamystats config to the web client.
/// </summary>
public sealed class StreamystatsConfigResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the integration is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the Streamystats base URL.
    /// </summary>
    public string? StreamystatsBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether movie recommendations are enabled.
    /// </summary>
    public bool StreamystatsMovieRecommendations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether series recommendations are enabled.
    /// </summary>
    public bool StreamystatsSeriesRecommendations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether promoted watchlists are enabled.
    /// </summary>
    public bool StreamystatsPromotedWatchlists { get; set; }
}
