namespace BtProxiLock
{
    using Akka.Actor;
    using Akka.Configuration;
    using BtProxiLockActors;
    using BtProxiLockActors.Actors;

    public class Startup
    {
        private static Config ConfigServer = ConfigurationFactory.ParseString(@"
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

        public static void StartServerActorSystem()
        {
            var system = BtProxiLockServerActorRefs.System = ActorSystem.Create("BtProxiLockServerActorSystem", ConfigServer);
            var lockingActor = BtProxiLockServerActorRefs.LockingActor = system.ActorOf<LockingActor>("LockingActor");
            BtProxiLockServerActorRefs.CommunicationActor = system.ActorOf(Props.Create(() => new CommunicationActor(lockingActor)), "CommunicationActor");
        }
    }
}