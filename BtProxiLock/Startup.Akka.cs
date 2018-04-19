namespace BtProxiLock
{
    using Akka.Actor;
    using Akka.Configuration;
    using BtProxiLockActors;
    using BtProxiLockActors.Actors;

    public class Startup
    {
        private static Config ConfigClient = ConfigurationFactory.ParseString(@"
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

        public static void StartClientActorSystem()
        {
            var system = BtProxiLockClientActorRefs.System = ActorSystem.Create("BtProxiLockClientActorSystem", ConfigClient);
            BtProxiLockClientActorRefs.DeviceDetectionActor = system.ActorOf<DeviceDetectionActor>("DeviceDetectionActor");
            BtProxiLockClientActorRefs.CommunicationActor = system.ActorSelection("akka.tcp://BtProxiLockServerActorSystem@localhost:9001/user/CommunicationActor");
        }
    }
}