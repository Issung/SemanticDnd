using Amazon.S3;
using DndTest.Api;
using DndTest.Api.Models.Request;
using DndTest.Config;
using DndTest.Data;
using DndTest.Data.Repositories;
using DndTest.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DndTest;

public class Program
{
    public static Task Main(string[] args)
    {
        return Main2();
    }

    // TODO: Refactor so we can use args properly.
    public static async Task Main2(Func<WebApplication, Task>? action = null)
    {
        var builder = WebApplication.CreateBuilder([]);

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 100_000_000; // Set the desired size (bytes)
        });

        // Add services to the container.
        var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var settings = new DndSettings()
        {
            Frontend =
            {
                //RootPath = "./App/dist"
                SpaProxyAddress = "http://localhost:3000/",
            }
        };

        builder.Services.AddSingleton(settings);

        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        //var dbConnection = new NpgsqlConnection(connectionString) {  };

        //builder.Services.AddHangfire(config =>
        //{
        //    config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(cs));
        //});
        //builder.Services.AddHangfireServer(config =>
        //{
        //    
        //});

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddDbContext<DndDbContext>();

        builder.Services
            .AddHttpClient()
            .AddRefitClient<IOllamaClient>()
            .ConfigureHttpClient(c => { c.BaseAddress = new Uri("http://localhost:11434"); })
        ;

        var s3Client = new AmazonS3Client(new AmazonS3Config
        {
            ServiceURL = "http://localhost:9090",
            ForcePathStyle = true,
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000") // Plain react SPA
                    .AllowAnyHeader()
                    .AllowAnyMethod();

                policy.WithOrigins("http://localhost:8081") // Expo react-native web frontend.
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services
            .AddFrontendSpa(settings)
            // Apis
            .AddScoped<BookmarksApi>()
            .AddScoped<ItemApi>()
            .AddScoped<SearchApi>()

            // Repos
            .AddScoped<ItemAppRepository>()

            // ???
            .AddSingleton<SecurityContext>()
            .AddScoped<SearchService>()
            .AddScoped<CustomFieldService>()
            .AddScoped<EmbeddingsService>()
            .AddSingleton<IAmazonS3>(s3Client)
            .AddSingleton<S3Service>()
            .AddScoped<NoteService>()
            .AddScoped<FileService>()
            .AddScoped<TikaService>()
            .AddScoped<LlmService>()
            .AddSingleton<SseTestService>()
            .AddSingleton<TimeProvider>(TimeProvider.System)
        ;

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<DndDbContext>();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        app.UseCors();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // This breaks the postgres vector plugin :( https://github.com/pgvector/pgvector-dotnet/issues/51
        //app.UseHangfireDashboard();

        app.MapFrontendSpa(settings.Frontend);

        MapEndpoints(app);

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        if (action != null)
        {
            await action(app);
        }

        app.Run();
    }

    private static void MapEndpoints(WebApplication app)
    {
        // Bookmarks / Bookmark Collections
        app.MapPost("/api/bookmarks", ([FromServices] BookmarksApi api, [FromBody] ItemBookmarksRequest request) => api.SetBookmarksForItem(request));
        app.MapGet("/api/bookmarkCollections", ([FromServices] BookmarksApi api) => api.GetCollections());
        app.MapPut("/api/bookmarkCollection", ([FromServices] BookmarksApi api, [FromBody] BookmarkCollectionPutRequest request) => api.PutBookmarkCollection(request));
        app.MapGet("/api/bookmarkCollection/{collectionId:int}", ([FromServices] BookmarksApi api, [FromRoute] int collectionId) => api.GetBookmarkCollection(collectionId));
        app.MapGet("/api/bookmarkCollection/{collectionId:int}/items", ([FromServices] BookmarksApi api, [FromRoute] int collectionId) => api.GetBookmarkCollectionItems(collectionId));
        app.MapDelete("/api/bookmarkCollection/{collectionId:int}", ([FromServices] BookmarksApi api, [FromRoute] int collectionId) => api.DeleteBookmarkCollection(collectionId));

        // Browse & Items
        app.MapGet("/api/browse/{folderId:int?}", ([FromServices] ItemApi api, int? folderId) => api.Browse(folderId));
        app.MapGet("/api/item/{id:int}", ([FromServices] ItemApi api, [FromRoute] int id) => api.Get(id));

        app.MapPut("/api/item/file/{id:int?}", ([FromServices] ItemApi api, [FromRoute] int? id, [FromForm] FilePutRequest request, IFormFile? file) => api.PutFile(id, request, file)).DisableAntiforgery();
        app.MapPut("/api/item/folder/{id:int?}", ([FromServices] ItemApi api, [FromRoute] int? id, [FromBody] FolderPutRequest request) => api.PutFolder(id, request));
        app.MapPut("/api/item/note/{id:int?}", ([FromServices] ItemApi api, [FromRoute] int? id, [FromBody] NotePutRequest request) => api.PutNote(id, request));
        app.MapPut("/api/item/shortcut/{id:int?}", ([FromServices] ItemApi api, [FromRoute] int? id, [FromBody] ShortcutPutRequest request) => api.PutShortcut(id, request));
        
        app.MapDelete("/api/item/{id:int}", ([FromServices] ItemApi api, [FromRoute] int id) => api.Delete(id));

        // Config / Custom Fields
        app.MapGet("/api/customFields", ([FromServices] CustomFieldApi api) => api.GetAll());

        // Search
        app.MapPost("/api/tradsearch", ([FromServices] SearchApi api, [FromBody] SearchRequest request) => api.TradSearch(request));
        //app.MapPost("/api/search", ([FromServices] SearchApi api, [FromBody] SearchRequest request) => api.HybridSearch(request));

        app.MapGet("/api/ssetest", (SseTestService sseTestService) => sseTestService.Test());
        app.MapGet("/api/question", async ([FromServices] LlmService llmService, [FromQuery] string question, HttpResponse response) =>
        {
            response.Headers.CacheControl = "no-cache";
            response.Headers.Append("X-Accel-Buffering", "no"); // for Nginx or reverse proxy
            response.ContentType = "text/event-stream";

            await foreach (var part in await llmService.Question(question))
            {
                var json = JsonSerializer.Serialize(part);
                await response.WriteAsync($"data: {json}\n\n");
                await response.Body.FlushAsync();
            }
        });
    }
}
