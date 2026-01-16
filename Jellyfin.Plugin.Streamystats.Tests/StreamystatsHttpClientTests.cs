using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamystats.Api;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class StreamystatsHttpClientTests
{
    [Fact]
    public async Task GetAsyncReturnsNullOnFailure()
    {
        using var handler = new FakeHandler(HttpStatusCode.InternalServerError, "error");
        using var httpClient = new HttpClient(handler);
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient().Returns(httpClient);
        var logger = Substitute.For<ILogger<StreamystatsHttpClient>>();

        var client = new StreamystatsHttpClient(factory, logger);

        var result = await client.GetAsync("https://example.com/api", "token", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsyncReturnsBodyOnSuccess()
    {
        using var handler = new FakeHandler(HttpStatusCode.OK, "{\"ok\":true}");
        using var httpClient = new HttpClient(handler);
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient().Returns(httpClient);
        var logger = Substitute.For<ILogger<StreamystatsHttpClient>>();

        var client = new StreamystatsHttpClient(factory, logger);

        var result = await client.GetAsync("https://example.com/api", null, CancellationToken.None);

        Assert.Equal("{\"ok\":true}", result);
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _body;

        public FakeHandler(HttpStatusCode statusCode, string body)
        {
            _statusCode = statusCode;
            _body = body;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_body)
            });
        }
    }
}
