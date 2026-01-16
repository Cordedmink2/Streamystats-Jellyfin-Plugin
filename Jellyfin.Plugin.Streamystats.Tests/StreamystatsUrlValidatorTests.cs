using Jellyfin.Plugin.Streamystats.Api;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class StreamystatsUrlValidatorTests
{
    [Fact]
    public void TryNormalize_ReturnsFalse_OnEmpty()
    {
        var result = StreamystatsUrlValidator.TryNormalize("", out var normalized);

        Assert.False(result);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_ReturnsFalse_OnInvalidScheme()
    {
        var result = StreamystatsUrlValidator.TryNormalize("ftp://example.com", out var normalized);

        Assert.False(result);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_NormalizesUrl()
    {
        var result = StreamystatsUrlValidator.TryNormalize("https://example.com/path", out var normalized);

        Assert.True(result);
        Assert.Equal("https://example.com", normalized);
    }
}
