using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace TopicExchangeSubscriber
{
    class Program
    {
        private const string Exchange = "TopicExchange";
        private const string AllMessageQueue = "AllMessageTopicQueue";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("Waiting for topic <sample.*> ...");

                    channel.ExchangeDeclare(Exchange, "topic");
                    channel.QueueDeclare(AllMessageQueue, true, false, false);
                    channel.QueueBind(AllMessageQueue, Exchange, "sample.*");
                    channel.BasicQos(0, 5, false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += MessageReceived;

                    channel.BasicConsume(AllMessageQueue, true, consumer);

                    Console.ReadLine();
                }
            }
        }

        private static void MessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var receivedMessage = e.Body.DeserializeText();
            var routingKey = e.RoutingKey;
            Console.WriteLine($"Message Received RoutingKey : {routingKey} Message : {receivedMessage}");
        }
    }
}
