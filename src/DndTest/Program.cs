using Amazon.S3;
using DndTest.Config;
using DndTest.Data;
using DndTest.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace DndTest;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 100_000_000; // Set the desired size (bytes)
        });

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddSingleton<DndSettings>();

        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        builder.Services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString));
        });
        builder.Services.AddHangfireServer(config =>
        {

        });

        builder.Services.AddDbContext<DndDbContext>(options =>
        {
            options.UseNpgsql(connectionString, o => o.UseVector());
            options.EnableSensitiveDataLogging();
        });

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


        builder.Services
            .AddScoped<EmbeddingsService>()
            .AddSingleton<IAmazonS3>(s3Client)
            .AddSingleton<S3Service>()
            .AddScoped<DocumentService>()
            .AddScoped<FileService>()
            .AddScoped<TikaService>()
            .AddSingleton<SseTestService>()
        ;

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<DndDbContext>();

        builder.Services.AddRazorPages();

        var app = builder.Build();

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

        app.UseHangfireDashboard();

        app.MapGet("/api/ssetest", (SseTestService sseTestService) => sseTestService.Test());

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
