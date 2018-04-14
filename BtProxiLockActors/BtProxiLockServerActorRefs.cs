using Akka.Actor;

namespace BtProxiLockActors
{
    /// <summary>
    /// Class to make globally accessible actors available
    /// </summary>
    public static class BtProxiLockServerActorRefs
    {
        public static ActorSystem System;

        public static IActorRef CommunicationActor;

        public static IActorRef ConfigurationActor;

        public static IActorRef LockingActor;
    }
}