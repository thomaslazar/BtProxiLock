namespace BtProxiLock
{
    using CommandLine;

    internal class Options
    {
        [Option('a', HelpText = "Bluetooth address of device to monitor.")]
        public string BluetoothAddress { get; set; }

        [Option('i', HelpText = "Monitoring interval in milliseconds. >= 1000", Default = 10000)]
        public int Intervall { get; set; }
    }
}