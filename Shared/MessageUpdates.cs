namespace BlazorApp.Shared;

public class MessageUpdate
{
    public string Id { get; set; }
    public string MessageText { get; set; }
    public string UserName { get; set; }
}

public class NewMessage
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string MessageText { get; set; }
}