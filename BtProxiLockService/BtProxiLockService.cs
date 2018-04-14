using Akka.Actor;
using Akka.Configuration;

namespace BtProxiLockService
{
    /// <summary>
    /// Custom TopShelf service class
    /// </summary>
    public class BtProxiLockService
    {
        private ActorSystem _actorSystem = null;

        private readonly Config _config = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = remote
                }
                remote {
                    dot-netty.tcp {
                        port = 8081 #bound to a specific port
                        hostname = localhost
                    }
                }
            }
        ");

        /// <summary>
        /// Starts up the ActorSystem and initializing global actors.
        /// </summary>
        public void Start()
        {
            _actorSystem = ActorSystem.Create("BtProxiLockActorSystem", _config);
        }

        /// <summary>
        /// Shuts down the ActorSystem
        /// </summary>
        public void Stop()
        {
            _actorSystem.Terminate();
            _actorSystem.Dispose();
            _actorSystem = null;
        }
    }
}