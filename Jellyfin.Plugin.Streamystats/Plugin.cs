using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.Streamystats.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.Streamystats;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="configurationManager">Configuration manager.</param>
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        IConfigurationManager configurationManager)
        : base(applicationPaths, xmlSerializer)
    {
        ConfigurationManager = configurationManager;
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "Streamystats";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("c9893499-ca27-4313-89ab-7e2a67e3e5ae");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <summary>
    /// Gets the configuration manager.
    /// </summary>
    public IConfigurationManager ConfigurationManager { get; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            },
            new PluginPageInfo
            {
                Name = "streamystats.js",
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.streamystats.js", GetType().Namespace)
            },
            new PluginPageInfo
            {
                Name = "streamystats.css",
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.streamystats.css", GetType().Namespace)
            }
        ];
    }
}
