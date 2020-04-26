using MessageData;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace RPCClient
{
    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "123456" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            Console.WriteLine($"CorrelationId = {props.CorrelationId}");

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body.ToArray());

                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public string SendPayment()
        {
            Payment payment = new Payment() { Amount = 122323, CreditCardNumber = "239872323" };
            
            Console.WriteLine($"Amount : {payment.Amount} Credit Card Number : {payment.CreditCardNumber}");

            channel.BasicPublish("", "rpc_queue", props, payment.SerializeObject());
            channel.BasicConsume(consumer, replyQueueName, true);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }

}
