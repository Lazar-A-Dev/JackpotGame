using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jackpot.Domain.Client
{
    public class Client
    {
        public int currentCredit = 100000;
        public string responseQueueName = "client." + Guid.NewGuid().ToString();
    }
}
