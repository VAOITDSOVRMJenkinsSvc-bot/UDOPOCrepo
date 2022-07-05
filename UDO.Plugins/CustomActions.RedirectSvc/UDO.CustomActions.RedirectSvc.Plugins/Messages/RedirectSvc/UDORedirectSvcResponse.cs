using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.RedirectSvc.Messages
{
    [Export(typeof(IMessageBase))]
    [DataContract]
    public class UDORedirectSvcResponse : MessageBase
    {
        [DataMember]
        public string MCSSOAPResponse;
        [DataMember]
        public string SoapRequestString;
        [DataMember]
        public string SoapResponseString;
        [DataMember]
        public UDORedirectSvcException RedirectSvcException;

        public UDORedirectSvcResponse()
        {
            MCSSOAPResponse = string.Empty;
            RedirectSvcException = new UDORedirectSvcException();
        }

        public UDORedirectSvcResponse(string soapresponse, UDORedirectSvcException exc)
        {
            MCSSOAPResponse = soapresponse;
            RedirectSvcException = exc;
        }
    }

    [DataContract]
    public class UDORedirectSvcException
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }

        public UDORedirectSvcException()
        {
            ExceptionOccurred = false;
            ExceptionMessage = string.Empty;
        }

        public UDORedirectSvcException(bool excOccurred, string excMessage)
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
