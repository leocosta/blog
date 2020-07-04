using StackExchange.Redis;
using System;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Publisher");
            Console.WriteLine();

            var subscriber = CreateSubscriber();

            do
            {
                Console.Write("Type a message: ");

                var value = Console.ReadLine();

                if (value.ToLower() == "exit")
                {
                    break;
                }

                subscriber.Publish("MyFirstTopic", value);

            } while (true);
        }

        private static ISubscriber CreateSubscriber()
        {
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = {
                    { "localhost", 6379 }
                },
            });

            var subscriber = redis.GetSubscriber();

            return subscriber;
        }
    }
}
