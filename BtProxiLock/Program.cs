using System;
using System.Net.Sockets;
using Akka.Actor;
using Akka.Configuration;
using BtProxiLockActors;
using BtProxiLockActors.Actors;
using BtProxiLockActors.Messages;
using CommandLine;
using Microsoft.Win32;

namespace BtProxiLock
{
    internal class Program
    {
        private static string uniqueAppÍd = "BtProxiLock";

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
            if (args.Length == 0)
            {
                args = new string[] { "--help" };
            }

            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunAndReturnExitCode(options));
        }

        private static void RunAndReturnExitCode(Options options)
        {
            if (options.Intervall < 1000)
            {
                Console.WriteLine("Interval needs to be >= 1000.");
                return;
            }

            if (options.Status)
            {
                var result = CheckServerRunning();

                if (result)
                {
                    Console.WriteLine("Server currently running.");
                    return;
                }
                else
                {
                    Console.WriteLine("Server currently not running.");
                    return;
                }
            }

            if (options.StartBackground)
            {
                SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

                // Check if server already running
                bool result;
                var mutex = new System.Threading.Mutex(true, uniqueAppÍd, out result);

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

            if (options.DeviceDetection)
            {
                var task = BtProxiLockClientActorRefs.DeviceDetectionActor.Ask<ReceivedMsg>(new DetectDevicesMsg());
                task.Wait(TimeSpan.FromSeconds(30));
                return;
            }

            if (!CheckServerRunning())
            {
                return;
            }

            //var commActor = clientSystem.ActorSelection("akka.tcp://BtProxiLockServerActorSystem@localhost:9001/user/CommunicationActor");

            if (options.BluetoothAddress != null || options.Intervall > 0)
            {
                BtProxiLockClientActorRefs.CommunicationActor.Tell(CreateConfigureMsgFromOptions(options));
            }

            if (options.TerminateServer)
            {
                var task = BtProxiLockClientActorRefs.CommunicationActor.Ask<ReceivedMsg>(new ShutdownMsg());
                task.Wait(TimeSpan.FromSeconds(5));
                if (task.IsCompleted)
                {
                    Console.WriteLine("Shutting down server.");
                }
            }
        }

        private static ConfigureMsg CreateConfigureMsgFromOptions(Options options)
        {
            return new ConfigureMsg(options.BluetoothAddress ?? null, options.Intervall);
        }

        private static bool CheckServerRunning()
        {
            bool result;
            var mutex = new System.Threading.Mutex(true, uniqueAppÍd, out result);

            return !result;
        }
    }
}