using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtProxiLock
{
    internal class Options
    {
        public Options(bool daemon)
        {
            this.Daemon = daemon;
        }

        [Option('d')]
        public bool Daemon { get; }
    }
}