using Jellyfin.Plugin.Streamystats.Api;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class StreamystatsRecommendationIdsDataTests
{
    [Fact]
    public void MoviesDefaultsToEmptyCollection()
    {
        var data = new StreamystatsRecommendationIdsData();

        Assert.NotNull(data.Movies);
        Assert.Empty(data.Movies);
    }

    [Fact]
    public void SeriesDefaultsToEmptyCollection()
    {
        var data = new StreamystatsRecommendationIdsData();

        Assert.NotNull(data.Series);
        Assert.Empty(data.Series);
    }
}
