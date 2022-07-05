using System;


namespace VRM.CRME.Plugin.DependentMaintenance.Messages
{
    public class GetMaritalInfoResponse 
    {
        public string MessageId { get; set; }
        public GetMaritalInfoMultipleResponse[] GetMaritalInfo { get; set; }
        public string Fault { get; set; }
        public string SoapLog { get; set; }
    }

    public class GetMaritalInfoMultipleResponse 
    {
        public string MessageId { get; set; }
        
        public string crme_State { get; set; }
        
        public string crme_SpouseSSN { get; set; }
        //CSDEv
        //public DateTime crme_MarriageStartDate { get; set; }
        public string crme_MarriageStartDate { get; set; }
        //CSDev
        //public DateTime crme_MarriageEndDate { get; set; }
        public string crme_MarriageEndDate { get; set; }
        
        public string crme_LastName { get; set; }
        
        public string crme_FirstName { get; set; }
        //CSDev
        //public DateTime? crme_DOB { get; set; }
        public string crme_DOB { get; set; }
        
        public string crme_Country { get; set; }
        
        public string crme_City { get; set; }
        //CSDev
        //public DateTime? crme_RelationshipBeginDate { get; set; }
        public string crme_RelationshipBeginDate { get; set; }
        //CSDev
        //public DateTime? crme_RelationshipEndDate { get; set; }
        public string crme_RelationshipEndDate { get; set; }
        
        public string crme_AwardInd { get; set; }
    }
}
