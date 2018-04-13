using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;

namespace BtProxiLockService
{
    public class BtProxiLockService
    {
        private ActorSystem _actorSystem = null;
        private readonly Config _config = ConfigurationFactory.ParseString(@"
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

        public BtProxiLockService()
        {

        }

        public void Start()
        {
            _actorSystem = ActorSystem.Create("BtProxiLockActorSystem", _config);
        }

        public void Stop()
        {
            _actorSystem.Terminate();
            _actorSystem.Dispose();
            _actorSystem = null;
        }
    }
}
