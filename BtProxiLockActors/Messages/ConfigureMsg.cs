namespace BtProxiLockActors.Messages
{
    /// <summary>
    /// Immutable configuration message class encapsulating configuration options
    /// </summary>
    public class ConfigureMsg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureMsg"/> class.
        /// </summary>
        /// <param name="address">The Bluetooth address.</param>
        /// <param name="interval">The interval.</param>
        public ConfigureMsg(string address, int interval)
        {
            BluetoothAddress = address;
            Interval = interval;
        }

        /// <summary>
        /// Gets the Bluetooth address.
        /// </summary>
        /// <value>
        /// The Bluetooth address.
        /// </value>
        public string BluetoothAddress { get; }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public int Interval { get; }
    }
}