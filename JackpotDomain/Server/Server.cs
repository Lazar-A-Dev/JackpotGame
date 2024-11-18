using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jackpot.Domain.Server
{
    public class Server
    {
        public int jackpotMinReq = 2000;
        public int jackpotMaxReq = 4000;
        public double percentigeBet = 0.16;
        public Dictionary<string, int> clientDictionary = new Dictionary<string, int>();
    }
}
