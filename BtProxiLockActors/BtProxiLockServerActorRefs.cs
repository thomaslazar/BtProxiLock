namespace BtProxiLockActors
{
    using Akka.Actor;

    /// <summary>
    /// Class to make globally accessible actors available
    /// </summary>
    public static class BtProxiLockServerActorRefs
    {
        public static ActorSystem System;

        public static IActorRef CommunicationActor;

        public static IActorRef LockingActor;
    }
}