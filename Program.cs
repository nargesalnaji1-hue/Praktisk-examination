using System;
using System.Threading.Tasks;
using SocketIOClient;

class Program
{
    //Metod för att hämta användernamn och valider att det inte är tomt 
    static string GetUserName()
    {
        // Fråga efter namn 
        string username = "";
        while (string.IsNullOrWhiteSpace(username))
        {
            Console.Write("Skriv ditt namn: ");
            username = Console.ReadLine();
        }
        return username;
    }

    static async Task Main(string[] args)
    {
        //  Skriv in din användernamn (validera)
        string username = GetUserName();

        //  Anslut till servern (med rätt URL och path)
        var socket = new SocketIO("wss://api.leetcode.se", new SocketIOOptions
        {
            Path = "/sys25d"
        });

        // Händelse: När klienten ansluter till servern
        socket.OnConnected += async (s, e) =>
        {
            Console.WriteLine("Du är nu ansluten till chatten!");
            await socket.EmitAsync("join", new { username });
        };

        socket.OnDisconnected += (s, e) =>
        {
            Console.WriteLine("[STATUS] Urkopplad");
        };

        //  inhämtar meddelanden
        socket.On("message", response =>
        {
            string sender = response.GetValue<string>("sender");
            string text = response.GetValue<string>("text");
            string time = response.GetValue<string>("timestamp");
            Console.WriteLine($"[{time}] {sender}: {text}");
        });

        //  Händelser när en användare ansluter eller lämnar chatten  (join/leave)
        socket.On("user-joined", r =>
        {
            Console.WriteLine($"* {r.GetValue<string>("username")} anslöt till chatten");
        });
        socket.On("user-left", r =>
        {
            Console.WriteLine($"* {r.GetValue<string>("username")} lämnade chatten");
        });

        await socket.ConnectAsync();

        //  Skicka meddelanden + och koppla ner korrekt 
        while (true)
        {
            string userinput = Console.ReadLine();
            if (userinput.Equals("/quit", StringComparison.OrdinalIgnoreCase))
            {
                await socket.EmitAsync("leave", new { username });
                await socket.DisconnectAsync();
                break;
            }
            else if (!string.IsNullOrWhiteSpace(userinput))
            {
                await socket.EmitAsync("message", new
                {
                    sender = username,
                    text = userinput,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                });
            }
        }
    }
}
