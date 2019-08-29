using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signaling2.Network
{
    public class ProxyManager
    {
        private readonly List<ClientProxy> Clients = new List<ClientProxy>();

        public void Add(ClientProxy client)
        {
            if (!Clients.Contains(client))
                Clients.Add(client);
        }

        public void Remove(ClientProxy client)
        {
            Clients.Remove(client);
        }

        public void Broadcast(string value,ClientProxy exception=null)
        {
            foreach (var client in Clients)
            {
                if (client == exception)
                    continue;
                client.Net.SendStringAsync(value);
            }
        }
        public bool TryFindFirstSDP(out string sdp)
        {
            bool result = false;
            sdp = "";

            foreach (var client in Clients)
            {
                if (client.TryGetSDP(out sdp))
                    break;
            }
            return result;
        }
    }
}
