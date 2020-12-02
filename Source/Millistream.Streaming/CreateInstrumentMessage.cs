namespace Millistream.Streaming
{
    /// <summary>
    /// The request message that the client uses to request new instrument references.
    /// </summary>
    public class CreateInstrumentMessage : RequestMessage
    {
        #region Fields
        private static readonly RequestClass[] s_requestClasses = new RequestClass[1] { RequestClass.MDF_RC_INSREF };
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="CreateInstrumentMessage"/> class.
        /// </summary>
        /// <param name="count">The number of new instrument references to be created.</param>
        public CreateInstrumentMessage(uint count)
            : base(MessageReference.MDF_M_REQUEST)
        {
            Count = count;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number of new instrument references to be created.
        /// </summary>
        public uint Count { get; }
        
        /// <summary>
        /// The request class to be requested.
        /// </summary>
        public RequestClass RequestClass => RequestClass.MDF_RC_INSREF;

        /// <summary>
        /// The optional id of the request.
        /// </summary>
        public string RequestId { get; set; }
        #endregion

        #region Methods
        internal override void AddFields(IMessage message)
        {
            message.Add(0, MessageReference);
            message.AddRequestClasses(s_requestClasses);
            message.AddUInt32(Field.MDF_F_REQUESTTYPE, Count);
            if (!string.IsNullOrEmpty(RequestId))
                message.AddString(Field.MDF_F_REQUESTID, RequestId);
        }
        #endregion
    }
}