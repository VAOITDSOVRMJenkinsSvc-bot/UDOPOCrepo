using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UDO.LOB.Core
{
    // TODO: There is error on MVIController "cannot convert type Void to ImessageBase"
    //       This when controller is calling the processor and processor is execution is 'public void'
    public interface IMessageBase : IExtensibleDataObject
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        string MessageId { get; set; }
    }

    [DataContract]
    //[DebuggerStepThrough]
    public abstract class MessageBase : IMessageBase, IExtensibleDataObject
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string MessageId { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }

        // Related Parent Information is not always required
        [DataMember(IsRequired = false)]
        public Guid RelatedParentId { get; set; }
        [DataMember(IsRequired = false)]
        public string RelatedParentEntityName { get; set; }
        [DataMember(IsRequired = false)]
        public string RelatedParentFieldName { get; set; }

        [DataMember(IsRequired = false)]
        public DiagnosticsContext DiagnosticsContext { get; set; }
        [DataMember(IsRequired = false)]
        public DiagnosticsConfiguration DiagnosticsConfiguration { get; set; }
    }

    [DataContract]
    public class DiagnosticsContext
    {
        #region Telemetry Context
        [DataMember(IsRequired = false)]
        public Guid InteractionId { get; set; }

        [DataMember(IsRequired = false)]
        public Guid IdProof { get; set; }

        [DataMember(IsRequired = false)]
        public Guid RequestId { get; set; }

        [DataMember(IsRequired = false)]
        public Guid VeteranId { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AgentId { get; set; }

        [DataMember(IsRequired = false)]
        public string MessageTrigger { get; set; }

        [DataMember(IsRequired = false)]
        public string OrganizationName { get; set; }

        [DataMember(IsRequired = false)]
        public string StationNumber { get; set; }
        #endregion

    }

    [DataContract]
    public class DiagnosticsConfiguration
    {
        #region Telemetry Configuration
        [DataMember(IsRequired = false)]
        public bool TrasferEvents { get; set; }

        [DataMember(IsRequired = false)]
        public bool TransferTrace { get; set; }
        #endregion

    }
}
