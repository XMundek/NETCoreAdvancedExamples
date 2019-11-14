using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRService.Hubs
{
    public class ChatUser
    {
        public string Nick { get; set; }
        public int Age { get; set; }
    }
    public class ChatHub : Hub
    {
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await this.Clients.Others.SendAsync("Disconnected", "Connection closed:" + this.Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public override async Task OnConnectedAsync()
        {           
            // this.Context.ConnectionId
            await base.OnConnectedAsync();
            await this.Clients.All.SendAsync("Connected", "Connection established:" + this.Context.ConnectionId);
        }

        public async void Attach(ChatUser user)
        {
            await this.Clients.Others.SendAsync("Attached", user);
        }
        public async void Send(ChatUser user, string msg)
        {
            await this.Clients.Others.SendAsync("Message", user, msg);
        }
    }
}
