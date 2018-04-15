using System;
using Akka.Actor;
using Akka.Configuration;
using BtProxiLockActors;
using BtProxiLockActors.Messages;
using CommandLine;
using Microsoft.Win32;

namespace BtProxiLock
{
    internal class Program
    {
        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                BtProxiLockServerActorRefs.LockingActor.Tell(new WorkstationUnlockedMsg());
            }
        }

        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunAndReturnExitCode(options));
        }

        private static void RunAndReturnExitCode(Options options)
        {
            if (options.StartupDaemon)
            {
                SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

                //// Check if server already running
                bool result;
                var mutex = new System.Threading.Mutex(true, "UniqueAppId", out result);

                if (!result)
                {
                    return;
                }

                // Starting server
                Startup.StartServerActorSystem();

                if (options.BluetoothAddress != null || options.Intervall > 0)
                {
                    BtProxiLockServerActorRefs.LockingActor.Tell(CreateConfigureMsgFromOptions(options));
                }

                GC.KeepAlive(mutex);

                var task = BtProxiLockServerActorRefs.System.WhenTerminated;
                task.Wait();
                return;
            }

            Startup.StartClientActorSystem();

            var clientSystem = BtProxiLockClientActorRefs.System;
            var commActor = clientSystem.ActorSelection("akka.tcp://BtProxiLockServerActorSystem@localhost:9001/user/CommunicationActor");

            if (options.BluetoothAddress != null || options.Intervall > 0)
            {
                commActor.Tell(CreateConfigureMsgFromOptions(options));
            }

            if (options.ShutdownDaemon)
            {
                var task = commActor.Ask<ReceivedMsg>(new ShutdownMsg());
                task.Wait(TimeSpan.FromSeconds(5));
                if (task.IsCompleted)
                {
                    Console.WriteLine("Shutting down daemon.");
                }
            }
        }

        private static ConfigureMsg CreateConfigureMsgFromOptions(Options options)
        {
            return new ConfigureMsg(options.BluetoothAddress ?? null, options.Intervall);
        }
    }
}