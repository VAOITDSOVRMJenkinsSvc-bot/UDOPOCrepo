using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    [DebuggerStepThrough]
    [DataContract]
    [Serializable]
    public class UdoEcMessageBase : UdoEcIMessageBase
    {
        [NonSerialized]
        private ExtensionDataObject _ExtensionData;

        protected UdoEcMessageBase()
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
