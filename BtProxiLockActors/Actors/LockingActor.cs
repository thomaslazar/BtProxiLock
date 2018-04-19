namespace BtProxiLockActors.Actors
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using Akka.Actor;
    using BtProxiLockActors.Messages;
    using InTheHand.Net;
    using InTheHand.Net.Sockets;

    /// <summary>
    /// Locking actor used to check if configured Bluetooth device is in range and if it isn't locks the workstation
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class LockingActor : ReceiveActor
    {
        private string bluetoothAddress = null;
        private int interval = 0;
        private bool workstationLocked = false;
        private bool doLock = false;
        private DateTime lastCheck = DateTime.Now;
        private bool firstTry = true;

        private ICancelable cancelToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockingActor"/> class.
        /// </summary>
        public LockingActor()
        {
            Receive<ConfigureMsg>(msg =>
            {
                if (msg.BluetoothAddress != null)
                {
                    bluetoothAddress = msg.BluetoothAddress;
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

                if ((now > lastCheck + TimeSpan.FromMilliseconds(interval) && lastInputTime >= interval * 2) || !firstTry)
                {
                    var inRange = IsBluetoothDeviceInRange(bluetoothAddress);

                    if (!inRange && firstTry)
                    {
                        // sometimes we get a false positive here, so we check again.
                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5), Self, new LockMsg(), ActorRefs.NoSender);
                        firstTry = false;
                        return;
                    }

                    firstTry = true;
                    lastCheck = now;

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

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        private static uint GetLastInputTimeInMilliseconds()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = default(LASTINPUTINFO);
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

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1121 // Use built-in type alias

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;

#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1121 // Use built-in type alias
        }
    }
}