using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.Security;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.VeteranSnapShot.Messages
{
    [DataContract]
    public class UDOLoadVeteranSnapshotAsyncRequest : UDORequestBase
    {
        [DataMember]
        public Guid udo_veteransnapshotid { get; set; }
        [DataMember]
        public Guid udo_idproofid { get; set; }
        //[DataMember]
        //public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string udo_participantid { get; set; }
        [DataMember]
        public string udo_filenumber { get; set; }
        [DataMember]
        public Guid udo_veteranid { get; set; }
        [DataMember]
        public SecureString ssid { get; set; }
    }
}
