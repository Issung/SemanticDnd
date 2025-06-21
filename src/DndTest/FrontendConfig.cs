using DndTest.Config;
using Microsoft.Extensions.FileProviders;
using Yarp.ReverseProxy.Configuration;

namespace DndTest;

public static class FrontendConfig
{
    /// <summary>
    /// Route where the frontend spa client is served from.
    /// </summary>
    private static readonly PathString appRoute = "/app";

    private const string SpaProxyRootCluster = "frontendSpaRootCluster";

    private static readonly RouteConfig[] SpaProxyRoutesMap =
    [
        new RouteConfig
        {
            RouteId = $"app",
            Match = new RouteMatch() { Path = $"{appRoute}/{{**catch-all}}" },
            ClusterId = SpaProxyRootCluster,
            AuthorizationPolicy = "anonymous"
        }
    ];

    /// <summary>
    /// Adds authentication + spa proxying services required for hosting SPA.<br/>
    /// Used in services setup stage.
    /// </summary>
    public static IServiceCollection AddFrontendSpa(this IServiceCollection services, DndSettings settings)
    {
        services.MaybeAddSpaReverseProxy(settings.Frontend);
        return services;
    }

    /// <summary>
    /// Either configure static files serving of SPA client or configure a reverse proxy to the SPA client dev server.<br/>
    /// Used after ap build.
    /// </summary>
    public static void MapFrontendSpa(this WebApplication app, FrontendSettings settings)
    {
        if (settings.SpaProxyAddress == null)
        {
            // Set up to serve static files from the app root path

            var spaRootFileProvider = MakeRootFileProvider(settings);

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                RequestPath = appRoute,
                FileProvider = spaRootFileProvider
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = appRoute,
                FileProvider = spaRootFileProvider
            });

            // If request is for something that doesn't match anything else, serve the index file so that history entries and bookmarking work for the SPA.
            app.MapFallbackToFile
            (
                $"{appRoute}/{{**slug:nonfile}}",
                "index.html",
                new StaticFileOptions()
                {
                    FileProvider = spaRootFileProvider
                }
            ).AllowAnonymous();
        }
        else
        {
            // Setup to reverse proxy to vite's development server (usually for local development environment only)
            app.MapReverseProxy();
        }
    }

    /// <summary>
    /// Adds reverse proxy to vite dev server if running in spa proxy mode
    /// </summary>
    private static void MaybeAddSpaReverseProxy(this IServiceCollection services, FrontendSettings settings)
    {
        if (settings.SpaProxyAddress != null)
        {
            services
                .AddReverseProxy()
                .LoadFromMemory(SpaProxyRoutesMap, BuildSpaProxyClusterConfigs(settings));
        }
    }

    private static string GetAndValidateSpaRootPath(FrontendSettings settings)
    {
        var spaBuildRootPath = Path.GetFullPath(settings.RootPath ?? throw new ArgumentException("FrontendSettings root path must be set", nameof(settings.RootPath)));

        // Check that the www root directory already exists, and the app is in it
        if (!Directory.Exists(spaBuildRootPath))
        {
            throw new ArgumentException($"Directory {spaBuildRootPath} not found. Is SPA app built?", nameof(settings.RootPath));
        }

        var indexHtml = spaBuildRootPath + "/index.html";
        if (!File.Exists(indexHtml))
        {
            throw new ArgumentException($"The index.html at does not exist at {spaBuildRootPath}. Is SPA app built?", nameof(settings.RootPath));
        }

        return spaBuildRootPath;
    }

    private static PhysicalFileProvider MakeRootFileProvider(FrontendSettings settings)
    {
        var rootPath = GetAndValidateSpaRootPath(settings);
        var rootFileProvider = new PhysicalFileProvider(rootPath);
        return rootFileProvider;
    }

    private static IReadOnlyList<ClusterConfig> BuildSpaProxyClusterConfigs(FrontendSettings settings) =>
    [
        new ClusterConfig
        {
            ClusterId = SpaProxyRootCluster,
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["spaServer"] = new DestinationConfig()
                {
                    Address = settings.SpaProxyAddress ?? throw new Exception($"{nameof(FrontendSettings.SpaProxyAddress)} unexpectedly null.")
                },
            }
        }
    ];
}