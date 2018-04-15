using System;
using System.Collections.Generic;
using System.Text;

namespace BtProxiLockActors.Messages
{
    public class ConfigureMsg
    {
        public ConfigureMsg(string address, int intervall)
        {
            Address = address;
            Interval = intervall;
        }

        public string Address { get; }

        public int Interval { get; }
    }
}