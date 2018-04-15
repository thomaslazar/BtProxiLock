using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Akka.Actor;
using BtProxiLockActors.Messages;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace BtProxiLockActors.Actors
{
    public class LockingActor : ReceiveActor
    {
        private string bluetoothAddress = null;
        private int interval = 0;
        private bool workstationLocked = false;
        private bool doLock = false;
        private DateTime lastCheck = DateTime.Now;

        private ICancelable cancelToken;

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        public LockingActor()
        {
            Receive<ConfigureMsg>(msg =>
            {
                if (msg.Address != null)
                {
                    bluetoothAddress = msg.Address;
                }

                if (msg.Interval > 0)
                {
                    interval = msg.Interval;
                }

                Self.Tell(new ScheduleLockMsg());
            });

            Receive<ScheduleLockMsg>(_ =>
            {
                if (interval == 0)
                {
                    return;
                }

                if (cancelToken != null)
                {
                    return;
                }

                cancelToken = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(0, interval, Self, new LockMsg(), ActorRefs.NoSender);
            });

            Receive<WorkstationUnlockedMsg>(_ =>
            {
                workstationLocked = false;

                Self.Tell(new ScheduleLockMsg());
            });

            Receive<LockMsg>(_ =>
            {
                if (interval == 0)
                {
                    return;
                }

                if (bluetoothAddress == null)
                {
                    return;
                }

                if (workstationLocked)
                {
                    return;
                }

                var now = DateTime.Now;

                if (now > lastCheck + TimeSpan.FromMilliseconds(interval))
                {
                    lastCheck = now;
                    var inRange = IsBluetoothDeviceInRange(bluetoothAddress);

                    if (inRange && !doLock)
                    {
                        doLock = true;
                        Self.Tell(new ScheduleLockMsg());
                    }

                    if (!inRange && doLock)
                    {
                        cancelToken.CancelIfNotNull();
                        cancelToken = null;
                        workstationLocked = true;
                        doLock = false;
                        LockWorkStation();
                    }
                }
            });
        }

        private bool IsBluetoothDeviceInRange(string address)
        {
            bool inRange;

            // A specially created value, so no matches.
            Guid fakeUuid = new Guid("{F13F471D-47CB-41d6-9609-BAD0690BF891}");

            BluetoothAddress btAddress;

            BluetoothAddress.TryParse(address, out btAddress);

            BluetoothDeviceInfo d = new BluetoothDeviceInfo(btAddress);
            try
            {
                var records = d.GetServiceRecords(fakeUuid);
                inRange = true;
            }
            catch (SocketException)
            {
                inRange = false;
            }

            return inRange;
        }
    }
}