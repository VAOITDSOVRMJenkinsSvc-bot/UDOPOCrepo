using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.VBMS.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOVBMSUploadDocumentAsyncRequest)]
    [DataContract]
    public class UDOVBMSUploadDocumentAsyncRequest : UDOVBMSUploadDocumentRequest
    {

    }
}
