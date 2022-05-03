﻿namespace Millistream.Streaming
{
    internal enum MDF_OPTION
    {
		MDF_OPT_FD,
		MDF_OPT_ERROR,
		MDF_OPT_RCV_BYTES,
		MDF_OPT_SENT_BYTES,
		MDF_OPT_DATA_CALLBACK_FUNCTION,
		MDF_OPT_DATA_CALLBACK_USERDATA,
		MDF_OPT_STATUS_CALLBACK_FUNCTION,
		MDF_OPT_STATUS_CALLBACK_USERDATA,
		MDF_OPT_CONNECT_TIMEOUT,
		MDF_OPT_HEARTBEAT_INTERVAL,
		MDF_OPT_HEARTBEAT_MAX_MISSED,
		MDF_OPT_TCP_NODELAY,
		MDF_OPT_NO_ENCRYPTION,
		MDF_OPT_TIME_DIFFERENCE,
		MDF_OPT_BIND_ADDRESS,
		MDF_OPT_TIME_DIFFERENCE_NS,
		MDF_OPT_CRYPT_DIGESTS,
		MDF_OPT_CRYPT_CIPHERS,
		MDF_OPT_CRYPT_DIGEST,
		MDF_OPT_CRYPT_CIPHER,
		MDF_OPT_TIMEOUT
	};
}