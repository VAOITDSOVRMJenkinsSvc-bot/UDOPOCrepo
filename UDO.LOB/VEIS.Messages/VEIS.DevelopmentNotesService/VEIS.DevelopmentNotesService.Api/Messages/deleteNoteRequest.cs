using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.DevelopmentNotesService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.DevelopmentNotesService,deleteNote method, Request.
	/// Code Generated by IMS on: 12/21/2018 2:00:12 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISdeleteddeleteNoteRequest : VEISEcRequestBase
	{
		[DataMember]
		public Guid RelatedParentId { get; set; }
		[DataMember]
		public string RelatedParentEntityName { get; set; }
		[DataMember]
		public string RelatedParentFieldName { get; set; }
		[DataMember]
		public bool LogTiming { get; set; }
		[DataMember]
		public bool LogSoap { get; set; }
		[DataMember]
		public bool Debug { get; set; }
		[DataMember]
		public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
		[DataMember]
		public VEISdeletedReqnote VEISdeletedReqnoteInfo { get; set; }
	}
	[DataContract]
	public class VEISdeletedReqnote
	{
		[DataMember]
		public string mcs_bnftClmNoteTc { get; set; }
		[DataMember]
		public string mcs_clmId { get; set; }
		[DataMember]
		public DateTime mcs_createDt { get; set; }
		[DataMember]
		public bool mcs_createDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_modifdDt { get; set; }
		[DataMember]
		public bool mcs_modifdDtSpecified { get; set; }
		[DataMember]
		public string mcs_noteId { get; set; }
		[DataMember]
		public string mcs_noteOutTn { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public string mcs_ptcpntNoteTc { get; set; }
		[DataMember]
		public string mcs_stdNoteId { get; set; }
		[DataMember]
		public DateTime mcs_suspnsDt { get; set; }
		[DataMember]
		public bool mcs_suspnsDtSpecified { get; set; }
		[DataMember]
		public string mcs_txt { get; set; }
		[DataMember]
		public string mcs_userId { get; set; }
		[DataMember]
		public string mcs_userNm { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
}
