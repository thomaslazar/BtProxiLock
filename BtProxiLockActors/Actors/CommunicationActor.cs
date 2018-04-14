using Akka.Actor;
using BtProxiLockActors.Messages;

namespace BtProxiLockActors.Actors
{
    public class CommunicationActor : ReceiveActor
    {
        public CommunicationActor()
        {
            Receive<ShutdownMsg>(msg =>
            {
                Sender.Tell(new ReceivedMsg());
                Context.System.Terminate();
            });
        }
    }
}