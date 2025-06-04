using Amazon.S3;
using DndTest.Api;
using DndTest.Api.Models.Request;
using DndTest.Config;
using DndTest.Data;
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
            .AddScoped<DocumentApi>()
            .AddScoped<EmbeddingsService>()
            .AddSingleton<IAmazonS3>(s3Client)
            .AddSingleton<S3Service>()
            .AddScoped<SearchApi>()
            .AddScoped<NoteService>()
            .AddScoped<FileService>()
            .AddScoped<TikaService>()
            .AddScoped<LlmService>()
            .AddSingleton<SseTestService>()
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
        app.MapGet("/api/documents", ([FromServices] DocumentApi api) => api.GetAll());
        app.MapGet("/api/document/{id:int}", ([FromServices] DocumentApi api, [FromRoute] int id) => api.Get(id));

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
