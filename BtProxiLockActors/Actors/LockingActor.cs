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
                var lastInputTime = GetLastInputTimeInMilliseconds();

                if (now > lastCheck + TimeSpan.FromMilliseconds(interval) && lastInputTime >= interval * 2)
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

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        private static uint GetLastInputTimeInMilliseconds()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return idleTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
    }
}