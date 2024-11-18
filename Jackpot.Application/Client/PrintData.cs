using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class PrintData
    {
        public void PrintTheData(int currentCredit, int bettingPoints)
        {
            Console.WriteLine("--------------------------------------------\n");
            Console.WriteLine("Current credit: " + currentCredit + "\n");
            Console.WriteLine("Your current bet: " + bettingPoints + "\n");
            Console.WriteLine("Enter your bet[1000 | 500 | 100 | 0 to exit]: " + "\n");
        }
    }
}
