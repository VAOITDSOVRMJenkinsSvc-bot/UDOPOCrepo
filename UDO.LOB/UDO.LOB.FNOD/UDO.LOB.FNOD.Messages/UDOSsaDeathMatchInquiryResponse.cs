using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.FNOD.Messages
{
    [Export(typeof(IMessageBase))]
    [DataContract]
    public class UDOSsaDeathMatchInquiryResponse : MessageBase
    {
        [DataMember]
        public bool DateOfDeath;
        [DataMember]
        public string SoapRequestString;
        [DataMember]
        public string SoapResponseString;
        [DataMember]
        public UDOSsaDeathMatchInquiryException SsaDeathMatchException;

        public UDOSsaDeathMatchInquiryResponse()
        {
            DateOfDeath = false;
            SsaDeathMatchException = new UDOSsaDeathMatchInquiryException();
        }

        public UDOSsaDeathMatchInquiryResponse(bool dod, UDOSsaDeathMatchInquiryException exc)
        {
            DateOfDeath = dod;
            SsaDeathMatchException = exc;
        }
    }

    [DataContract]
    public class UDOSsaDeathMatchInquiryException
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }

        public UDOSsaDeathMatchInquiryException()
        {
            ExceptionOccurred = false;
            ExceptionMessage = string.Empty;
        }

        public UDOSsaDeathMatchInquiryException(bool excOccurred, string excMessage)
        {
            ExceptionOccurred = excOccurred;
            ExceptionMessage = excMessage;
        }

        public void SetValues(bool excOccurred, string excMessage)
        {
            ExceptionOccurred = excOccurred;
            ExceptionMessage = excMessage;
        }
    }
}
