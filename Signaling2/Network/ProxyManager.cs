using Newtonsoft.Json;
using Signaling2.Network.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signaling2.Network
{
    public class ProxyManager
    {
        private readonly List<ClientProxy> Clients = new List<ClientProxy>();

        public int Count()
        {
            return Clients.Count;
        }
        public void Add(ClientProxy client)
        {
            if (!Clients.Contains(client))
                Clients.Add(client);
        }

        public void Remove(ClientProxy client)
        {
            Clients.Remove(client);
        }

        public async Task Broadcast(string value,ClientProxy exception=null)
        {
            foreach (var client in Clients)
            {
                if (client == exception)
                    continue;
                await client.Net.SendStringAsync(value);
            }
        }
        public void BroadcastCandidate()
        {
            foreach (var client in Clients)
            {
                client.StartBroadcastCandidate = true;
                client.Candidates.ForEach(async candidate =>
               {
                   var json = JsonConvert.SerializeObject(new Candidate() { Json = candidate });
                   await Broadcast(json, client);
               });
            }
        }
        public bool TryFindFirstSDP(out string sdp)
        {
            bool result = false;
            sdp = "";

            foreach (var client in Clients)
            {
                result = client.TryGetSDP(out sdp);
                if (result)
                    break;
            }
            return result;
        }
    }
}
