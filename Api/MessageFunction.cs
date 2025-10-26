using System.Net;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace Api;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public static class MessageFunctions
{
    [Function("SaveMessage")]
    public static async Task<HttpResponseData> SaveMessage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "message")]
        HttpRequestData req
    )
    {
        var db = new AzureContext();
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
    public static async Task<HttpResponseData> GetMessages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "message")]
        HttpRequestData req 
    )
    {
        var db = new AzureContext();
        var messages = await db.Messages.ToListAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(messages);
        return response;
    }
}