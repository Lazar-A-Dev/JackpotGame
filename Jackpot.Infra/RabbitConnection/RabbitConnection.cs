using RabbitMQ.Client;
using System;

namespace Jackpot.Infra.RabbitConnection
{
    public class RabbitConnection
    {
        public IConnection Connection { get; private set; }
        public IModel Channel { get; private set; }

        public RabbitConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            this.Connection = factory.CreateConnection();
            this.Channel = this.Connection.CreateModel();
        }
    }
}

