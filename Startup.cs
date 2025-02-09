using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicMateAPI.Data;
using MusicMateAPI.Hubs;
using MusicMateAPI.Services;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Env.Load(); // Load environment variables from .env file
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHttpClient<OAuthService>(); // Register OAuthService with HttpClient
        services.AddHttpClient<SpotifyService>(); // Register SpotifyService with HttpClient
        services.AddScoped<IVoiceControlService, VoiceControlService>(); // Register VoiceControlService
        services.AddScoped<IOfflineService, OfflineService>(); // Register OfflineService
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
        );
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        });
        services.AddSignalR(); // Add SignalR services
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<NotificationHub>("/notificationHub"); // Map SignalR hub for notifications
            endpoints.MapHub<PlaybackHub>("/playbackHub"); // Map SignalR hub for playback sync
        });
    }
}
