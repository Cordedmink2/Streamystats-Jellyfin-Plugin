using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Response for Streamystats IDs format recommendations.
/// </summary>
public sealed class StreamystatsRecommendationIdsResponse
{
    /// <summary>
    /// Gets recommendation data.
    /// </summary>
    [JsonPropertyName("data")]
    public StreamystatsRecommendationIdsData? Data { get; init; }
}
