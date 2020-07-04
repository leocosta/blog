using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Subscriber
{
    class Program
    {
        private const string CacheKey = "Messages";

        private static readonly IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        static void Main(string[] args)
        {
            Console.WriteLine($"Subscribers");
            Console.WriteLine();
            
            CreateSubscriber()
                .Subscribe("MyFirstTopic", (channel, message) =>
                {
                    SaveMessage(message);
                    ClearConsole();
                    ShowMessages();
                });

            Console.ReadKey();
        }

        private static void SaveMessage(RedisValue message)
        {
            var messages = cache.GetOrCreate(CacheKey, i => new Dictionary<DateTime, string>());

            messages.Add(DateTime.Now, message);

            cache.Set(CacheKey, messages);
        }

        private static void ClearConsole()
        {
            Console.Clear();
        }

        private static void ShowMessages()
        {
            var messages = cache.Get<IDictionary<DateTime, string>>(CacheKey);
            foreach (var item in messages)
            {
                Console.WriteLine($"{ item.Key }: { item.Value }");
            }
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
