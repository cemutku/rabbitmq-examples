using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace DirectRoutingSubscriber
{
    class Program
    {
        private const string Exchange = "DirectExchange";
        private const string BookOrderQueueName = "BookOrderDirectRoutingQueue";
        
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, "direct");

                    channel.QueueDeclare(BookOrderQueueName, true, false, false);
                    channel.QueueBind(BookOrderQueueName, Exchange, "BookOrder");

                    // tells the RabbitMQ not to give more than one message to a worker at a time.
                    // Do not dispatch a new message to a worker until it has processed and acknowledged a previous one.
                    // Instead it will dispatch it to the next worker that is not busy
                    channel.BasicQos(0, 1, false);

                    Console.WriteLine("Waiting for book orders...");

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
