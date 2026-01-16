using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamystats.Api;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.System;
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
        httpClientFactory.CreateClient().Returns(_ => throw new InvalidOperationException("HTTP client should not be used."));
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

    [Fact]
    public void GetPublicServerIdFallsBackToPublicInfoEndpoint()
    {
        var configurationManager = Substitute.For<IConfigurationManager>();
        configurationManager.GetConfiguration("system/public").Returns(_ => throw new ResourceNotFoundException("system/public"));
        configurationManager.GetConfiguration("network").Returns(new NetworkConfiguration
        {
            InternalHttpPort = 8096
        });

        using var handler = new FakeHttpMessageHandler("{\"Id\":\"server-xyz\"}");
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient(handler));
        var logger = Substitute.For<ILogger<StreamystatsServerIdResolver>>();
        var resolver = new StreamystatsServerIdResolver(configurationManager, httpClientFactory, logger);

        var result = resolver.GetPublicServerId();

        Assert.Equal("server-xyz", result);
    }

    [Fact]
    public void GetPublicServerIdReturnsEmptyWhenFallbackFails()
    {
        var configurationManager = Substitute.For<IConfigurationManager>();
        configurationManager.GetConfiguration("system/public").Returns(_ => throw new ResourceNotFoundException("system/public"));
        configurationManager.GetConfiguration("network").Returns(new NetworkConfiguration
        {
            InternalHttpPort = 8096
        });

        using var handler = new FakeHttpMessageHandler("{}");
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient(handler));
        var logger = Substitute.For<ILogger<StreamystatsServerIdResolver>>();
        var resolver = new StreamystatsServerIdResolver(configurationManager, httpClientFactory, logger);

        var result = resolver.GetPublicServerId();

        Assert.Equal(string.Empty, result);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;

        public FakeHttpMessageHandler(string response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(_response)
            };

            return Task.FromResult(message);
        }
    }
}
