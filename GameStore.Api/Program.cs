using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("GameStore") ??
                       throw new InvalidOperationException("Connection string 'GameStore' not found.");
builder.Services.AddSqlite<GameStoreContext>(connectionString);

var app = builder.Build();

// Map endpoints
app.MapGenresEndpoints();
app.MapGamesEndpoints();

// Apply migrations
await app.MigrateDbAsync();

app.Run();
