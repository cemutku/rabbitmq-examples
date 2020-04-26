using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Globalization;
using System.Text;

namespace RPCServer
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("rpc_queue", false, false, false, null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("rpc_queue", false, consumer);

                    Console.WriteLine("Waiting RPC requests");

                    consumer.Received += (model, ea) =>
                    {
                        string response = null;

                        var body = ea.Body;
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;

                        try
                        {
                            var payment = (Payment)ea.Body.DeserializeObject(typeof(Payment));
                            response = (new Random()).Next(1000, 100000000).ToString(CultureInfo.InvariantCulture);
                            Console.WriteLine($"Received Code : {response} Amount : {payment.Amount} Credit Card Number : {payment.CreditCardNumber}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            response = "";
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    };

                    Console.ReadLine();
                }
            }
        }
    }
}
