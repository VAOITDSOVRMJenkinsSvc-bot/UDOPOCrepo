using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VVADocOperations.Api.Messages
{ 
	[DataContract] 
	public class VEISvvado_GetDocumentListResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEISvvado_dcmntRecordCollection VEISvvado_dcmntRecordCollectionInfo
		{
			get;
			set;
		}

		[DataMember]
		public bool ExceptionOccured
		{
			get;
			set;
		} 
	}
    [DataContract]
    public class VEISvvado_dcmntRecordCollection
    {
        [DataMember]
        public string mcs_message { get; set; }
        [DataMember]
        public Int32 mcs_size { get; set; }
        [DataMember]
        public VEISvvado_dcmntRecordMultipleResponse[] VEISvvado_dcmntRecordInfo { get; set; }
    }
    [DataContract]
    public class VEISvvado_dcmntRecordMultipleResponse
    {
        [DataMember]
        public string mcs_dcmntRecordId { get; set; }
        [DataMember]
        public string mcs_authorNm { get; set; }
        [DataMember]
        public Int32 mcs_authorRoNbr { get; set; }
        [DataMember]
        public string mcs_batchNm { get; set; }
        [DataMember]
        public string mcs_categyListTxt { get; set; }
        [DataMember]
        public string mcs_cntctNm { get; set; }
        [DataMember]
        public string mcs_dcmntDt { get; set; }
        [DataMember]
        public string mcs_dcmntFormatCd { get; set; }
        [DataMember]
        public Int32 mcs_dcmntSizeNbr { get; set; }
        [DataMember]
        public string mcs_dcmntTypeLupId { get; set; }
        [DataMember]
        public string mcs_efoldrId { get; set; }
        [DataMember]
        public string mcs_ivmYearNbr { get; set; }
        [DataMember]
        public string mcs_lastUserModifdNm { get; set; }
        [DataMember]
        public string mcs_rcvdDt { get; set; }
        [DataMember]
        public string mcs_readInd { get; set; }
        [DataMember]
        public string mcs_recdsMgmntNbr { get; set; }
        [DataMember]
        public Int32 mcs_roLupId { get; set; }
        [DataMember]
        public string mcs_rstrcdDcmntInd { get; set; }
        [DataMember]
        public string mcs_rstrcdReasonLupId { get; set; }
        [DataMember]
        public string mcs_sourceComntTxt { get; set; }
        [DataMember]
        public string mcs_sourceTxt { get; set; }
        [DataMember]
        public string mcs_storgDt { get; set; }
        [DataMember]
        public string mcs_subjctTxt { get; set; }
        [DataMember]
        public string mcs_trtmntBeginDt { get; set; }
        [DataMember]
        public string mcs_trtmntCndtnTxt { get; set; }
        [DataMember]
        public string mcs_trtmntEndDt { get; set; }
        [DataMember]
        public string mcs_writeOutTypeTxt { get; set; }
        [DataMember]
        public string mcs_dcmntTypeDescpTxt { get; set; }
        [DataMember]
        public string mcs_fnDcmntId { get; set; }
        [DataMember]
        public string mcs_fnDcmntSource { get; set; }
        [DataMember]
        public string mcs_claimNbr { get; set; }
        [DataMember]
        public string mcs_ssnNbr { get; set; }
        [DataMember]
        public string mcs_vetFirstName { get; set; }
        [DataMember]
        public string mcs_vetMiddleNm { get; set; }
        [DataMember]
        public string mcs_vetLastName { get; set; }
        [DataMember]
        public string mcs_vetBirthDt { get; set; }
        [DataMember]
        public string mcs_jrsdtnRoNbr { get; set; }
    }

}
