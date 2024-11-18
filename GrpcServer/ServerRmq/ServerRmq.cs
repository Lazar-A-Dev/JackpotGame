using Grpc.Core;
using GrpcServer.Data;
using GrpcServer.Models;
using Jackpot.Infra.RabbitConnection;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Services;
using Jackpot.Domain.Server;
using System.Text;

namespace GrpcServer.ServerRmq
{
    public class ServerRmq
    {
        private readonly AppDbContext _dbContext;

        public ServerRmq(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void TheServerRmq(IModel channel, IConnection connection)
        {
            Jackpot.Domain.Server.Server domServer = new Server();
            Dictionary<string, int> clientDictionary = domServer.clientDictionary;

            int jackpotMinReq = domServer.jackpotMinReq;
            int jackpotMaxReq = domServer.jackpotMaxReq;

            Console.WriteLine("MinJackpotValue: " + jackpotMinReq + " MaxJackpotValue: " + jackpotMaxReq + "\n");

            Random rnd = new Random();
            int value = rnd.Next(jackpotMinReq, jackpotMaxReq);
            var server = new EventingBasicConsumer(channel);

            server.Received += async (sender, e) =>
            {
                string request = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                //Console.WriteLine("Request received:" + request);

                char[] spearator = { '|' };
                Int32 count = 2;

                // Here we split the request into 2 parts, the string key and the int value
                string[] strlist = request.Split(spearator, count, StringSplitOptions.None);

                int numVal = (int)(double.Parse(strlist[1]) * domServer.percentigeBet);
                //Random rnd = new Random();
                //int value = rnd.Next(jackpotMinReq, jackpotMaxReq);
                int testValue = 0;

                if (!clientDictionary.ContainsKey(strlist[0]))
                {
                    clientDictionary.Add(strlist[0], numVal);
                    Console.WriteLine("\nClient: " + strlist[0] + " was added to the dictionary !\n");
                }
                //if the dictionary alredy has the user
                else if (clientDictionary.ContainsKey(strlist[0]))
                {
                    //testValue help us keep track how much points the player accumulated
                    testValue = clientDictionary[strlist[0]] + numVal;
                    if (testValue >= value && testValue >= jackpotMinReq && testValue <= jackpotMaxReq)
                    {

                        clientDictionary[strlist[0]] = 0;

                        foreach (KeyValuePair<string, Int32> author in clientDictionary)
                        {
                            //We inform all the clients that one user won a jackpot
                            string response = "The user: |" + strlist[0] + "| has won a jackpot!!";
                            channel.BasicPublish("", author.Key, null, Encoding.UTF8.GetBytes(response));
                        }
                        //Here we send the jackpot to the winner
                        channel.BasicPublish("", strlist[0], null, Encoding.UTF8.GetBytes(value.ToString()));

                        //Here we send the user info to our localDb
                        var toDoItem = new Item
                        {
                            UserId = strlist[0],
                            JackpotValue = testValue,
                            Time = DateTime.Now.ToString()
                        };

                        // Its added to the database
                        await _dbContext.AddAsync(toDoItem);
                        await _dbContext.SaveChangesAsync();

                        Console.WriteLine($"Item added to the LocalDb: {toDoItem.UserId} | {toDoItem.JackpotValue}");


                    }
                    clientDictionary[strlist[0]] += numVal;
                    Console.WriteLine("Client: " + strlist[0] + " now has:" + clientDictionary[strlist[0]] + " point for the jackpot request");
                    strlist[0] = string.Empty;
                    strlist[1] = string.Empty;
                }


            };
            channel.BasicConsume("jackpot", true, server);
            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            connection.Close();
        }


    }

}