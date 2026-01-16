using Jellyfin.Plugin.Streamystats.Configuration;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class PluginConfigurationTests
{
    [Fact]
    public void DefaultsAreSet()
    {
        var config = new PluginConfiguration();

        Assert.True(config.Enabled);
        Assert.Equal("http://localhost:3000", config.StreamystatsBaseUrl);
        Assert.True(config.StreamystatsMovieRecommendations);
        Assert.True(config.StreamystatsSeriesRecommendations);
        Assert.False(config.StreamystatsPromotedWatchlists);
    }
}
