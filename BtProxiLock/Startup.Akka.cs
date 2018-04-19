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
        private static Config configClient = ConfigurationFactory.ParseString(@"
            akka {
                stdout-loglevel = WARNING
                loglevel = WARNING
                actor {
                    provider = remote
                }
                remote {
                    dot-netty.tcp {
                        port = 0
                        hostname = localhost
                    }
                }
            }
        ");

        /// <summary>
        /// Initialise and start the client actor system.
        /// </summary>
        public static void StartClientActorSystem()
        {
            var system = BtProxiLockClientActorRefs.System = ActorSystem.Create("BtProxiLockClientActorSystem", configClient);
            BtProxiLockClientActorRefs.DeviceDetectionActor = system.ActorOf<DeviceDetectionActor>("DeviceDetectionActor");
            BtProxiLockClientActorRefs.CommunicationActor = system.ActorSelection("akka.tcp://BtProxiLockServerActorSystem@localhost:9001/user/CommunicationActor");
        }
    }
}