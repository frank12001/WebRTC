
namespace Signaling2.Network.Packet
{
    public class Candidate : IBase
    {
        public const string MyOperator = "SaveCandidate";
        public string Json;
        public Candidate()
        {
            Operator = MyOperator;
        }
    }
}
