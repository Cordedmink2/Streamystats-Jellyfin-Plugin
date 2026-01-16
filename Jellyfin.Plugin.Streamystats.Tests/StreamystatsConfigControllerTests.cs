using Jellyfin.Plugin.Streamystats.Api;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class StreamystatsConfigControllerTests
{
    [Fact]
    public void GetConfigReturnsPluginConfig()
    {
        var applicationPaths = Substitute.For<IApplicationPaths>();
        var serializer = Substitute.For<IXmlSerializer>();
        _ = new Jellyfin.Plugin.Streamystats.Plugin(applicationPaths, serializer);

        var logger = Substitute.For<ILogger<StreamystatsConfigController>>();
        var controller = new StreamystatsConfigController(logger);

        var result = controller.GetConfig().Value;

        Assert.NotNull(result);
        Assert.True(result!.Enabled);
        Assert.Equal("http://localhost:3000", result.StreamystatsBaseUrl);
        Assert.True(result.StreamystatsMovieRecommendations);
        Assert.True(result.StreamystatsSeriesRecommendations);
        Assert.False(result.StreamystatsPromotedWatchlists);
    }
}
