using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Signaling2.Network.Packet;

namespace Signaling2.Network
{
    public class ClientPoxy
    {            
        private readonly NetUnit Net;
        private readonly ProxyManager Manager;
        private string SDP="";
        public List<string> Candidates = new List<string>();

        public ClientPoxy(WebSocket socket, ProxyManager manager)
        {
            Net = new NetUnit(socket, Receive, Disconnect);
            this.Manager = manager;
            //check if someone has sdp
            if (Manager.TryFindFirstSDP(out string otherSDP))
            {
                //代表我是 answer
                //將別人的 SDP 送給自己
                var s = JsonConvert.SerializeObject(new Sdp() { Operator = "BroadcastSDP", Json = otherSDP });
                Send(s);
            }
        }
        public void Send(string value)
        {
            Net.SendStringAsync(value);
        }
        public bool TryGetSDP(out string sdp)
        {
            sdp = SDP;
            return (SDP.Length > 0);         
        }
        public void Disconnect()
        {
            Net.Dispose();
            Manager.Remove(this);
        }
        public void Receive(string value)
        {
            try
            {
                var packet = JsonConvert.DeserializeObject<IBase>(value);
                switch (packet.Operator)
                {
                    case "SaveSDP":
                        var sdp = JsonConvert.DeserializeObject<Sdp>(value);
                        this.SDP = sdp.Json;
                        break;
                    case "SaveCandidate":
                        var candidate = JsonConvert.DeserializeObject<Candidate>(value);
                        Candidates.Add(candidate.Json);
                        break;
                    case "BroadcastSDP":
                        var broadcastSDP = JsonConvert.DeserializeObject<Sdp>(value);
                        this.SDP = broadcastSDP.Json;
                        var s = JsonConvert.SerializeObject(new Sdp() { Operator = "BroadcastSDP", Json = this.SDP });
                        Manager.Broadcast(s,this);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
