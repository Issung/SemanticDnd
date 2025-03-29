using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DndTest.Data;
using DndTest.Services;
using Refit;
using DndTest.Data.Model;

namespace DndTest;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services
            .AddDbContext<DndDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services
            .AddHttpClient()
            .AddRefitClient<IOllamaClient>()
            .ConfigureHttpClient(c => { c.BaseAddress = new Uri("http://localhost:11434"); })
        ;

        builder.Services
            .AddScoped<EmbeddingsService>()
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

        app.MapGet("/api/ssetest", (SseTestService sseTestService) => sseTestService.Test());

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
