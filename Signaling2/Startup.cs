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

namespace Signaling2
{
    public class Startup
    {
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
                        //°õ¦æ±µ¦¬ 
                        WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                        if (client1 == null)
                            client1 = socket;
                        if (client1 != null && client2 == null)
                            client2 = socket;
                        await Receive(socket);
                        Console.WriteLine("create connection");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            });
        }

        static WebSocket client1=null;
        static WebSocket client2=null;
        static string OfferICE="";
        static string AnswerICE = "";
        async Task Receive(WebSocket socket)
        {
            while (socket.State == WebSocketState.Open)
            {
                if (OfferICE.Length > 0)
                {
                    await SendStringAsync(socket, JsonConvert.SerializeObject(OfferICE));
                }
                var buffer = new byte[1024];
                var incoming = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                byte[] mybuff = new byte[incoming.Count];
                Array.Copy(buffer, 0, mybuff, 0, incoming.Count);
                string s = System.Text.UTF8Encoding.Default.GetString(mybuff);
                Console.WriteLine($"Recevie Something: {s}");

                if (s.Length > 0)
                {
                    if(socket == client1)
                        OfferICE = s;
                    if (socket == client2)
                    {
                        AnswerICE = s;
                        SendStringAsync(client1, AnswerICE);
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
    }
}
