namespace BtProxiLockActors.Actors
{
    using Akka.Actor;
    using BtProxiLockActors.Messages;

    public class CommunicationActor : ReceiveActor
    {
        private readonly IActorRef lockingActor;

        public CommunicationActor(IActorRef lockingActor)
        {
            this.lockingActor = lockingActor;

            Receive<ShutdownMsg>(msg =>
            {
                Sender.Tell(new ReceivedMsg());
                Context.System.Terminate();
            });

            Receive<ConfigureMsg>(msg => lockingActor.Forward(msg));
        }
    }
}