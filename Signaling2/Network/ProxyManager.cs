using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signaling2.Network
{
    public class ProxyManager
    {
        private readonly List<ClientPoxy> Clients = new List<ClientPoxy>();

        public void Add(ClientPoxy client)
        {
            if (!Clients.Contains(client))
                Clients.Add(client);
        }

        public void Remove(ClientPoxy client)
        {
            Clients.Remove(client);
        }

        public void Broadcast(string value,ClientPoxy exception=null)
        {
            foreach (var client in Clients)
            {
                if (client == exception)
                    continue;
                client.Send(value);
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
