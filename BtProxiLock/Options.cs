using CommandLine;

namespace BtProxiLock
{
    internal class Options
    {
        [Option('d', Required = false)]
        public bool StartupDaemon { get; set; }

        [Option('t', Required = false)]
        public bool ShutdownDeamon { get; set; }
    }
}