using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Streamystats.Model;

/// <summary>
/// Payload provided by the File Transformation plugin.
/// </summary>
public sealed class PatchRequestPayload
{
    /// <summary>
    /// Gets or sets the contents of the file being transformed.
    /// </summary>
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }
}
