using Akka.Actor;

namespace BtProxiLockActors
{
    /// <summary>
    /// Class to make globally accessible actors available
    /// </summary>
    public static class BtProxiLockClientActorRefs
    {
        public static ActorSystem System;
    }
}