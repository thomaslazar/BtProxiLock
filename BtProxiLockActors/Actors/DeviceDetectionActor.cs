using System;
using Akka.Actor;
using BtProxiLockActors.Messages;
using InTheHand.Net.Sockets;

namespace BtProxiLockActors.Actors
{
    public class DeviceDetectionActor : ReceiveActor
    {
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
                    string.Format(
                        "- DeviceName: {0}{1}  Address: {2}{1}  Last seen: {3}{1}  Last used: {4}{1}",
                        device.DeviceName, Environment.NewLine, device.DeviceAddress, device.LastSeen,
                        device.LastUsed);

                blueToothInfo += string.Format("  Class of device{0}   Device: {1}{0}   Major Device: {2}{0}   Service: {3}",
                    Environment.NewLine, device.ClassOfDevice.Device, device.ClassOfDevice.MajorDevice,
                    device.ClassOfDevice.Service);
                Console.WriteLine(blueToothInfo);
                Console.WriteLine();
            }

            return devices;
        }
    }
}