namespace BtProxiLock
{
    using Akka.Actor;
    using Akka.Configuration;
    using BtProxiLockActors;
    using BtProxiLockActors.Actors;

    /// <summary>
    /// Startup class to initialise parts of the application
    /// </summary>
    public class Startup
    {
        private static Config configServer = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = remote
                }
                remote {
                    dot-netty.tcp {
                        port = 9001 #bound to a specific port
                        hostname = localhost
                    }
                }
            }
        ");

        /// <summary>
        /// Initialise and start the server actor system.
        /// </summary>
        public static void StartServerActorSystem()
        {
            var system = BtProxiLockServerActorRefs.System = ActorSystem.Create("BtProxiLockServerActorSystem", configServer);
            var lockingActor = BtProxiLockServerActorRefs.LockingActor = system.ActorOf<LockingActor>("LockingActor");
            BtProxiLockServerActorRefs.CommunicationActor = system.ActorOf(Props.Create(() => new CommunicationActor(lockingActor)), "CommunicationActor");
        }
    }
}