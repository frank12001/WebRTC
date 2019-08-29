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
    }
}
