using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace SignalRServer.Hubs
{
    public class MyHub : Hub
    {
        public async Task Send(string who, string message){
            await this.Clients.All.SendAsync("Recieve",who,message);
        }
    }
}