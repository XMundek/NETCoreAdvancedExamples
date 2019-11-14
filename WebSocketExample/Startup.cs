using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebSocketExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebSockets();
            //var webSocketOptions = new WebSocketOptions()
            //{
            //    KeepAliveInterval = TimeSpan.FromSeconds(120),
            //    ReceiveBufferSize = 4 * 1024
            //};
            //app.UseWebSockets(webSocketOptions);
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        lock (sockets)
                        {
                            sockets.Add(webSocket);
                        }
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });
            app.UseStaticFiles();
            app.Run(async ctx => await ctx.Response.WriteAsync("run wstest.html"));
        }
        private static readonly List<WebSocket> sockets = new List<WebSocket>();
        private static readonly System.Timers.Timer timer = new System.Timers.Timer(3000);


        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try { 
                var buffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString());

                var arr = new ArraySegment<byte>(buffer, 0, buffer.Length);
                Task[] tasks;
                lock (sockets)
                {

                    tasks = sockets.Select(
                        s => s.SendAsync(arr, WebSocketMessageType.Text, true, CancellationToken.None)
                    ).ToArray();
                }
            
                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (true){
                
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    lock (sockets)
                    {
   
                        sockets.Remove(webSocket);
                    }

                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    break;
                }
                var arr = new ArraySegment<byte>(buffer, 0, result.Count);
                var msgType = result.MessageType;
                var eom = result.EndOfMessage;
                Task[] tasks;
                lock (sockets) {
                    tasks = sockets
                        .Where(s=>s!=webSocket)
                        .Select(
                        s => s.SendAsync(arr, msgType, eom, CancellationToken.None)
                    ).ToArray();
                }
                try
                {
                    if (tasks.Length>0)
                        Task.WaitAll(tasks);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.Write(ex);
                }
            }
            
        }
    }
}