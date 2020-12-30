namespace Millistream.Streaming
{
    /// <summary>
    /// Connection Statuses
    /// </summary>
    public enum ConnectionStatus
    {
		MDF_STATUS_LOOKUP,
		MDF_STATUS_CONNECTING,
		MDF_STATUS_CONNECTED,
		MDF_STATUS_DISCONNECTED,
		MDF_STATUS_READYTOLOGON,
		MDF_STATUS_SND_HB_REQ,
		MDF_STATUS_RCV_HB_REQ,
		MDF_STATUS_RCV_HB_RES
	};
}