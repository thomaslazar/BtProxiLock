namespace BtProxiLockActors
{
    using Akka.Actor;

    /// <summary>
    /// Class to make globally accessible server actors available
    /// </summary>
    public static class BtProxiLockServerActorRefs
    {
        /// <summary>
        /// Gets or sets the ActorSystem.
        /// </summary>
        /// <value>
        /// The ActorSystem.
        /// </value>
        public static ActorSystem System { get; set; }

        /// <summary>
        /// Gets or sets the communication actor.
        /// </summary>
        /// <value>
        /// The communication actor.
        /// </value>
        public static IActorRef CommunicationActor { get; set; }

        /// <summary>
        /// Gets or sets the locking actor.
        /// </summary>
        /// <value>
        /// The locking actor.
        /// </value>
        public static IActorRef LockingActor { get; set; }
    }
}