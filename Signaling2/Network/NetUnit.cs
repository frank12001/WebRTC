using System;
using System.Text;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Signaling2.Network
{
    public class NetUnit : IDisposable
    {
        public static List<int> getPacketHistory = new List<int>();
        public static List<string> getPacketHistoryType = new List<string>();
        public static int QueueUpdateCount = 0;
        public static int CallIncomeCount = 0;

        private const int BufferSize = 10240;
        private string MessageBuffer = "";
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
            Task.Run(async () => 
            {
                while (IsSocketOpen())
                {
                    QueueUpdateCount++;
                    QueueUpdate();
                    if (Logic.Count > 0)
                    {
                        CallIncomeCount++;
                        var packet = Logic.Dequeue();
                        this.onIncome(packet);
                    }
                    await Task.Delay(30);
                }
                Dispose();
            });
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
                getPacketHistoryType.Add(JsonConvert.SerializeObject(incoming));
                var mybuff = new byte[incoming.Count];
                Array.Copy(buffer, 0, mybuff, 0, incoming.Count);
                var packet = Encoding.Default.GetString(mybuff);

                if (incoming.EndOfMessage)
                {
                    var s = MessageBuffer + packet;
                    NetIncome.Enqueue(s);
                    MessageBuffer = "";
                }
                else
                {
                    MessageBuffer += packet;
                }

                getPacketHistory.Add(packet.Length);
            }
            Dispose();
        }

        private bool IsSocketOpen()
        {
            return Socket.State == WebSocketState.Open;
        }
    }
}
