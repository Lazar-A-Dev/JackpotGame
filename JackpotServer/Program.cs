using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Jackpot.Domain.Server;
using Jackpot.Infra.RabbitConnection;

namespace JackpotServer
{
    class Program
    {
        static void Main(string[] args)
        {

            Jackpot.Domain.Server.Server domServer = new Server();

            Dictionary<string, int> clientDictionary = domServer.clientDictionary;

            int jackpotMinReq = domServer.jackpotMinReq;
            int jackpotMaxReq = domServer.jackpotMaxReq;
            Console.WriteLine("MinJackpotValue: " + jackpotMinReq + " MaxJackpotValue: " + jackpotMaxReq + "\n");

            Random rnd = new Random();
            int value = rnd.Next(jackpotMinReq, jackpotMaxReq);

            RabbitConnection rc = new RabbitConnection();
            IConnection connection = rc.Connection;
            IModel channel = connection.CreateModel();

            var server = new EventingBasicConsumer(channel);
            server.Received += (sender, e) =>
            {
                string request = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                //Console.WriteLine("Request received:" + request);

                char[] spearator = { '|' };
                Int32 count = 2;

                // Here we split the request into 2 parts, the string key and the int value
                string[] strlist = request.Split(spearator,
                       count, StringSplitOptions.None);

                int numVal = (int) (double.Parse(strlist[1]) * domServer.percentigeBet);
                //Random rnd = new Random();
                //int value = rnd.Next(jackpotMinReq, jackpotMaxReq);
                int testValue = 0;

                ModifyDictionary modifyDictionary = new ModifyDictionary();
                clientDictionary = modifyDictionary.ModifyTheDictionary(clientDictionary, strlist,testValue, numVal, value, jackpotMinReq, jackpotMaxReq, channel);


            };
            channel.BasicConsume("jackpot", true, server);
            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            connection.Close();
        }
    }
}
