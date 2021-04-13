# Millistream.NET

An unofficial .NET wrapper for Millistream's low-latency, high-throughput and high-availability C/C++ streaming API that can be used to subscribe to streaming real-time or delayed financial data.

| Package | Build Status | NuGet  |
| :------------ |-------------| -------------|
| Millistream.Streaming | [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mgnsm/Millistream.NET/Millistream.Streaming?label=GitHub%20Actions&logo=GitHub)](https://github.com/mgnsm/Millistream.NET/actions?query=workflow%3AMillistream.Streaming) | [![NuGet Badge](https://img.shields.io/nuget/v/Millistream.Streaming.svg)](http://www.nuget.org/packages/Millistream.Streaming/) |
| Millistream.Streaming.DataTypes | [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mgnsm/Millistream.NET/Millistream.Streaming.DataTypes?label=GitHub%20Actions&logo=GitHub)](https://github.com/mgnsm/Millistream.NET/actions?query=workflow%3AMillistream.Streaming.DataTypes) | [![NuGet Badge](https://img.shields.io/nuget/v/Millistream.Streaming.DataTypes.svg)](http://www.nuget.org/packages/Millistream.Streaming.DataTypes/) |

## Installation
Millistream.NET is distributed via [NuGet](https://www.nuget.org/packages/Millistream.Streaming). The native and wrapped API can be downloaded from [Millistream's official website](https://packages.millistream.com/). Binaries are available for Linux, macOS and Windows. They come in both 32- and 64-bit versions and should work on both little- and big-endian systems. Please refer to [the official documentation](https://bit.ly/2LOYjkT) for more information about the wrapped API itself. The NuGet package does not include any native assemblies. You will have to download and install these separately.
### Native Dependency
On **Windows** you download and run an [.exe](https://packages.millistream.com/Windows/libmdf-1.0.23.exe) that will install the native `libmdf` core library along pre-built binaries of [zlib](http://zlib.net) and [OpenSSL](http://openssl.org/) (`libmdf` links against these) to `%windir%\System32`. You can do this silently from a command prompt using Powershell:

    powershell (new-object System.Net.WebClient).DownloadFile('https://packages.millistream.com/Windows/libmdf-1.0.23.exe', 'libmdf-1.0.23.exe')
    .\libmdf-1.0.23.exe /S

On **macOS** you download and install a `.pkg` file, for example in a Bash shell:

    curl -O https://packages.millistream.com/macOS/libmdf-1.0.23.pkg 
    sudo installer -pkg libmdf-1.0.23.pkg -target /

On **Linux**, the native API and the dependent libraries are available through your distribution repository. Below is an example of how to install everything needed using the `apt-get` command-line tool on Ubuntu:

    sudo wget "https://packages.millistream.com/apt/sources.list.d/`lsb_release -cs`.list" -O /etc/apt/sources.list.d/millistream.list 
    wget -q "https://packages.millistream.com/D2FCCE35.gpg" -O- | sudo apt-key add - 
    sudo apt update
    sudo apt-get install libmdf

Instructions on how to install the API on other supported distributions can be found on the [FTP server](https://bit.ly/2wD2omK). You may also want to take a look at the [YAML build pipeline](.github/workflows/millistream.streaming.yml) in this repository. It installs the native binaries and runs integration tests against them on macOS, Ubuntu and Windows using the cloud-hosted runners in GitHub Actions.
## Getting Started
Once you have installed the native `libmdf` library on your computer, you can then [install](https://docs.microsoft.com/en-us/nuget/consume-packages/ways-to-install-a-package) Millistream.NET into your project using NuGet:

    PM> Install-Package Millistream.Streaming

Below is a basic example of how to use the .NET API to connect to a server and subscribe to some streaming data:

```cs
using System;
using System.Text;
using Millistream.Streaming;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

//1. Initialize the managed API and message handles.
using MarketDataFeed mdf = new();
using Message message = new();

//2. Register a connection status callback (optional).
mdf.StatusCallback = (data, status, host, ip) =>
    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
        $"Connection Status: {status}");

//3. Connect.
mdf.Connect("sandbox.millistream.com:9100");

//4. Send a MDF_M_LOGON message to log on.
message.Add(0, MessageReference.MDF_M_LOGON);
message.AddString(Field.MDF_F_USERNAME, "sandbox");
message.AddString(Field.MDF_F_PASSWORD, "sandbox");
mdf.Send(message);
message.Reset();

//5. Consume and wait for the server to send a MDF_M_LOGONGREETING message.
if (!Consume(mdf, MessageReference.MDF_M_LOGONGREETING))
{
    Console.WriteLine("Failed to connect to the API.");
    return;
}
Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - Logged in");

//6. Register a data callback (optional).
mdf.DataCallback = (data, handle) =>
{
    while (handle.GetNextMessage(out MessageReference mref, out MessageClasses mclass,
        out ulong insref))
    {
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
            $"Received an {mref} message with the following fields:");

        while (handle.GetNextField(out Field field, out ReadOnlySpan<byte> value))
        {
#if NETCOREAPP
            Console.WriteLine($"{field}: {Encoding.UTF8.GetString(value)}");
#else
            Console.WriteLine($"{field}: {Encoding.UTF8.GetString(value.ToArray())}");
#endif
        }
    }
};

//7. Request some data.
message.Add(0, MessageReference.MDF_M_REQUEST);
message.AddList(Field.MDF_F_REQUESTCLASS, // <- What kind of data to request.
    StringConstants.RequestClasses.MDF_RC_BASICDATA + " " + // <- Basic data
    StringConstants.RequestClasses.MDF_RC_QUOTE); // < ...and quotes in this case.
message.AddNumeric(
    Field.MDF_F_REQUESTTYPE, // <- The type of request.
    StringConstants.RequestTypes.MDF_RT_FULL // <- Full (image+streaming) in this case.
);
message.AddList(
    Field.MDF_F_INSREFLIST, // <- What instrument identifier(s) the request is for.
    "772"); // <- 772 is the unique identifier for Ericsson B.
mdf.Send(message);
message.Reset();

// 8. Consume until a key is pressed.
// NOTE: If you don't register a data callback by setting the DataCallback property, 
// you should call the GetNextMessage and GetNextField methods after calling Consume
// below.
while (!Console.KeyAvailable)
{
    if (mdf.Consume(1) == -1)
        break;
}

// 9. Log off by sending an MDF_M_LOGOFF message (optional).
mdf.DataCallback = null; // Unregister the data callback before logging out.
message.Add(0, MessageReference.MDF_M_LOGOFF);
mdf.Send(message);
if (Consume(mdf, MessageReference.MDF_M_LOGOFF))
    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - Logged out");

// 10. Disconnect.
mdf.Disconnect();

// 11. Explictly or implictly dispose the managed handles.

static bool Consume(MarketDataFeed mdf, MessageReference messageReference)
{
    DateTime time = DateTime.UtcNow;
    do
    {
        int ret = mdf.Consume(1);
        switch (ret)
        {
            case 1:
                while (mdf.GetNextMessage(out MessageReference mref, out MessageClasses _,
                    out ulong _))
                    if (mref == messageReference)
                        return true;
                break;
            case -1:
                return false;
        }

    } while (DateTime.UtcNow.Subtract(time).TotalSeconds < 10);
    return false;
}
```
## Data Types
There is a separate [Millistream.Streaming.DataTypes NuGet package](http://www.nuget.org/packages/Millistream.Streaming.DataTypes/) that contains managed implementations of all the data types (`String`, `Time`, `Date`, `InsRef`, `List`, `Tabular`, `Number`, `Bool`, `UInt` and `BitField`) that are used in the native streaming API. All data types are implemented as immutable value types and declared as [readonly structs](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/struct#readonly-struct).

This package targets .NET Standard 2.1. The reason for this is that it internally uses some of the built-in .NET types' `TryParse` overloads that are only available in the .NET Core 3.x runtime. 

`Millistream.Streaming` targets .NET Standard 1.4 and .NET Framework 4.5 and has no dependency on `Millistream.Streaming.DataTypes`.

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