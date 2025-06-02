using GameStore.Api.Data;
using GameStore.Api.Endpoints;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

// Map endpoints
app.MapGenresEndpoints();
app.MapGamesEndpoints();

// Apply migrations
await app.MigrateDbAsync();

app.Run();
