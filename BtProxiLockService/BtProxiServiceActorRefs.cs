using Akka.Actor;

namespace BtProxiLockService
{
    /// <summary>
    /// Class to make globally accessible actors available 
    /// </summary>
    public class BtProxiServiceActorRefs
    {
        private readonly ActorSystem _actorSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="BtProxiServiceActorRefs"/> class.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        public BtProxiServiceActorRefs(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }
    }
}
