using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace New_folder
{
    public class GameHub : Hub
    {
        public async Task SendMessage(string message)
        { 
            Console.WriteLine($"SendMessage -> {message}");
            await Clients.All.SendAsync("MessageSent", message);
            Console.WriteLine($"MessageSent -> {message}");
        }
    }
}