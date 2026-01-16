using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.Streamystats.Api;

/// <summary>
/// Registers plugin services.
/// </summary>
public sealed class StreamystatsServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddSingleton<StreamystatsServerIdResolver>();
        serviceCollection.AddSingleton<StreamystatsHttpClient>();
    }
}
