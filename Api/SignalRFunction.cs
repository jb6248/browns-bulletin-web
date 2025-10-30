using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
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
}