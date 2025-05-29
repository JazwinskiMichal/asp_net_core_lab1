using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Games endpoints
app.MapGamesEndpoints();

app.Run();
