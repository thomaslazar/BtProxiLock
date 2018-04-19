namespace BtProxiLock
{
    using CommandLine;

    /// <summary>
    /// Class describing command line options
    /// </summary>
    internal class Options
    {
#pragma warning disable SA1600 // Elements must be documented

        [Option('l', "list", HelpText = "List nearby devices. Hint: Device needs to be in pairing mode.")]
        public bool DeviceDetection { get; set; }

        [Option('b', "background", HelpText = "Start background server.")]
        public bool StartBackground { get; set; }

        [Option('t', "terminate", HelpText = "Terminate background server.")]
        public bool TerminateServer { get; set; }

        [Option('a', "address", HelpText = "Bluetooth address of device to monitor.")]
        public string BluetoothAddress { get; set; }

        [Option('i', "interval", HelpText = "Monitoring interval in milliseconds. >= 1000", Default = 10000)]
        public int Interval { get; set; }

        [Option('s', "status", HelpText = "Check current status.")]
        public bool Status { get; set; }

        [Option("install", HelpText = "Add background server to autostart.")]
        public bool AddAutostart { get; set; }

        [Option("uninstall", HelpText = "Remove background server from autostart.")]
        public bool RemoveAutostart { get; set; }

#pragma warning restore SA1600 // Elements must be documented
    }
}