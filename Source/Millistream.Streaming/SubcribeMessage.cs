using System;
using System.Collections.Generic;
using System.Linq;

namespace Millistream.Streaming
{
    /// <summary>
    /// The request message that the client uses to request data from the server.
    /// </summary>
    public class SubscribeMessage : RequestMessage
    {
        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="SubscribeMessage"/> class.
        /// </summary>
        /// <param name="requestType">The type of request to be sent.</param>
        public SubscribeMessage(RequestType requestType)
            : this(requestType, default) { }
        
        /// <summary>
        /// Creates an instance of the <see cref="SubscribeMessage"/> class.
        /// </summary>
        /// <param name="requestType">The type of request to be sent.</param>
        /// <param name="requestClasses">An optional enumerable sequence of the request classes to be requested.</param>
        public SubscribeMessage(RequestType requestType, IEnumerable<RequestClass> requestClasses)
            : base(MessageReference.MDF_M_REQUEST)
        {
            RequestType = requestType;
            RequestClasses = requestClasses;
        }
        #endregion

        #region Properties
        private IEnumerable<RequestClass> _requestClasses;
        /// <summary>
        ///  An enumerable sequence of the request classes to be requested. If the sequence is empty, the request will be for all request classes available. 
        /// </summary>
        public IEnumerable<RequestClass> RequestClasses
        {
            get => _requestClasses;
            set
            {
                if (value != null && value.Contains(RequestClass.MDF_RC_INSREF))
                    throw new ArgumentException($"Invalid request class {RequestClass.MDF_RC_INSREF}. The {nameof(CreateInstrumentMessage)} type must be used to request new instrument references to be created.");
                _requestClasses = value;
            }
        }

        /// <summary>
        /// The type of request to be sent.
        /// </summary>
        public RequestType RequestType { get; }

        /// <summary>
        /// The optional id of the request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///  An enumerable sequence of instrument references for the request. If the sequence is empty, the request will be for all instruments available. 
        /// </summary>
        public IEnumerable<ulong> InstrumentReferences { get; set; }

        /// <summary>
        /// Set this property to only receive data that has been updated since a specific timestamp.
        /// </summary>
        /// <remarks>
        /// All times are expressed in UTC.
        /// </remarks>
        public DateTime? UtcStartTime { get; set; }
        #endregion

        #region Methods
        internal override void AddFields(Message message)
        {
            message.Add(0, MessageReference);
            message.AddRequestClasses(RequestClasses?.ToArray());
            message.AddUInt32(Field.MDF_F_REQUESTTYPE, (uint)RequestType);
            if (!string.IsNullOrEmpty(RequestId))
                message.AddString(Field.MDF_F_REQUESTID, RequestId);
            if (InstrumentReferences != null && InstrumentReferences.Any())
                message.AddInstrumentReferences(Field.MDF_F_INSREFLIST, InstrumentReferences.ToArray());
            if (UtcStartTime.HasValue)
            {
                message.AddDate(Field.MDF_F_DATE, UtcStartTime.Value.Date);
                if (UtcStartTime.Value.TimeOfDay != default)
                    message.AddTime(Field.MDF_F_TIME, UtcStartTime.Value.TimeOfDay);
            }
        }
        #endregion
    }
}
