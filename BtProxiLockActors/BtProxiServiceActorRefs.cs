using Akka.Actor;

namespace BtProxiLockService
{
    /// <summary>
    /// Class to make globally accessible actors available
    /// </summary>
    public static class BtProxiServiceActorRefs
    {
        public static ActorSystem System;

        public static IActorRef CommunicationActor;

        public static IActorRef ConfigurationActor;

        public static IActorRef LockingActor;
    }
}