using MessageData;
using RabbitMQ.Client;
using System;

namespace TopicExchangePublisher
{
    class Program
    {
        private const string Exchange = "TopicExchange";
        private const string PaymentQueueName = "PaymentTopicQueue";
        private const string BookOrderQueueName = "BookOrderTopicQueue";
        private const string AllMessageQueue = "AllMessageTopicQueue";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, "topic");

                    channel.QueueDeclare(PaymentQueueName, true, false, false);
                    channel.QueueDeclare(BookOrderQueueName, true, false, false);
                    channel.QueueDeclare(AllMessageQueue, true, false, false);

                    // Receieve only payments
                    channel.QueueBind(PaymentQueueName, Exchange, "sample.payment");

                    // Receieve only book orders
                    channel.QueueBind(BookOrderQueueName, Exchange, "sample.bookorder");

                    // Receive all messages
                    channel.QueueBind(AllMessageQueue, Exchange, "sample.*");

                    int defaultMessageCount = 10;
                    Console.WriteLine("Please enter message count for publish (topic):");
                    var enteredMessageCount = Console.ReadLine();
                    Int32.TryParse(enteredMessageCount, out defaultMessageCount);

                    for (int i = 0; i < defaultMessageCount; i++)
                    {
                        var payment = new Payment { Amount = 2000, CreditCardNumber = "444666888" };
                        channel.BasicPublish(Exchange, "sample.payment", null, payment.SerializeObject());
                        Console.WriteLine($"Payment Sent (Topic : sample.payment) Amount : {payment.Amount} Credit Card Number : {payment.CreditCardNumber}");

                        Console.WriteLine("----------------------------------------------------------------------------------------------------------------");

                        var bookOrder = new BookOrder { Amount = 3000, OrderDate = DateTime.Now, OrderNumber = Guid.NewGuid().ToString() };
                        channel.BasicPublish(Exchange, "sample.bookorder", null, bookOrder.SerializeObject());
                        Console.WriteLine($"Book Order Sent (Topic : sample.bookorder) Amount : {bookOrder.Amount} Date : {bookOrder.OrderDate} Order Number : {bookOrder.OrderNumber}");
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
