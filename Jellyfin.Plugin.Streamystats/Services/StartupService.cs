using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Jellyfin.Plugin.Streamystats.Helpers;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.Streamystats.Services;

/// <summary>
/// Registers Streamystats web UI injections on startup.
/// </summary>
public sealed class StartupService : IScheduledTask
{
    private static readonly Guid IndexTransformId = Guid.Parse("8f3c8f16-f302-4d0a-8aef-c4e84b2b2bc6");
    private readonly ILogger<StartupService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public StartupService(ILogger<StartupService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "Streamystats Startup";

    /// <inheritdoc />
    public string Key => "Jellyfin.Plugin.Streamystats.Startup";

    /// <inheritdoc />
    public string Description => "Registers Streamystats UI injection with File Transformation.";

    /// <inheritdoc />
    public string Category => "Streamystats";

    /// <inheritdoc />
    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        var payload = new JObject
        {
            ["id"] = IndexTransformId.ToString(),
            ["fileNamePattern"] = "index.html",
            ["callbackAssembly"] = GetType().Assembly.FullName,
            ["callbackClass"] = typeof(TransformationPatches).FullName,
            ["callbackMethod"] = nameof(TransformationPatches.IndexHtml)
        };

        var fileTransformationAssembly =
            AssemblyLoadContext.All.SelectMany(x => x.Assemblies)
                .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation", StringComparison.OrdinalIgnoreCase) ?? false);

        if (fileTransformationAssembly == null)
        {
            _logger.LogWarning("File Transformation plugin not found. Streamystats UI injection will be skipped.");
            return Task.CompletedTask;
        }

        var pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");
        var registerMethod = pluginInterfaceType?.GetMethod("RegisterTransformation");
        if (registerMethod == null)
        {
            _logger.LogWarning("File Transformation interface not available. Streamystats UI injection will be skipped.");
            return Task.CompletedTask;
        }

        registerMethod.Invoke(null, new object?[] { payload });
        _logger.LogInformation("Registered Streamystats web UI injection.");

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo
        {
            Type = TaskTriggerInfoType.StartupTrigger
        };
    }
}
