
using System;

namespace BlazorApp.Shared
{
    
    public class NegotiationInfo
    {
        public string? Url { get; set; }
        public string? AccessToken { get; set; }
    }
    
    public enum Destination
    {
        User,
        Room
    }
    
    public class Message
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserName { get; set; }
        public Destination? DestinationType;
        public string? destination;
        public string? MessageText { get; set; }

        public Message(string userName, string messageText)
        {
            UserName = userName;
            MessageText = messageText;
        }

        public bool IsToOrFrom(string username)
        {
            return UserName == username || (destination == username && DestinationType == Destination.User);
        }
    }
}