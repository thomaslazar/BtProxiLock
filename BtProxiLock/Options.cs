using CommandLine;

namespace BtProxiLock
{
    internal class Options
    {
        public Options(bool daemon)
        {
            this.Daemon = daemon;
        }

        [Option('d')]
        public bool Daemon { get; }
    }
}