using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Signaling2.Network;

namespace Signaling2
{
    public class Startup
    {
        static ProxyManager Manager = new ProxyManager();
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.Map("/ws", (con) =>
            {
                con.UseWebSockets();
                con.Use(async (HttpContext context, Func<Task> n) =>
                {
                    try
                    {
                        //執行接收 
                        var socket = await context.WebSockets.AcceptWebSocketAsync();
                        var client = new ClientProxy(socket, Manager);
                        Manager.Add(client);
                        //websocket receiveAsync 如果在其他 Task 呼叫會斷線
                        await client.Net.ListenAsync();
                        Console.WriteLine("create connection");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            });
        }

        static int index = -1;
        static List<WebSocket> clients = new List<WebSocket>();
        static string OfferICE="";
        static string AnswerICE = "";
        async Task Receive(WebSocket socket,int index)
        {
            if (index == 1)
            {
                await SendStringAsync(socket,OfferICE);
            }
            while (socket.State == WebSocketState.Open)
            {
                var buffer = new byte[102400];
                var incoming = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                byte[] mybuff = new byte[incoming.Count];
                Array.Copy(buffer, 0, mybuff, 0, incoming.Count);
                string s = System.Text.UTF8Encoding.Default.GetString(mybuff);
                Console.WriteLine($"Recevie Something: {s}");

                try
                {
                    var g = JsonConvert.DeserializeObject<SDP>(s);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (s.Length > 0)
                {
                    if (index == 0)
                    {
                        if(OfferICE.Length == 0)
                            OfferICE = s;
                    }
                    if (index == 1)
                    {
                        if (AnswerICE.Length == 0)
                        {
                            AnswerICE = s;
                            await SendStringAsync(clients[index - 1], AnswerICE);
                        }
                    }
                }
            }
        }

        public async Task SendStringAsync(WebSocket socket,string text)
        {
            byte[] pack = System.Text.UTF8Encoding.Default.GetBytes(text);
            var outgoing = new ArraySegment<byte>(pack, 0, pack.Length);
            await socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public class SDP : PacketBase
        {
            public string Json;
        }

        public class PacketBase
        {
            public string Operator;   
        }
    }
}
