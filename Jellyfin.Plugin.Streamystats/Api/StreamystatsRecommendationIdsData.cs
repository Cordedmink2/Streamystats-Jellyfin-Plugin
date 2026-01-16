using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Recommendations data in IDs format.
/// </summary>
public sealed class StreamystatsRecommendationIdsData
{
    /// <summary>
    /// Gets movie IDs.
    /// </summary>
    [JsonPropertyName("movies")]
    public Collection<string> Movies { get; init; } = new();

    /// <summary>
    /// Gets series IDs.
    /// </summary>
    [JsonPropertyName("series")]
    public Collection<string> Series { get; init; } = new();
}
