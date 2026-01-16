using Jellyfin.Plugin.Streamystats.Api;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.System;
using NSubstitute;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class StreamystatsServerIdResolverTests
{
    [Fact]
    public void GetPublicServerIdReturnsIdWhenPresent()
    {
        var configurationManager = Substitute.For<IConfigurationManager>();
        configurationManager.GetConfiguration("system/public").Returns(new PublicSystemInfo
        {
            Id = "server-123"
        });

        var resolver = new StreamystatsServerIdResolver(configurationManager);

        var result = resolver.GetPublicServerId();

        Assert.Equal("server-123", result);
    }

    [Fact]
    public void GetPublicServerIdReturnsEmptyWhenMissing()
    {
        var configurationManager = Substitute.For<IConfigurationManager>();
        configurationManager.GetConfiguration("system/public").Returns(null);

        var resolver = new StreamystatsServerIdResolver(configurationManager);

        var result = resolver.GetPublicServerId();

        Assert.Equal(string.Empty, result);
    }
}
