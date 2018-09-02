namespace Millistream.Streaming
{
    /// <summary>
    /// Represents the method that will handle an event which provides requested data.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the received data.</param>
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
}
