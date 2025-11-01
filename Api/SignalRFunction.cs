using System.Net;
using System.Text.Json;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using SignalRConnectionInfo = Microsoft.Azure.Functions.Worker.SignalRConnectionInfo;

namespace Api;

public class SignalRFunction
{
    [Function("Negotiate")]
    public SignalRConnectionInfo Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")]
        HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = "chat",
            ConnectionStringSetting = "AzureSignalRConnectionString")]
        SignalRConnectionInfo connectionInfo)
    {
        return connectionInfo;
    }
    
    [Function("BroadcastMessage")]
    public async Task<BroadcastOutput> Broadcast(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "broadcast")]
        HttpRequestData req)
    {
        var body = await JsonSerializer.DeserializeAsync<MessageUpdate>(
            req.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var signalRMessage = new SignalRMessage
        {
            Target = MessageUpdate.MethodName,
            Arguments = [body]
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Broadcast sent.");

        // return both the SignalR message and the HTTP response
        return new BroadcastOutput
        {
            SignalRMessage = signalRMessage,
            HttpResponse = response
        };
    }
    
}

public class BroadcastOutput
{
    [SignalROutput(HubName = "chat", ConnectionStringSetting = "AzureSignalRConnectionString")]
    public SignalRMessage? SignalRMessage { get; set; }

    public HttpResponseData? HttpResponse { get; set; }
}
