using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("GameStore") ??
                       throw new InvalidOperationException("Connection string 'GameStore' not found.");
builder.Services.AddSqlite<GameStoreContext>(connectionString);

var app = builder.Build();

// Games endpoints
app.MapGamesEndpoints();

// Apply migrations
app.MigrateDb();

app.Run();
