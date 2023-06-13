using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace 
    business
{
    public class StreamingHub : Hub
    {
        public async Task SendMessage(string user, string messagem)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, messagem);
        }
    }
}
