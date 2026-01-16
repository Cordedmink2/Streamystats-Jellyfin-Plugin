using Jellyfin.Plugin.Streamystats.Api;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class StreamystatsServiceRegistratorTests
{
    [Fact]
    public void RegisterServicesAddsServerIdResolver()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Substitute.For<IConfigurationManager>());
        var registrator = new StreamystatsServiceRegistrator();
        var host = Substitute.For<IServerApplicationHost>();

        registrator.RegisterServices(services, host);

        var provider = services.BuildServiceProvider();
        var resolver = provider.GetService<StreamystatsServerIdResolver>();

        Assert.NotNull(resolver);
    }
}
