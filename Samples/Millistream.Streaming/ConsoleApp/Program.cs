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
message.Add(0, MessageReferences.MDF_M_LOGON);
message.AddString(Fields.MDF_F_USERNAME, "sandbox");
message.AddString(Fields.MDF_F_PASSWORD, "sandbox");
mdf.Send(message);
message.Reset();

//5. Consume and wait for the server to send a MDF_M_LOGONGREETING message.
if (!Consume(mdf, MessageReferences.MDF_M_LOGONGREETING))
{
    Console.WriteLine("Failed to connect to the API.");
    return;
}
Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - Logged in");

//6. Register a data callback (optional).
mdf.DataCallback = (data, handle) =>
{
    while (handle.GetNextMessage(out ushort mref, out ulong insref))
    {
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - " +
            "Received a message with the following fields:");

        while (handle.GetNextField(out uint tag, out ReadOnlySpan<byte> value))
        {
#if NETFRAMEWORK
            Console.WriteLine($"Field: {tag}, Value: {Encoding.UTF8.GetString(value.ToArray())}");
#else
            Console.WriteLine($"Field: {tag}, Value: {Encoding.UTF8.GetString(value)}");
#endif
        }
    }
};

//7. Request some data.
message.Add(0, MessageReferences.MDF_M_REQUEST);
message.AddList(Fields.MDF_F_REQUESTCLASS, // <- What kind of data to request.
    RequestClasses.MDF_RC_BASICDATA + " " + // <- Basic data
    RequestClasses.MDF_RC_QUOTE); // < ...and quotes in this case.
message.AddNumeric(
    Fields.MDF_F_REQUESTTYPE, // <- The type of request.
    RequestTypes.MDF_RT_FULL // <- Full (image+streaming) in this case.
);
message.AddList(
    Fields.MDF_F_INSREFLIST, // <- What instrument identifier(s) the request is for.
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
message.Add(0, MessageReferences.MDF_M_LOGOFF);
mdf.Send(message);
if (Consume(mdf, MessageReferences.MDF_M_LOGOFF))
    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} - Logged out");

// 10. Disconnect.
mdf.Disconnect();

// 11. Explictly or implictly dispose the managed handles.

static bool Consume(MarketDataFeed mdf, ushort messageReference)
{
    DateTime time = DateTime.UtcNow;
    do
    {
        int ret = mdf.Consume(1);
        switch (ret)
        {
            case 1:
                while (mdf.GetNextMessage(out ushort mref, out ulong _))
                    if (mref == messageReference)
                        return true;
                break;
            case -1:
                return false;
        }

    } while (DateTime.UtcNow.Subtract(time).TotalSeconds < 10);
    return false;
}