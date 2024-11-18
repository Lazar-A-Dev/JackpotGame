using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Jackpot.Domain.Client;
using System.Reflection.Metadata.Ecma335;

namespace Client
{
    public class MainMenu
    {
        public void TheMainManu(int currentCredit, IModel channel, string responseQueueName) {
            //This part receives confirmation for jackpot from the server

            var client = new EventingBasicConsumer(channel);
            client.Received += (sender, e) =>
            {
                string message = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                //Check if string contains any numbers, if not then print message:

                bool isIntString = message.All(char.IsDigit);
                if (!isIntString)
                {
                    Console.WriteLine("Message from jackpot server: " + message + "\n");

                }
                else
                {
                    int value = Int32.Parse(message);
                    currentCredit += value;
                }
            };

            channel.BasicConsume(responseQueueName, true, client);

            while (currentCredit > 0)
            {

                PrintData printData = new PrintData();
                printData.PrintTheData(currentCredit, 0);

                //Here we test if the inputed value is an int or a string
                var testImput = Console.ReadLine();
                int number;

                bool res = Int32.TryParse(testImput, out number);
                if (res == true)
                {
                    int bet = Convert.ToInt32(testImput);
                    if (bet == 0)
                        break;

                    while (currentCredit - bet > 0)
                    {
                        
                        //We increase the betting points by the inserted
                        Console.WriteLine("Would you like to increse/decrease your bet, play or go back (i/d/p/g)?");
                        string choice = Console.ReadLine();


                        //This part is for increasing the bet -------------------->
                        if (choice == "i")
                        {
                            IncreaseBet increaseBet = new IncreaseBet();
                            bet = increaseBet.IncreaseTheBet(number, bet, currentCredit);
                            printData.PrintTheData(currentCredit, bet);
                        }

                        // This part is for deincreasing the bet ------------------->
                        else if (choice == "d")
                        {
                            DecreaseBet decreaseBet = new DecreaseBet();
                            bet = decreaseBet.DecreaseTheBet(number, bet, currentCredit);
                            printData.PrintTheData(currentCredit, bet);
                        }
                        // This part is for playing the game ------------------->
                        else if (choice == "p")
                        {
                            PlayGame playGame = new PlayGame();
                            currentCredit = playGame.PlayTheGame(currentCredit, bet, responseQueueName, channel);

                        }
                        // This part is for going back ------------------->
                        else if (choice == "g" || bet == 0) {
                            break;
                        }

                        else
                            Console.WriteLine("You must enter the given values, try again\n");
                    }
                }
                else
                    Console.WriteLine("You must enter a numeric value, try again\n");
            }
        }
    }
}
