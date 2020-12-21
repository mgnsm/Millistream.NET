using System.Collections.Generic;
using System.Linq;

namespace Millistream.Streaming
{
    /// <summary>
    /// The request message used to unsubscribe from streaming data from the server.
    /// </summary>
    public class UnsubscribeMessage : RequestMessage
    {
        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="UnsubscribeMessage"/> class.
        /// </summary>
        public UnsubscribeMessage() 
            : base(MessageReference.MDF_M_UNSUBSCRIBE) 
        { }

        /// <summary>
        /// Creates an instance of the <see cref="UnsubscribeMessage"/> class.
        /// </summary>
        /// <param name="requestClasses">An optional enumerable sequence of the request classes to be unsubscribed from.</param>
        public UnsubscribeMessage(IEnumerable<RequestClass> requestClasses) : base(MessageReference.MDF_M_UNSUBSCRIBE) =>
            RequestClasses = requestClasses;
        #endregion

        #region Properties
        /// <summary>
        ///  An enumerable sequence of the request classes to be requested. If the sequence is empty, the request will be for all request classes available.
        /// </summary>
        public IEnumerable<RequestClass> RequestClasses { get; set; }

        /// <summary>
        /// The optional id of the request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///  An enumerable sequence of instrument references for the request. If the sequence is empty, the request will be for all instruments available. 
        /// </summary>
        public IEnumerable<ulong> InstrumentReferences { get; set; }
        #endregion

        #region Methods
        internal override void AddFields(Message message)
        {
            message.Add(0, MessageReference);
            if (RequestClasses != null && RequestClasses.Any())
                message.AddList(RequestClasses);
            else
                message.AddString(Field.MDF_F_REQUESTCLASS, "*");
            if (!string.IsNullOrEmpty(RequestId))
                message.AddString(Field.MDF_F_REQUESTID, RequestId);
            if (InstrumentReferences != null && InstrumentReferences.Any())
                message.AddList(Field.MDF_F_INSREFLIST, InstrumentReferences);
        }
        #endregion
    }
}
