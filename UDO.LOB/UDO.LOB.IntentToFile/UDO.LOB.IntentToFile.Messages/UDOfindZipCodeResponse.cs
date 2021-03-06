using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;


//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.IntentToFile.Messages
{
    [Export(typeof(IMessageBase))]
    // TODO: Remove ExportMetadata not sure it is needed
    // [ExportMetadata("MessageType", MessageRegistry.UDOfindZipCodeResponse)]
    [DataContract]
    public class UDOfindZipCodeResponse : MessageBase
    {
		[DataMember]
		public ValidateAddressMultipleResponse[] ValidatedAddresses { get; set; }
		[DataMember]
		public bool ExceptionOccurred { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }
	}
	[DataContract]
	public class ValidateAddressMultipleResponse
	{
        [DataMember]
        public string cityType { get; set; }
        [DataMember]
        public string postalCode { get; set; }
        [DataMember]
        public string processedBy { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string statusCode { get; set; }
        [DataMember]
        public string statusDescription { get; set; }
    }
}
