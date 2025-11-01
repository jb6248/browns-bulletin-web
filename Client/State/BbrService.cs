using System.Net.Http.Json;
using BlazorApp.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorApp.Client.State;

/// <summary>
/// SignalR service for Brown's Bulletin
/// </summary>
public class BbrService
{
    private readonly HttpClient _http;
    
    public event Action<MessageUpdate>? OnMessageUpdate;

    private HubConnection? _hubConnection;

    public BbrService(HttpClient http)
    {
        _http = http;
    }
    
    public string ConnectionStatus => _hubConnection?.State.ToString() ?? "Not Connected";

    public async Task InitializeAsync()
    {
        if (_hubConnection is not null) return;
        Console.WriteLine("Initializing SignalR connection...");
        
        var negotiateResponse = await _http
            .PostAsync("api/negotiate", null);

        Console.WriteLine("Negotiated SignalR connection. Response: " + negotiateResponse);
        if (!negotiateResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to negotiate SignalR connection.");
            return;
        }

        var negotiateInfo = await negotiateResponse.Content.ReadFromJsonAsync<NegotiationInfo>();

        if (negotiateInfo is null)
        {
            Console.WriteLine("Negotiation info is null.");
            return;
        }

        Console.WriteLine("Negotiation info received. URL: " + negotiateInfo.Url);
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(negotiateInfo.Url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(negotiateInfo.AccessToken);
            })
            .Build();
        
        
        _hubConnection.On<MessageUpdate>(MessageUpdate.MethodName, (update) =>
        {
            Console.WriteLine("Received message update via SignalR: " + update.MessageText);
            OnMessageUpdate?.Invoke(update);
        });
        
        await _hubConnection.StartAsync();
        Console.WriteLine("SignalR connection started.");
    }
    
    public async Task BroadcastMessageUpdateAsync(MessageUpdate update)
    {
        // call broadcast function
        var response = await _http.PostAsJsonAsync("api/broadcast", update);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to broadcast message update.");
        }
    }
}
