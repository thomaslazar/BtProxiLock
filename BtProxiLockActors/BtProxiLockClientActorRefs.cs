namespace BtProxiLockActors
{
    using Akka.Actor;

    /// <summary>
    /// Class to make globally accessible client actors available
    /// </summary>
    public static class BtProxiLockClientActorRefs
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
        public static ActorSelection CommunicationActor { get; set; }

        /// <summary>
        /// Gets or sets the device detection actor.
        /// </summary>
        /// <value>
        /// The device detection actor.
        /// </value>
        public static IActorRef DeviceDetectionActor { get; set; }
    }
}