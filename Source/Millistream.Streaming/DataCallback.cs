namespace Millistream.Streaming
{
    /// <summary>
    /// Encapsulates a data callback method that will be called when there are any messages to decode.
    /// </summary>
    /// <typeparam name="TCallbackUserData">The type of the custom user data that will be available to the data callback method.</typeparam>
    /// <typeparam name="TStatusCallbackUserData">The type of the custom user data that will be available to the status callback method of the <paramref name="handle"/>.</typeparam>
    /// <param name="userData">The custom user data that will be available to the data callback method.</param>
    /// <param name="handle">A reference to the managed API handle that invoked the callback.</param>
    public delegate void DataCallback<TCallbackUserData, TStatusCallbackUserData>(
        TCallbackUserData userData, MarketDataFeed<TCallbackUserData, TStatusCallbackUserData> handle);
}