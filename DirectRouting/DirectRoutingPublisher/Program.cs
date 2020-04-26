using MessageData;
using RabbitMQ.Client;
using System;

namespace DirectRoutingPublisher
{
    class Program
    {
        private const string Exchange = "DirectExchange";
        private const string BookOrderQueueName = "BookOrderDirectRoutingQueue";
        private const string PaymentQueueName = "PaymentDirectRotingQueueName";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, "direct");
                    channel.QueueDeclare(BookOrderQueueName, true, false, false);
                    channel.QueueDeclare(PaymentQueueName, true, false, false);

                    channel.QueueBind(BookOrderQueueName, Exchange, "BookOrder");
                    channel.QueueBind(PaymentQueueName, Exchange, "Payment");


                    int defaultMessageCount = 10;
                    Console.WriteLine("Please enter message count for publish:");
                    var enteredMessageCount = Console.ReadLine();
                    Int32.TryParse(enteredMessageCount, out defaultMessageCount);

                    for (int i = 0; i < defaultMessageCount; i++)
                    {
                        var payment = new Payment { Amount = 2000, CreditCardNumber = "444666888" };
                        channel.BasicPublish(Exchange, "Payment", null, payment.SerializeObject());
                        Console.WriteLine($"Payment Sent Amount : {payment.Amount} Credit Card Number : {payment.CreditCardNumber}");

                        var bookOrder = new BookOrder { Amount = 3000, OrderDate = DateTime.Now, OrderNumber = Guid.NewGuid().ToString() };
                        channel.BasicPublish(Exchange, "BookOrder", null, bookOrder.SerializeObject());
                        Console.WriteLine($"Book Order Sent Amount : {bookOrder.Amount} Date : {bookOrder.OrderDate} Order Number : {bookOrder.OrderNumber}");
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
