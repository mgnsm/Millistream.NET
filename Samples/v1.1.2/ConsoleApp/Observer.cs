using Millistream.Streaming;
using System;
using System.Text;

namespace ConsoleApp
{
    class Observer : IObserver<ResponseMessage>
    {
        private readonly IDataFeed _dataFeed;

        public Observer(IDataFeed dataFeed) => _dataFeed = dataFeed;

        public void OnNext(ResponseMessage message)
        {
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
                $"Received a {message.MessageReference} message with the following fields:");
            foreach (var field in message.Fields)
#if NET_CORE
                Console.WriteLine($"{field.Key}: {Encoding.UTF8.GetString(field.Value.Span)}");
#else
                Console.WriteLine($"{field.Key}: {Encoding.UTF8.GetString(field.Value.ToArray())}");
#endif
            _dataFeed?.Recycle(message);
        }

        public void OnCompleted() { }

        public void OnError(Exception exception) => Console.WriteLine(exception.Message);
    }
}