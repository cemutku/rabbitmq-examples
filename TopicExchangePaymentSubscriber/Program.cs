using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace TopicExchangePaymentSubscriber
{
    class Program
    {
        private const string Exchange = "TopicExchange";
        private const string PaymentQueueName = "PaymentTopicQueue";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("Waiting for topic <sample.payment> ...");

                    channel.ExchangeDeclare(Exchange, "topic");
                    channel.QueueDeclare(PaymentQueueName, true, false, false);
                    channel.QueueBind(PaymentQueueName, Exchange, "sample.payment");
                    channel.BasicQos(0, 5, false);

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
