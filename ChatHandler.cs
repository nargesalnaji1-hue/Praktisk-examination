using System;
using System.Collections.Generic;

class ChatHandler
{
    private string username;
    private List<Message> history = new List<Message>();

    public ChatHandler(string username)
    {
        this.username = username;
    }

    public void SendMessage(string text)
    {
        var msg = new Message
        {
            Sender = username,
            Text = text,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };

        history.Add(msg);
        Console.WriteLine($"[{msg.Timestamp}] {msg.Sender}: {msg.Text}");
    }

    public void Leave()
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm}] {username} har lämnat chatten.");
    }
}
