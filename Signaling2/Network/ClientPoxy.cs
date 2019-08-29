using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Signaling2.Network
{
    public class ClientPoxy
    {            
        private readonly NetUnit Net;
        private readonly ProxyManager Manager;

        public ClientPoxy(WebSocket socket, ProxyManager manager)
        {
            Net = new NetUnit(socket, Receive, Disconnect);
            this.Manager = manager;
        }
        public void Disconnect()
        {
            Net.Dispose();
            Manager.Remove(this);
        }
        public void Receive(string packet)
        {

        }

    }
}
