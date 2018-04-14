using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace BtProxiLockService
{
    /// <summary>
    /// Program to control the TopShelf service
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<BtProxiLockService>(s =>
                {
                    s.ConstructUsing(name => new BtProxiLockService());
                    s.WhenStarted(btpls => btpls.Start());
                    s.WhenStopped(btpls => btpls.Stop());
                });
                x.RunAsLocalService();

                x.SetDescription("Bluetooth Proximity Lock Service");
                x.SetDisplayName("BtProxiLockService");
                x.SetServiceName("BtProxiLockService");

                x.UseNLog();
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}