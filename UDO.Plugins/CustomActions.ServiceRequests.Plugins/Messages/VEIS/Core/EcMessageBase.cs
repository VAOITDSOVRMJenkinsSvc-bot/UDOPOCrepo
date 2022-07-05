using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VEIS.Core.Core;
namespace VEIS.Core.Messages
{

    ////RICH: NEW FILE/CLASSES - Core capability enabled through use of Base Classes
    [DataContract]
    public class VEISEcResponseBase : MessageBase
    {
        /// <summary>
        /// Indicates if the EC failed
        /// </summary>
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        /// <summary>
        /// The Error Message if a failure occurred
        /// </summary>
        [DataMember]
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// The SOAP Request that was sent to the Endpoint
        /// </summary>
        [DataMember]
        public string SerializedSOAPRequest { get; set; }
        /// <summary>
        /// The SOAP Response that was replied back to the request
        /// </summary>
        [DataMember]
        public string SerializedSOAPResponse { get; set; }
        /// <summary>
        /// The time it took to process the EC Handler
        /// </summary>
        [DataMember]
        public long ProcessorTimer { get; set; }
        /// <summary>
        /// The time it took between the request entering the WCF pipeline and the response leaving it
        /// </summary>
        [DataMember]
        public long ServiceTimer { get; set; }
        /// <summary>
        /// The Guid to track all ECs across a single LOB instance
        /// </summary>
        [DataMember]
        public Guid CorrelationId { get; set; }
        /// <summary>
        /// The string trace of the EC
        /// </summary>
        [DataMember]
        public string EcTraceLog { get; set; }
    }


    [DataContract]
    public class VEISEcRequestBase : MessageBase
    {
        /// <summary>
        /// The Name of the Organization that initiated this call
        /// </summary>
        [DataMember]
        public string OrganizationName { get; set; }
        /// <summary>
        /// the Guid of the User who initiated this call
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// Identifier to track all ECs messages called by a single LOB message
        /// </summary>
        [DataMember]
        public Guid CorrelationId { get; set; }
    }
}