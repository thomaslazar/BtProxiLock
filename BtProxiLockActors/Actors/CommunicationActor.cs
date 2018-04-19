namespace BtProxiLockActors.Actors
{
    using Akka.Actor;
    using BtProxiLockActors.Messages;

    /// <summary>
    /// Communication actor used to forward messages towards various server actors.
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class CommunicationActor : ReceiveActor
    {
        private readonly IActorRef lockingActor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationActor"/> class.
        /// </summary>
        /// <param name="lockingActor">The locking actor.</param>
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