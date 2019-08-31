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
        static readonly ProxyManager Manager = new ProxyManager();
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
                endpoints.MapGet("/getSaveSDPCount", async context =>
                {
                    Manager.TryFindFirstSDP(out string sdp);
                    await context.Response.WriteAsync($"ClientProxy.getSaveSDPCount: {ClientProxy.getSaveSDPCount},");

    //                await context.Response.WriteAsync($"Manager Count: {Manager.Count()}, First SDP: {sdp}, " +
    //$"ClientProxy.getSaveSDPCount: {ClientProxy.getSaveSDPCount}, " +
    //$"NetUnit.getPacketHistory: {JsonConvert.SerializeObject(NetUnit.getPacketHistory)}, " +
    //$"NetUnit.QueueUpdateCount: {NetUnit.QueueUpdateCount}, " +
    //$"NetUnit.CallIncomeCount: {NetUnit.CallIncomeCount}" +
    //$"ClientProxy.ReceiveCount: {ClientProxy.ReceiveCount}" +
    //$"ClientProxy.ReceiveDefaultCount: {ClientProxy.ReceiveDefaultCount}" +
    //$"ClientProxy.ReceiveErrorCount: {ClientProxy.ReceiveErrorCount}" +
    //$"ClientProxy.ErrorsValue: {JsonConvert.SerializeObject(ClientProxy.ErrorsValues[0])}" +
    //$"NetUnit.getPacketHistoryType: {JsonConvert.SerializeObject(NetUnit.getPacketHistoryType)}");
                });
                endpoints.MapGet("/log", async context =>
                {
                    Manager.TryFindFirstSDP(out string sdp);
                    await context.Response.WriteAsync($"Manager Count: {Manager.Count()}, First SDP: {sdp}, " +
                        $"NetUnit.getPacketHistory: {JsonConvert.SerializeObject(NetUnit.getPacketHistory)}, ");


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
    }
}
