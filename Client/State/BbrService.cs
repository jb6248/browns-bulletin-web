using System.Diagnostics;
using System.Net.Http.Json;
using BlazorApp.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.SignalR.Management;

namespace BlazorApp.Client.State;

/// <summary>
/// SignalR service for Brown's Bulletin
/// </summary>
public class BbrService
{
    private readonly HttpClient _http;
    
    private static string UPDATE_MESSAGE_METHOD = "UpdateMessage";
    public event Action<MessageUpdate>? OnMessageUpdate;

    private HubConnection? _hubConnection;
    // private ServiceManager _serviceManager;
    // private ServiceHubContext _serviceHubContext;

    public BbrService(HttpClient http)
    {
        _http = http;
    }

    public async Task InitializeAsync()
    {
        if (_hubConnection is not null) return;
        Console.WriteLine("Initializing SignalR connection...");
        // https://learn.microsoft.com/en-us/azure/azure-signalr/signalr-concept-client-negotiation
        // I have no idea why the service manager was suggested; it doesn't seem to help at all
        // _serviceManager = new ServiceManagerBuilder()
        //     .WithOptions(option =>
        //     {
        //         option.ConnectionString = "<Your Azure SignalR Service Connection String>";
        //     })
        //     // .WithLoggerFactory(loggerFactory)
        //     .BuildServiceManager();
        // _serviceHubContext = await _serviceManager.CreateHubContextAsync("chat", CancellationToken.None);
        // var negotiationResponse = await _serviceHubContext.NegotiateAsync(new (){UserId = "<Your User Id>"});

        // this is chatgpt; this makes sense to me
        // call your Azure Function's negotiate endpoint
        var negotiateResponse = await _http
            .PostAsync("api/negotiate", null);

        Console.WriteLine("Negotiated SignalR connection. Response: " + negotiateResponse);
        if (!negotiateResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to negotiate SignalR connection.");
            return;
        }

        var negotiateInfo = await negotiateResponse.Content.ReadFromJsonAsync<SignalRConnectionInfo>();

        if (negotiateInfo is null)
        {
            Console.WriteLine("Negotiation info is null.");
            return;
        }

        Console.WriteLine("Negotiation info received. URL: " + negotiateInfo.Url);
        Console.WriteLine("Token: " + negotiateInfo.AccessToken);
        // _hubConnection = new HubConnectionBuilder()
        //     .WithUrl(negotiateInfo.Url, options =>
        //     {
        //         options.AccessTokenProvider = () => Task.FromResult(negotiateInfo.AccessToken);
        //     })
        //     .WithAutomaticReconnect()
        //     .Build();


        _hubConnection = new HubConnectionBuilder()
            .WithUrl(negotiateInfo.Url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(negotiateInfo.AccessToken);
            })
            .Build();
        
        
        _hubConnection.On<MessageUpdate>(UPDATE_MESSAGE_METHOD, (update) =>
        {
            Console.WriteLine("Received message update via SignalR: " + update.MessageText);
            OnMessageUpdate?.Invoke(update);
        });
        
        await _hubConnection.StartAsync();
        Console.WriteLine("SignalR connection started.");
    }
    
    public async Task BroadcastMessageUpdateAsync(MessageUpdate update)
    {
        if (_hubConnection is null) return;
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            Console.WriteLine("Broadcasting message update via SignalR: " + update.MessageText);
            await _hubConnection.InvokeAsync(UPDATE_MESSAGE_METHOD, update);
        }
    }
}