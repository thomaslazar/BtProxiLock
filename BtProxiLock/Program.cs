using System;
using Akka.Actor;
using Akka.Configuration;
using BtProxiLockActors;
using BtProxiLockActors.Messages;
using CommandLine;

namespace BtProxiLock
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunAndReturnExitCode(options));
        }

        private static void RunAndReturnExitCode(Options options)
        {
            if (options.StartupDaemon)
            {
                //// Check if server already running
                bool result;
                var mutex = new System.Threading.Mutex(true, "UniqueAppId", out result);

                if (!result)
                {
                    return;
                }

                // Starting server
                Startup.StartServerActorSystem();

                GC.KeepAlive(mutex);

                var task = BtProxiLockServerActorRefs.System.WhenTerminated;
                task.Wait();
            }

            if (options.ShutdownDeamon)
            {
                Startup.StartClientActorSystem();

                var clientSystem = BtProxiLockClientActorRefs.System;
                var commActor = clientSystem.ActorSelection("akka.tcp://BtProxiLockServerActorSystem@localhost:9001/user/CommunicationActor");
                var task = commActor.Ask<ReceivedMsg>(new ShutdownMsg());
                task.Wait(TimeSpan.FromSeconds(5));
                if (task.IsCompleted)
                {
                    Console.WriteLine("Answer received");
                }
            }
        }
    }
}