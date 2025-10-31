using System.Net;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace Api;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class MessageFunctions(AzureContext db)
{
    [Function("SaveMessage")]
    public async Task<HttpResponseData> SaveMessage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "message")]
        HttpRequestData req
    )
    {
        var msg = new Message(req.Query["UserName"], req.Query["MessageText"]);
        try
        {
            await db.Messages.AddAsync(msg);
            await db.SaveChangesAsync();
            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(msg);
            return resp;
        }
        catch (DbUpdateException e)
        {
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
    
    [Function("GetMessages")]
    public async Task<HttpResponseData> GetMessages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "message/{username?}")]
        HttpRequestData req,
        string? username
    )
    {
        var messages = string.IsNullOrEmpty(username)
            ? await db.Messages
                .ToListAsync()
            : await db.Messages.ToListAsync(); // || (m.destination == username && m.DestinationType == Destination.User)).ToListAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(messages);
        return response;
    }
}