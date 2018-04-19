namespace BtProxiLock
{
    using System;
    using Akka.Actor;
    using BtProxiLockActors;
    using BtProxiLockActors.Messages;
    using CommandLine;
    using Microsoft.Win32;
    using static BtProxiLockShared.Global;

    /// <summary>
    /// Command line application for the BtProxiLock Server
    /// </summary>
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

            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            // Check if server already running
            bool result;
            var mutex = new System.Threading.Mutex(true, UniqueAppId, out result);

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

        private static ConfigureMsg CreateConfigureMsgFromOptions(Options options)
        {
            return new ConfigureMsg(options.BluetoothAddress ?? null, options.Intervall);
        }

        private static bool CheckServerRunning()
        {
            bool result;
            var mutex = new System.Threading.Mutex(true, UniqueAppId, out result);

            return !result;
        }
    }
}