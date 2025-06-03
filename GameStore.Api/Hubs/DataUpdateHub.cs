using Microsoft.AspNetCore.SignalR;

namespace GameStore.Api.Hubs;

public class DataUpdateHub(ILogger<DataUpdateHub> logger) : Hub
{
    private readonly ILogger<DataUpdateHub> _logger = logger;

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId} from {UserAgent}", 
            Context.ConnectionId, 
            Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString());
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogWarning("Client disconnected: {ConnectionId}. Reason: {Exception}", 
            Context.ConnectionId, 
            exception?.Message ?? "Normal disconnect");
        
        await base.OnDisconnectedAsync(exception);
    }
}
