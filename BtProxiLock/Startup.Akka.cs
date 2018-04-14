using Akka.Actor;
using Akka.Configuration;
using BtProxiLockActors;
using BtProxiLockActors.Actors;

namespace BtProxiLock
{
    public class Startup
    {
        private static Config _configServer = ConfigurationFactory.ParseString(@"
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

        private static Config _configClient = ConfigurationFactory.ParseString(@"
            akka {
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

        public static void StartServerActorSystem()
        {
            var system = BtProxiLockServerActorRefs.System = ActorSystem.Create("BtProxiLockServerActorSystem", _configServer);
            BtProxiLockServerActorRefs.CommunicationActor = system.ActorOf<CommunicationActor>("CommunicationActor");
            BtProxiLockServerActorRefs.ConfigurationActor = system.ActorOf<ConfigurationActor>("ConfigurationActor");
            BtProxiLockServerActorRefs.LockingActor = system.ActorOf<LockingActor>("LockingActor");
        }

        public static void StartClientActorSystem()
        {
            var system = BtProxiLockClientActorRefs.System = ActorSystem.Create("BtProxiLockClientActorSystem", _configClient);
        }
    }
}