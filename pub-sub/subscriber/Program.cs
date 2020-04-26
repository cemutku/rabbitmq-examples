using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace subscriber
{
    class Program
    {
        private const string Exchange = "BookOrderExchange";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, ExchangeType.Fanout);

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queueName, Exchange, string.Empty);

                    Console.WriteLine("Waiting for book orders...");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += BookOrderReceived;
                    channel.BasicConsume(queueName, true, consumer);

                    Console.ReadLine();
                }
            }
        }

        private static void BookOrderReceived(object sender, BasicDeliverEventArgs e)
        {
            var receivedBookOrder = (BookOrder)e.Body.DeserializeObject(typeof(BookOrder));
            Console.WriteLine($"Book Order Received Amount : {receivedBookOrder.Amount} Date : {receivedBookOrder.OrderDate} Order Number : {receivedBookOrder.OrderNumber}");
        }
    }
}
