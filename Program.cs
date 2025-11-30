
using Cyviz.Infrastructure.Extensions;
using Cyviz.Infrastructure.Middlewares;
using Cyviz.SignalR.Hubs;
using Serilog;
using Serilog.Formatting.Json;

namespace Cyviz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Load configuration
            ConfigureAppConfiguration(builder);

            // 2. Configure Serilog
            var logFilePath = builder.Configuration["Logging:LocalLogPath"] ?? "logs/log-.json";

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10_000_000,
                    retainedFileCountLimit: 10,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    formatter: new JsonFormatter())
                .CreateLogger();

            builder.Host.UseSerilog();

            // 3. Configure Services (DI)
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // 4. Run migrations + seed devices
            app.RunMigrations();

            // 5. Configure Middleware + Endpoints
            ConfigureApp(app);

            // 6. Start worker system
            app.StartSignalRWorkers();

            app.Run();
        }

        private static void ConfigureAppConfiguration(WebApplicationBuilder builder)
        {
            var environment = builder.Environment.EnvironmentName;

            builder.Configuration.AddJsonFile("appsettings.json", optional: false)
                                 .AddJsonFile($"appsettings.{environment}.json", optional: true);

            if (environment == "Local")
                builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

            builder.Configuration.AddEnvironmentVariables();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddInfrastructure(config);

            // Add CORS for React
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            // Add SignalR
            services.AddSignalR();

            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ORDER OF MIDDLEWARES MATTERS
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors("AllowFrontend");
            app.UseRouting();

            app.MapControllers();

            // Map SignalR hubs
            app.MapHub<ControlHub>("/controlHub");
            app.MapHub<DeviceHub>("/deviceHub");
        }
    }
}
