
using System;

namespace BlazorApp.Shared
{
    public class Message
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserName { get; set; }
        public string? MessageText { get; set; }

        public Message(string userName, string messageText)
        {
            UserName = userName;
            MessageText = messageText;
        }
    }
}