# Millistream.NET

An unofficial .NET wrapper for Millistream's low-latency, high-throughput and high-availability C/C++ streaming API that can be used to subscribe to streaming real-time or delayed financial data.

| Package | Build Status | NuGet  |
| :------------ |-------------| -------------|
| Millistream.Streaming | ![Build Status Badge](https://bit.ly/3eeOGKP) | [![NuGet Badge](https://img.shields.io/nuget/v/Millistream.Streaming.svg)](http://www.nuget.org/packages/Millistream.Streaming/) |
| Millistream.Streaming.DataTypes | ![Build Status Badge](https://bit.ly/2yyIxds) | [![NuGet Badge](https://img.shields.io/nuget/v/Millistream.Streaming.DataTypes.svg)](http://www.nuget.org/packages/Millistream.Streaming.DataTypes/) |

## Installation
Millistream.NET is distributed via [NuGet](https://www.nuget.org/packages/Millistream.Streaming). The native and wrapped API can be downloaded from [Millistream's official website](https://packages.millistream.com/). Binaries are available for Linux, macOS and Windows. They come in both 32- and 64-bit versions and should work on both little- and big-endian systems. Please refer to [the official documentation](https://bit.ly/2LOYjkT) for more information about the wrapped API itself. The NuGet package does not include any native assemblies. You will have to download and install these separately.
 
On **Windows** you download and run an [.exe](https://packages.millistream.com/Windows/libmdf-1.0.23.exe) that will install the native `libmdf` core library along pre-built binaries of [zlib](http://zlib.net) and [OpenSSL](http://openssl.org/) (`libmdf` links against these) to `%windir%\System32`. You can do this silently from a command prompt using Powershell:

    powershell (new-object System.Net.WebClient).DownloadFile('https://packages.millistream.com/Windows/libmdf-1.0.23.exe', 'libmdf-1.0.23.exe')
    .\libmdf-1.0.23.exe /S

On **macOS** you download and install a `.pkg` file, for example in a Bash shell:

    curl -O https://packages.millistream.com/macOS/libmdf-1.0.21.pkg 
    sudo installer -pkg libmdf-1.0.21.pkg -target /

On **Linux**, the native API and the dependent libraries are available through your distribution repository. Below is an example of how to install everything needed using the `apt-get` command-line tool on Ubuntu:

    sudo wget "https://packages.millistream.com/apt/sources.list.d/`lsb_release -cs`.list" -O /etc/apt/sources.list.d/millistream.list 
    wget -q "https://packages.millistream.com/D2FCCE35.gpg" -O- | sudo apt-key add - 
    sudo apt update
    sudo apt-get install libmdf

Instructions on how to install the API on other supported distributions can be found on the [FTP server](https://bit.ly/2wD2omK). You may also want to take a look at the [YAML build pipeline](https://github.com/mgnsm/Millistream.NET/blob/master/Build/Millistream.Streaming/ci-pipeline.yml) in this repository. It installs the native binaries and runs integration tests against them on macOS, Ubuntu and Windows using the Microsoft-hosted agents in Azure Pipelines.
## Basic example
Once you have installed the native `libmdf` library on your computer, you can then [install](https://docs.microsoft.com/en-us/nuget/consume-packages/ways-to-install-a-package) Millistream.NET into your project using NuGet and use it in your application. Below is a basic example of how to use the .NET API to connect to a server and subscribe to some streaming data:

```cs
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

            //1. Create an instance of the DataFeed class.
            using (DataFeed dataFeed = new DataFeed())
            {
                //2. Hook up an event handler to the ConnectionStatusChanged event.
                dataFeed.ConnectionStatusChanged += OnConnectionStatusChanged;
                //3. Subscribe to the Data observable.
                dataFeed.Data.Subscribe(new Observer(dataFeed));
                //4. Call the Connect method to connect to the feed and authenticate.
                if (dataFeed.Connect(Host, Username, Password))
                {
                    //5. Issue a subscription request and wait for data.
                    dataFeed.Request(new SubscribeMessage(
                        RequestType.MDF_RT_FULL, // <- The type of request. Full (image+streaming) in this case.
                        new RequestClass[1] { RequestClass.MDF_RC_QUOTE }) //<- What kind of data to request. 
                                                                           //   Quotes in this case.
                    {
                        InstrumentReferences = new ulong[1] { 772 }, //<- What instrument identifier(s) the 
                                                                     //   request is for. 772 is the unique 
                                                                     //   identifier for Ericsson B on Nasdaq OMX 
                                                                     //   Stockholm.
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

    class Observer : IObserver<ResponseMessage>
    {
        private readonly IDataFeed _dataFeed;

        public Observer(IDataFeed dataFeed) => _dataFeed = dataFeed ?? 
            throw new ArgumentNullException(nameof(dataFeed));

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
            _dataFeed.Recycle(message);
        }

        public void OnCompleted() { }

        public void OnError(Exception exception) => Console.WriteLine(exception.Message);
    }
}
```
## Data Types
There is a separate [Millistream.Streaming.DataTypes NuGet package](http://www.nuget.org/packages/Millistream.Streaming.DataTypes/) that contains managed implementations of all the data types (`String`, `Time`, `Date`, `InsRef`, `List`, `Tabular`, `Number`, `Bool`, `UInt` and `BitField`) that are used in the native streaming API. All data types are implemented as immutable value types and declared as [readonly structs](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/struct#readonly-struct).

Unlike `Millistream.Streaming`, this package targets .NET Standard 2.1. The reason for this is that it internally uses some of the built-in .NET types' `TryParse` overloads that are only available in the .NET Core 3.x runtime. 

`Millistream.Streaming` still targets .NET Standard 1.2 and .NET Framework 4.5 and has no dependency on `Millistream.Streaming.DataTypes`.

Below is an example of how to parse the value of an `MDF_F_LASTPRICE` field that is included in a `ResponseMessage` to a `Number`:

```cs
Number? lastPrice;
IReadOnlyDictionary<Field, ReadOnlyMemory<byte>> fields = responseMessage.Fields;
if (fields.TryGetValue(Field.MDF_F_LASTPRICE, out ReadOnlyMemory<byte> value)
    && Number.TryParse(value.Span, out Number number))
{
    lastPrice = number;
}
```

Each data type contains `Parse` and `TryParse` methods that accept either a `ReadOnlySpan<char>` or a `ReadOnlySpan<byte>`. Just like with the built-in types, `Parse` throws an `ArgumentException` if the conversion fails and `TryParse` returns `false`.
