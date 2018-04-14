using System;
using Akka.Actor;
using Akka.Configuration;
using CommandLine;

namespace BtProxiLock
{
    internal class Program
    {
        private static Config _configServer = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = remote
                }
                remote {
                    dot-netty.tcp {
                        port = 8081 #bound to a specific port
                        hostname = localhost
                    }
                }
            }
        ");

        private static Config _configClient = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = remote
                }
                remote {
                    dot-netty.tcp {
                        port = 8081 #bound to a specific port
                        hostname = localhost
                    }
                }
            }
        ");

        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunAndReturnExitCode(options));
        }

        private static void RunAndReturnExitCode(Options options)
        {
            if (options.Daemon)
            {
                var actorSystem = ActorSystem.Create("BtProxiLockActorSystem", _configServer);

                var task = actorSystem.WhenTerminated;
                task.Wait();
            }
        }
    }
}