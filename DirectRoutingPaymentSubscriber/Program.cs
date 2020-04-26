using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace DirectRoutingPaymentSubscriber
{
    class Program
    {
        private const string Exchange = "DirectExchange";        
        private const string PaymentQueueName = "PaymentDirectRotingQueueName";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, "direct");
                    channel.QueueDeclare(PaymentQueueName, true, false, false);
                    channel.QueueBind(PaymentQueueName, Exchange, "Payment");
                    channel.BasicQos(0, 1, false);

                    Console.WriteLine("Waiting for payments...");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += PaymentReceived;

                    channel.BasicConsume(PaymentQueueName, true, consumer);

                    Console.ReadLine();
                }
            }
        }

        private static void PaymentReceived(object sender, BasicDeliverEventArgs e)
        {
            var receivedPayment = (Payment)e.Body.DeserializeObject(typeof(Payment));
            var routingKey = e.RoutingKey;
            Console.WriteLine($"Received RoutingKey : {routingKey} Amount : {receivedPayment.Amount} Credit Card Number : {receivedPayment.CreditCardNumber}");
        }
    }
}
