using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class IncreaseBet
    {
        public int IncreaseTheBet(int number, int bettingPoints, int currentCredit)
        {

            Console.WriteLine("Insert bet for which you would like to increse the bet: (100, 500, 1000)");
            var increaseTest = Console.ReadLine();
            bool inc = Int32.TryParse(increaseTest, out number);
            while (true)
            {
                if (inc == true)
                {
                    int newBet = Convert.ToInt32(increaseTest);
                    if (bettingPoints + newBet <= currentCredit)
                    {
                        bettingPoints += newBet;
                        newBet = 0;
                        return bettingPoints;
                    }
                    else
                        Console.WriteLine("You can not bet above your current creddit points");
                }
                else
                {
                    Console.WriteLine("Please enter a valid input");
                }
            }

        }
    }
}
