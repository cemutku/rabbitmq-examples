using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace TopixExchangeBookOrderSubscriber
{
    class Program
    {
        private const string Exchange = "TopicExchange";
        private const string BookOrderQueueName = "BookOrderTopicQueue";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("Waiting for topic <sample.bookorder> ...");

                    channel.ExchangeDeclare(Exchange, "topic");
                    channel.QueueDeclare(BookOrderQueueName, true, false, false);
                    channel.QueueBind(BookOrderQueueName, Exchange, "sample.bookorder");
                    channel.BasicQos(0, 5, false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += BookOrderReceived;

                    channel.BasicConsume(BookOrderQueueName, true, consumer);

                    Console.ReadLine();
                }
            }
        }

        private static void BookOrderReceived(object sender, BasicDeliverEventArgs e)
        {
            var receivedBookOrder = (BookOrder)e.Body.DeserializeObject(typeof(BookOrder));
            var routingKey = e.RoutingKey;
            Console.WriteLine($"Received RoutingKey : {routingKey} Amount : {receivedBookOrder.Amount} Date : {receivedBookOrder.OrderDate} Order Number : {receivedBookOrder.OrderNumber}");
        }
    }
}
