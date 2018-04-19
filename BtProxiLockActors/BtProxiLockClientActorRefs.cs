﻿namespace BtProxiLockActors
{
    using Akka.Actor;

    /// <summary>
    /// Class to make globally accessible actors available
    /// </summary>
    public static class BtProxiLockClientActorRefs
    {
        public static ActorSystem System;

        public static ActorSelection CommunicationActor;

        public static IActorRef DeviceDetectionActor;
    }
}