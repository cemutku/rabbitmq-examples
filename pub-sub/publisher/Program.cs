using MessageData;
using RabbitMQ.Client;
using System;

namespace publisher
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
                    int defaultMessageCount = 10;
                    Console.WriteLine("Please enter message count for publish:");
                    var enteredMessageCount = Console.ReadLine();
                    Int32.TryParse(enteredMessageCount, out defaultMessageCount);

                    for (int i = 0; i < defaultMessageCount; i++)
                    {
                        BookOrder bookOrder = new BookOrder();
                        bookOrder.Amount = 10000;
                        bookOrder.OrderDate = DateTime.Now;
                        bookOrder.OrderNumber = Guid.NewGuid().ToString();
                        var message = bookOrder.SerializeObject();

                        channel.BasicPublish(Exchange, string.Empty, null, message);

                        Console.WriteLine($"Book Order Sent Amount : {bookOrder.Amount} Date : {bookOrder.OrderDate} Order Number : {bookOrder.OrderNumber}");
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
