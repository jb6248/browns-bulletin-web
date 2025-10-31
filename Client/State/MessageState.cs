using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlazorApp.Client.State;

using Shared;
using System.Net;
using Microsoft.Azure.Cosmos.Linq;

/// <summary>
/// Stores all the messages in current session
/// </summary>
public class MessageState
{
    private readonly UserState _userState;
    public List<Message>? Messages { get; set; }
    private HttpClient _http;

    // get instance of userstate and httpclient to use
    public MessageState(UserState userState, HttpClient http)
    {
        _userState = userState;
        _http = http;
    }

    public async Task LoadMessages()
    {
        if (_userState.CurrentUser is null) return;
        // hit the azure load msg function and save
        try
        {
            var r = await _http.GetAsync($"api/message/{_userState.CurrentUser}");
            Console.WriteLine("Raw response:");
            Console.WriteLine(r);
            var resp = await _http.GetFromJsonAsync<Message[]>($"api/message/{_userState.CurrentUser}") ?? [];
            Messages = resp.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    // add a new message to the state
    public async Task SendMessage(string message)
    {
        if (message == "" || _userState.CurrentUser is null) return;
        var url =
            $"api/message?UserName={Uri.EscapeDataString(_userState.CurrentUser)}&MessageText={Uri.EscapeDataString(message)}";
        try
        {
            var resp = await _http.PostAsync(url, null);
            if (resp.StatusCode != HttpStatusCode.OK) return;
            if (Messages is not null)
            {
                Messages.Add(new Message(_userState.CurrentUser, message));
            }
            else
            {
                Messages = [];
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
}