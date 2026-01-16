using System;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using NSubstitute;

namespace Jellyfin.Plugin.Streamystats.Tests;

public class PluginTests
{
    [Fact]
    public void PluginExposesExpectedMetadata()
    {
        var applicationPaths = Substitute.For<IApplicationPaths>();
        var serializer = Substitute.For<IXmlSerializer>();

        var plugin = new Jellyfin.Plugin.Streamystats.Plugin(applicationPaths, serializer);

        Assert.Equal("Streamystats", plugin.Name);
        Assert.Equal(Guid.Parse("c9893499-ca27-4313-89ab-7e2a67e3e5ae"), plugin.Id);
    }
}
