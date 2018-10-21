# Millistream.NET
![Build Status Badge](https://bit.ly/2ENttLt) [![NuGet Badge](https://img.shields.io/nuget/v/Millistream.Streaming.svg)](http://www.nuget.org/packages/Millistream.Streaming/)

An unofficial .NET Standard wrapper for Millistream's low-latency, high-throughput and high-availability C/C++ streaming API that can be used to subscribe to streaming real-time or delayed financial data.
## Installation
Millistream.NET is distributed via [NuGet](https://www.nuget.org/packages/Millistream.Streaming). The native and wrapped API can be downloaded from [Millistream's anonymous FTP server](https://bit.ly/2LOXHf5). Binaries are available for Windows, Linux and OS X. They come in both 32- and 64-bit versions and should work on both little- and big-endian systems. Please refer to [the official documentation](https://bit.ly/2LOYjkT) for more information about the wrapped API itself. The NuGet package does not include any native assemblies. You will have to download and install these separately.
 
On Windows you download and run an [.exe from the FTP server](https://bit.ly/2N96qh2) that will install the native `libmdf` core library along pre-built binaries of [zlib](http://zlib.net) and [OpenSSL](http://openssl.org/) (`libmdf` links against these) to `%windir%\System32`.
 
On Linux, the native API and the dependent libraries are available through your distribution repository. Below is an example of how to install everything needed using the `apt-get` command line tool on Ubuntu.

    sudo wget "https://packages.millistream.com/apt/sources.list.d/`lsb_release -cs`.list" -O /etc/apt/sources.list.d/millistream.list 
    wget -q "https://packages.millistream.com/D2FCCE35.gpg" -O- | sudo apt-key add - 
    sudo apt update
    sudo apt-get install libmdf

Instructions on how to install the API on other supported distributions can be found on the [FTP server](https://bit.ly/2wD2omK).
## Basic example
Once you have installed the native `libmdf` library on your computer, you can then [install](https://docs.microsoft.com/en-us/nuget/consume-packages/ways-to-install-a-package) Millistream.NET into your project using NuGet and use it in your application. Below is a basic example of how to use the .NET API to connect to a server and subscribe to some streaming data:

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
                    dataFeed.Connect(Host, Username, Password);
                    //4. Issue a subscription request and wait for the DataReceived event to get raised
                    dataFeed.Request(new SubscribeMessage(
                        RequestType.MDF_RT_FULL, // <- The type of request. Full (image+streaming) in this case.
                        new RequestClass[1] { RequestClass.MDF_RC_QUOTE }) //<- What kind of data to request. Quotes in this case.
                    {
                        InstrumentReferences = new ulong[1] { 772 }, //<- What instrument identifier(s) the request is for. 772 is the unique identifier for Ericsson B on Nasdaq OMX Stockholm
                    });

                    //prevent the app from terminating until you press a key
                    Console.ReadLine();
                }
            }

            private static void OnDataReceived(object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
                    $"Received a {e.Message.MessageReference} message with the following fields:");
                foreach (var field in e.Message.Fields)
                    Console.WriteLine($"{field.Key}: {field.Value}");
            }

            private static void OnConnectionStatusChanged(object sender,
                ConnectionStatusChangedEventArgs e) =>
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
                    $"Connection Status: {e.ConnectionStatus}");
        }
    }
