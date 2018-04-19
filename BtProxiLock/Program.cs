namespace BtProxiLock
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Akka.Actor;
    using BtProxiLockActors;
    using BtProxiLockActors.Messages;
    using CommandLine;
    using static BtProxiLockShared.Global;

    internal class Program
    {
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
            if (options.Interval < 1000)
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
                var serverPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                var serverExe = Path.Combine(serverPath, "BtProxiLockSrv.exe");

                var pi = new ProcessStartInfo
                {
                    FileName = serverExe,
                    WorkingDirectory = serverPath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                var args = $"-i {options.Interval}";
                if (options.BluetoothAddress != null)
                {
                    args += $" -a {options.BluetoothAddress}";
                }

                pi.Arguments = args;
                Process.Start(pi);

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

            if (options.BluetoothAddress != null || options.Interval > 0)
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
            return new ConfigureMsg(options.BluetoothAddress ?? null, options.Interval);
        }

        private static bool CheckServerRunning()
        {
            bool result;
            var mutex = new System.Threading.Mutex(true, UniqueAppId, out result);

            return !result;
        }
    }
}