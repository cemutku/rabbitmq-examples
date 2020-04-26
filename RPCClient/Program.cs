using MessageData;
using System;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var rpcClient = new RpcClient();

            Console.WriteLine("Sending Payment...");

            var response = rpcClient.SendPayment();

            Console.WriteLine($"Received Payment Code : {response}");

            rpcClient.Close();

            Console.ReadLine();
        }
    }
}
