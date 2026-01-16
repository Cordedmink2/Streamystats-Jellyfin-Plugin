using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Streamystats.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Enabled = true;
        StreamystatsBaseUrl = "http://localhost:3000";
        StreamystatsMovieRecommendations = true;
        StreamystatsSeriesRecommendations = true;
        StreamystatsPromotedWatchlists = false;
        StreamystatsSectionOrder = "Movies,Series,Watchlists";
    }

    /// <summary>
    /// Gets or sets a value indicating whether the integration is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the Streamystats base URL.
    /// </summary>
    public string StreamystatsBaseUrl { get; set; }

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

    /// <summary>
    /// Gets or sets the comma-separated order for Streamystats sections.
    /// </summary>
    public string StreamystatsSectionOrder { get; set; }
}
