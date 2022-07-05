using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VVADocOperations.Api.Messages
{ 
	[DataContract] 
	public class VEISvvado_GetDocumentListRequest : VEISEcRequestBase
    { 

		[DataMember]
		public Guid RelatedParentId
		{
			get;
			set;
		}

		[DataMember]
		public string RelatedParentEntityName
		{
			get;
			set;
		}

		[DataMember]
		public string RelatedParentFieldName
		{
			get;
			set;
		}

		[DataMember]
		public bool LogTiming
		{
			get;
			set;
		}

		[DataMember]
		public bool LogSoap
		{
			get;
			set;
		}

		[DataMember]
		public bool Debug
		{
			get;
			set;
		}

		[DataMember]
		public LegacyHeaderInfo LegacyServiceHeaderInfo
		{
			get;
			set;
		}

		[DataMember]
		public VEISvvado_documentlist documentlistInfo
		{
			get;
			set;
		}
        public class VEISvvado_documentlist
        {
            [DataMember]
            public string mcs_claimNbr { get; set; }
        }
    }
}
