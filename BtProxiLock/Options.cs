namespace BtProxiLock
{
    using CommandLine;

    internal class Options
    {
        [Option('l', HelpText = "List nearby devices. Hint: Device needs to be in pairing mode.")]
        public bool DeviceDetection { get; set; }

        [Option('b', HelpText = "Start background server.")]
        public bool StartBackground { get; set; }

        [Option('t', HelpText = "Terminate background server.")]
        public bool TerminateServer { get; set; }

        [Option('a', HelpText = "Bluetooth address of device to monitor.")]
        public string BluetoothAddress { get; set; }

        [Option('i', HelpText = "Monitoring interval in milliseconds. >= 1000", Default = 10000)]
        public int Interval { get; set; }

        [Option('s', HelpText = "Check current status.")]
        public bool Status { get; set; }
    }
}