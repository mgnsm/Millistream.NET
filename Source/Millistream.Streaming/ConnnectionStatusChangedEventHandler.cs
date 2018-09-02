namespace Millistream.Streaming
{
    /// <summary>
    /// Represents the method that will handle an event which provides connection status data.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the connection status data.</param>
    public delegate void ConnnectionStatusChangedEventHandler(object sender, ConnectionStatusChangedEventArgs e);
}
