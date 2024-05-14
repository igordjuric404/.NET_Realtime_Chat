using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public async Task SendMessage(string sender, string receiver, string message)
    {
        await Clients.User(receiver).SendAsync("ReceiveMessage", sender, receiver, message);
        await Clients.User(sender).SendAsync("ReceiveMessage", sender, receiver, message);
    }
}
