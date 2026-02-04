using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TourPlanner.Api;
using TourPlanner.Api.Middleware;
using TourPlanner.Api.Validators;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var runtimeConnectionString = Program.BuildRuntimePostgresConnectionString(builder.Configuration);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("logs/tourplanner-.log",
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 14));

    builder.Services.AddDbContext<TourPlannerDbContext>(opts =>
        opts.UseNpgsql(runtimeConnectionString ?? builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
    {
        opts.Password.RequireDigit = true;
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = true;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequiredLength = 8;
        opts.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<TourPlannerDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("JWT Secret is not configured.");

    builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddCors(opts =>
    {
        opts.AddPolicy("FrontendPolicy", policy =>
            policy.WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:4173",
                    builder.Configuration["Frontend:Url"] ?? "http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateTourRequestValidator>();

    builder.Services.AddScoped<ITourRepository, TourRepository>();
    builder.Services.AddScoped<ITourLogRepository, TourLogRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

    builder.Services.AddScoped<ITourService, TourService>();
    builder.Services.AddScoped<ITourLogService, TourLogService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IImageService, ImageService>();
    builder.Services.Configure<OrsOptions>(opts =>
    {
        opts.ApiKey = builder.Configuration["ORS_API_KEY"] ?? string.Empty;
    });
    builder.Services.AddHttpClient<IOrsService, OrsService>();

    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.Configure<ImagesSettings>(builder.Configuration.GetSection("Images"));
    builder.Services.Configure<ImageServiceOptions>(opts =>
        opts.BasePath = builder.Configuration["Images:BasePath"] ?? "images");

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TourPlannerDbContext>();
        db.Database.Migrate();
        await DataSeeder.SeedAsync(scope.ServiceProvider);
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    var swaggerEnabled = app.Environment.IsDevelopment() ||
                         builder.Configuration.GetValue<bool>("Swagger:Enabled");

    if (swaggerEnabled)
    {
        app.UseSwagger(c => c.RouteTemplate = "api/v1/swagger/{documentName}/swagger.json");
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "TourPlanner API v1");
            c.RoutePrefix = "api/v1/swagger";
        });
    }
    app.UseCors("FrontendPolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    var imagesPath = builder.Configuration["Images:BasePath"] ?? "images";
    Directory.CreateDirectory(imagesPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.GetFullPath(imagesPath)),
        RequestPath = "/images"
    });

    app.MapControllers();

    Log.Information("TourPlanner API starting");
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Needed for integration tests
public partial class Program
{
    internal static string? BuildRuntimePostgresConnectionString(IConfiguration configuration)
    {
        var database = configuration["POSTGRES_DB"];
        var username = configuration["POSTGRES_USER"];
        var password = configuration["POSTGRES_PASSWORD"];

        if (string.IsNullOrWhiteSpace(database) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var host = configuration["POSTGRES_HOST"] ?? "db";
        var port = configuration["POSTGRES_PORT"] ?? "5432";
        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }
}

public class ImagesSettings
{
    public string BasePath { get; set; } = "images";
}
