using System;
using Millistream.Streaming;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            const string Host = "HOST";
            const string Username = "YOUR_USERNAME";
            const string Password = "YOUR_PASSWORD";

            //1. Create an instance of the DataFeed class
            using (DataFeed dataFeed = new DataFeed())
            {
                //2. Hook up an event handler to the ConnectionStatusChanged event.
                dataFeed.ConnectionStatusChanged += OnConnectionStatusChanged;
                //3. Subscribe to the Data observable
                dataFeed.Data.Subscribe(new Observer(dataFeed));
                //4. Call the Connect method to connect to the feed and authenticate
                if (dataFeed.Connect(Host, Username, Password))
                {
                    //5. Issue a subscription request and wait for the DataReceived event to get raised
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

        static void OnConnectionStatusChanged(object sender,
            ConnectionStatusChangedEventArgs e) =>
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
                $"Connection Status: {e.ConnectionStatus}");
    }
}