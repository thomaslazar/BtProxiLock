namespace BtProxiLockActors.Actors
{
    using System;
    using Akka.Actor;
    using BtProxiLockActors.Messages;
    using InTheHand.Net.Sockets;

    /// <summary>
    /// Device detection actor used to detect Bluetooth devices in pairing mode
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class DeviceDetectionActor : ReceiveActor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDetectionActor"/> class.
        /// </summary>
        public DeviceDetectionActor()
        {
            Receive<DetectDevicesMsg>(_ =>
            {
                DiscoverBluetoothDevice();
                Sender.Tell(new ReceivedMsg());
            });
        }

        private BluetoothDeviceInfo[] DiscoverBluetoothDevice()
        {
            var btClient = new BluetoothClient();

            Console.Write("Detecting devices...");
            var devices = btClient.DiscoverDevices();
            Console.WriteLine();

            if (devices.Length == 0)
            {
                Console.WriteLine("No devices found.");
                return null;
            }

            Console.WriteLine("Bluetooth devices");
            foreach (var device in devices)
            {
                var blueToothInfo =
                    $"- DeviceName: {device.DeviceName}\n Address: {device.DeviceAddress}\n Last seen: {device.LastSeen}\n Last used: {device.LastUsed}\n";

                blueToothInfo += $"  Class of device\n   Device: {device.ClassOfDevice.Device}\n   Major Device: {device.ClassOfDevice.MajorDevice}\n   Service: {device.ClassOfDevice.Service}";
                Console.WriteLine(blueToothInfo);
                Console.WriteLine();
            }

            return devices;
        }
    }
}