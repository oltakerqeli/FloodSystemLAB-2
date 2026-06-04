using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FloodSystem.API.Data;
using FloodSystem.API.Services.Auth;
using FloodSystem.API.Repositories.Auth.Interfaces;
using FloodSystem.API.Repositories.Auth.Implementations;
using FloodSystem.API.Services.Reporting;
using FloodSystem.API.Repositories.Reporting;
using FloodSystem.API.Services.Dashboard;
using FloodSystem.API.Repositories.Dashboard;
using FloodSystem.API.Repositories.Weather.Interfaces;
using FloodSystem.API.Repositories.Weather.Implementations;
using FloodSystem.API.Services.Weather;
using FloodSystem.API.MongoDB;
using FloodSystem.API.Services.Search; 

var builder = WebApplication.CreateBuilder(args);
Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// REPOSITORET
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<ITrafficUpdateRepository, TrafficUpdateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// SERVICES
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<ZoneService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<TrafficService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHostedService<WeatherBackgroundService>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Flood System API";
    config.Version = "v1";

    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Enter: Bearer {token}"
    });

    config.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("Bearer")
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Flood System API";
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();