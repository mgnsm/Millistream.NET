using System;
using Millistream.Streaming;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string Host = "HOST";
            const string Username = "YOUR_USERNAME";
            const string Password = "YOUR_PASSWORD";

            //1. Create an instance of the DataFeed class
            using (DataFeed dataFeed = new DataFeed())
            {
                //2. Hook up event handlers to the ConnectionStatusChanged and DataReceived events.
                dataFeed.ConnectionStatusChanged += OnConnectionStatusChanged;
                dataFeed.DataReceived += OnDataReceived;
                //3. Call the Connect method to connect to the feed and authenticate
                if(dataFeed.Connect(Host, Username, Password))
                {
                    //4. Issue a subscription request and wait for the DataReceived event to get raised
                    dataFeed.Request(new SubscribeMessage(
                        RequestType.MDF_RT_FULL, // <- The type of request. Full (image+streaming) in this case.
                        new RequestClass[1] { RequestClass.MDF_RC_QUOTE }) //<- What kind of data to request. Quotes in this case.
                    {
                        InstrumentReferences = new ulong[1] { 772 }, //<- What instrument identifier(s) the request is for. 772 is the unique identifier for Ericsson B on Nasdaq OMX Stockholm
                    });
                }
                else
                {
                    Console.WriteLine("Failed to connect to the data feed.");
                }

                //prevent the app from terminating until you press a key
                Console.ReadLine();
            }
        }

        static void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
                $"Received a {e.Message.MessageReference} message with the following fields:");
            foreach (var field in e.Message.Fields)
                Console.WriteLine($"{field.Key}: {field.Value}");
        }

        static void OnConnectionStatusChanged(object sender,
            ConnectionStatusChangedEventArgs e) =>
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
                $"Connection Status: {e.ConnectionStatus}");
    }
}