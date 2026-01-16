using Jellyfin.Plugin.Streamystats.Api;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
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

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<StreamystatsServerIdResolver>>();
        var resolver = new StreamystatsServerIdResolver(configurationManager, httpClientFactory, logger);

        var result = resolver.GetPublicServerId();

        Assert.Equal("server-123", result);
    }

    [Fact]
    public void GetPublicServerIdReturnsEmptyWhenMissing()
    {
        var configurationManager = Substitute.For<IConfigurationManager>();
        configurationManager.GetConfiguration("system/public").Returns(null);

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<StreamystatsServerIdResolver>>();
        var resolver = new StreamystatsServerIdResolver(configurationManager, httpClientFactory, logger);

        var result = resolver.GetPublicServerId();

        Assert.Equal(string.Empty, result);
    }
}
