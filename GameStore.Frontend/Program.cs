using GameStore.Frontend.Components;
using GameStore.Frontend.Interfaces;
using GameStore.Frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register the root component
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for general use
builder.Services.AddScoped(sp => 
    new HttpClient 
    { 
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

// Add named HTTP client specifically for API calls
builder.Services.AddHttpClient("GameStoreAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5063/"); // Your API URL
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register GameService - THIS WAS MISSING!
builder.Services.AddScoped<GameService>();

// Register SignalR service
builder.Services.AddScoped<ISignalRService, SignalRService>();

// Add logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

await builder.Build().RunAsync();