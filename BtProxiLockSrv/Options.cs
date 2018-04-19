namespace BtProxiLock
{
    using CommandLine;

    /// <summary>
    /// Class describing command line options
    /// </summary>
    internal class Options
    {
#pragma warning disable SA1600 // Elements must be documented

        [Option('a', HelpText = "Bluetooth address of device to monitor.")]
        public string BluetoothAddress { get; set; }

        [Option('i', HelpText = "Monitoring interval in milliseconds. >= 1000", Default = 10000)]
        public int Intervall { get; set; }

#pragma warning restore SA1600 // Elements must be documented
    }
}