using CommandLine;

namespace BtProxiLock
{
    internal class Options
    {
        // Options affecting server
        [Option('d')]
        public bool StartupDaemon { get; set; }

        [Option('a')]
        public string BluetoothAddress { get; set; }

        [Option('i')]
        public int Intervall { get; set; }

        // Options affecting client
        [Option('t')]
        public bool ShutdownDaemon { get; set; }
    }
}