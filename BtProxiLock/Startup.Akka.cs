using Akka.Actor;
using Akka.Configuration;
using BtProxiLockActors;
using BtProxiLockActors.Actors;

namespace BtProxiLock
{
    public class Startup
    {
        public static Config ConfigServer = ConfigurationFactory.ParseString(@"
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

        public static Config ConfigClient = ConfigurationFactory.ParseString(@"
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

        public static void StartServerActorSystem()
        {
            var system = BtProxiLockServerActorRefs.System = ActorSystem.Create("BtProxiLockServerActorSystem", ConfigServer);
            var lockingActor = BtProxiLockServerActorRefs.LockingActor = system.ActorOf<LockingActor>("LockingActor");
            BtProxiLockServerActorRefs.CommunicationActor = system.ActorOf(Props.Create(() => new CommunicationActor(lockingActor)), "CommunicationActor");
        }

        public static void StartClientActorSystem()
        {
            var system = BtProxiLockClientActorRefs.System = ActorSystem.Create("BtProxiLockClientActorSystem", ConfigClient);
            BtProxiLockClientActorRefs.DeviceDetectionActor = system.ActorOf<DeviceDetectionActor>("DeviceDetectionActor");
            BtProxiLockClientActorRefs.CommunicationActor = system.ActorSelection("akka.tcp://BtProxiLockServerActorSystem@localhost:9001/user/CommunicationActor");
        }
    }
}