using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security;
using Jackpot.Domain.Client;
using Jackpot.Infra.RabbitConnection;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            Jackpot.Domain.Client.Client client = new Jackpot.Domain.Client.Client();
            int currentCredit = client.currentCredit;//Reading from Domain

            RabbitConnection rc = new RabbitConnection();

            IConnection connection = rc.Connection;//factory.CreateConnection();
            IModel channel = connection.CreateModel();

            string responseQueueName = client.responseQueueName;//Reading from Domain
            channel.QueueDeclare(responseQueueName);

            MainMenu mainMenu = new MainMenu();
            mainMenu.TheMainManu(currentCredit, channel, responseQueueName);


            Console.WriteLine("Thank you for playing, please come again\n");
            channel.Close();
            connection.Close();
        }

    }
}
