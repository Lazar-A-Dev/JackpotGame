using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class PlayGame
    {
        public int PlayTheGame(int currentCredit, int bet, string responseQueueName, IModel channel)
        {
            currentCredit -= bet;
            int value;

            //PrintData(currentCredit, bet);

            Random rnd = new Random();

            if (bet <= 100)
            {
                value = rnd.Next(1, 5);
                if (value == 4)
                {
                    currentCredit += bet * 2;
                    Console.WriteLine("Congratulations on the win, win credits increased by:" + bet * 2 + "\n");
                }
                else
                    Console.WriteLine("Oops, try again\n");

            }
            else if (bet <= 500)
            {
                value = rnd.Next(1, 6);
                if (value == 5)
                {
                    currentCredit += bet * 2;
                    Console.WriteLine("Congratulations on the win, win credits increased by:" + bet * 2 + "\n");
                }
                else
                    Console.WriteLine("Oops, try again\n");
            }
            else if (bet <= 1000)
            {
                value = rnd.Next(1, 8);
                if (value == 7)
                {
                    currentCredit += bet * 2;
                    Console.WriteLine("Congratulations on the win, win credits increased by:" + bet * 2 + "\n");
                }
                else
                    Console.WriteLine("Oops, try again\n");
            }
            else if (bet > 1000)
            {
                value = rnd.Next(1, 10);
                if (value == 8)
                {
                    currentCredit += bet * 2;
                    Console.WriteLine("Congratulations on the win, win credits increased by:" + bet * 2 + "\n");
                }
                else
                    Console.WriteLine("Oops, try again\n");
            }

            PrintData printData = new PrintData();
            printData.PrintTheData(currentCredit, bet);
            //PrintData(currentCredit, bet);

            //We sent the current bet to the server
            string request = responseQueueName + "|" + bet.ToString();
            channel.BasicPublish("", "jackpot", null, Encoding.UTF8.GetBytes(request));

            return currentCredit;
        }
    }
}
