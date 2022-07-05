using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace VEIS.Core.Core
{
    [DebuggerStepThrough]
    [DataContract]
    [Serializable]
    public abstract class MessageBase : IMessageBase
    {
        [NonSerialized]
        private ExtensionDataObject _ExtensionData;

        protected MessageBase()
        {
            MessageId = MessageId = Guid.NewGuid().ToString();
        }
        [DataMemberAttribute(IsRequired = true, EmitDefaultValue = false)]
        public string MessageId { get; set; }
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return _ExtensionData;
            }
            set
            {
                _ExtensionData = value;
            }
        }
    }
}
