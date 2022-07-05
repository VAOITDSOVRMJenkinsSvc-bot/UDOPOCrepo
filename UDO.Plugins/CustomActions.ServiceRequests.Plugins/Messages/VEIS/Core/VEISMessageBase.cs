using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VEIS.Core.Core;

namespace VEIS.Core.Messages
{
    [DataContract]
    public class ResponseBase : MessageBase
    {
        [DataMember]
        public EcDiagnostics EcDiagnostics { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public string TraceLog { get; set; }
        [DataMember]
        public decimal ProcessorTimer { get; set; }
        public ResponseBase()
        {
            EcDiagnostics = new EcDiagnostics()
            {
                Request = "",
                Response = "",
                Timer = 0
            };
        }
    }
    [DataContract]
    public class RequestBase : MessageBase
    {
        //For Future use to track multiple EC calls against an LOB from the VINEXT side
        [DataMember]
        public Guid CorrelationId { get; set; }
        [DataMember]
        public LegacyHeaderInfo HeaderInfo { get; set; }
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }

        public RequestBase()
        {
            HeaderInfo = new LegacyHeaderInfo()
            {
                ApplicationName = "",
                ClientMachine = "",
                LoginName = "",
                Password = "",
                StationNumber = ""
            };
        }
    }

    [DataContract]
    public class LegacyHeaderInfo
    {
        [DataMember]
        public string StationNumber { get; set; }
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string ClientMachine { get; set; }
        [DataMember]
        public string Password { get; set; }
    }

    [DataContract]
    public class ResponseFilter
    {

        [DataMember]
        public int ChunkNumber { get; set; }

        [DataMember]
        public bool ChunkNumberSpecified { get; set; }

        [DataMember]
        public int ChunkSize { get; set; }

        [DataMember]
        public bool ChunkSizeSpecified { get; set; }

        [DataMember]
        public string EndDate { get; set; }

        [DataMember]
        public bool EndDateSpecified { get; set; }

        [DataMember]
        public string StartDate { get; set; }

        [DataMember]
        public bool StartDateSpecified { get; set; }
    }

    [DataContract]
    public class EcDiagnostics
    {
        /// <summary>
        /// Serialized EC Request
        /// </summary>
        [DataMember]
        public string Request { get; set; }
        /// <summary>
        /// Serialized EC Response
        /// </summary>
        [DataMember]
        public string Response { get; set; }
        /// <summary>
        /// Timer started immediately before EC Request and stopped immediately upon return of response
        /// </summary>
        [DataMember]
        public decimal Timer { get; set; }

        [DataMember]
        public int NumberOfEcs { get; set; }

        public EcDiagnostics() { }
        public EcDiagnostics(bool failed)
        {
            Request = "NA";
            Response = "NA";
            Timer = 0;
        }
    }
}


