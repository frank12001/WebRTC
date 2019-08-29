using System;
using System.Text;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Signaling2.Network
{
    public class NetUnit : IDisposable
    {
        private const int BufferSize = 102400;
        private readonly WebSocket Socket;        
        private Queue<string> NetIncome = new Queue<string>();
        private Queue<string> Temp = new Queue<string>();
        private Queue<string> Logic = new Queue<string>();

        private Action<string> onIncome = null;
        private Action onDisconnect = null;

        private bool IsDispose = false;

        public NetUnit(WebSocket socket, Action<string> onIncome,Action onDisconnect)
        {
            Socket = socket;
            this.onIncome = onIncome;
            this.onDisconnect = onDisconnect;
            //Task.Run(async () => 
            //{
            //    while (true)
            //    {
            //        if (!IsSocketOpen())
            //        {
            //            Dispose();
            //            return;
            //        }
            //        await NetworkUpdate();
            //    }
            //});
            Task.Run(async () => 
            {
                while (IsSocketOpen())
                {
                    QueueUpdate();
                    var packet = Logic.Dequeue();
                    this.onIncome(packet);
                    await Task.Delay(30);
                }
                Dispose();
            });

            //async Task NetworkUpdate()
            //{
            //    var buffer = new byte[BufferSize];
            //    var incoming = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            //    var mybuff = new byte[incoming.Count];
            //    Array.Copy(buffer, 0, mybuff, 0, incoming.Count);
            //    var packet = Encoding.Default.GetString(mybuff);
            //    NetIncome.Enqueue(packet);
            //}

            void QueueUpdate()
            {
                //將 Temp 倒入 Logic
                while (Temp.Count != 0)
                {
                    Logic.Enqueue(Temp.Dequeue());
                }
                var temp = NetIncome;
                NetIncome = Temp;
                Temp = temp;
            }


        }
        public void Dispose()
        {
            if (!IsDispose)
            {
                IsDispose = true;
                onDisconnect();
                Socket.Dispose();
            }
        }
        public async Task SendStringAsync( string text)
        {
            byte[] pack = Encoding.Default.GetBytes(text);
            var outgoing = new ArraySegment<byte>(pack, 0, pack.Length);
            await Socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task ListenAsync()
        {
            while (IsSocketOpen())
            {
                var buffer = new byte[BufferSize];
                var incoming = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var mybuff = new byte[incoming.Count];
                Array.Copy(buffer, 0, mybuff, 0, incoming.Count);
                var packet = Encoding.Default.GetString(mybuff);
                NetIncome.Enqueue(packet);
            }
            Dispose();
        }

        private bool IsSocketOpen()
        {
            return Socket.State == WebSocketState.Open;
        }
    }
}
