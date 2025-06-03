using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using GameStore.Api.Hubs;
using GameStore.Api.Interfaces;
using GameStore.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("GameStore") ??
                       throw new InvalidOperationException("Connection string 'GameStore' not found.");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Game Store API",
        Version = "v1",
        Description = "A simple API for managing games and genres"
    });
});

builder.Services.AddSqlite<GameStoreContext>(connectionString);

// Add SignalR
builder.Services.AddSignalR(options =>
{
    // Add these options for debugging
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins("http://localhost:5080") // Frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
  });
});

// Add PLC Data Service
builder.Services.AddHostedService<PlcDataService>();
builder.Services.AddScoped<IPlcDataService, PlcDataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

  // Add CORS
app.UseCors("AllowFrontend");

// Map SignalR Hub
app.MapHub<DataUpdateHub>("/datahub");

// Map endpoints
app.MapGenresEndpoints();
app.MapGamesEndpoints();

// Apply migrations
await app.MigrateDbAsync();

app.Run();
