
namespace Signaling2.Network.Packet
{
    public class Sdp : IBase
    {
        public const string MyOperator = "SaveSDP";
        public string Json;
        public Sdp()
        {
            Operator = MyOperator;
        }
    }
}
