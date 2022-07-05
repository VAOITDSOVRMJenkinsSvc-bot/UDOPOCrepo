using System;



namespace VRM.CRME.Plugin.DependentMaintenance.Messages

{
    public class GetDependentInfoResponse 
    {
        public string MessageId { get; set; }

        public GetDependentInfoMultipleResponse[] GetDependentInfo { get; set; }

        public string Fault { get; set; }

        public string SoapLog { get; set; }

    }
    public class GetDependentInfoMultipleResponse 
    {
        public string MessageId { get; set; }
                
        public string crme_Zip { get; set; }
        
        public string crme_State { get; set; }
        
        public string crme_SSN { get; set; }
        
        public string crme_SpouseVAFileNumber { get; set; }
        
        public string crme_SpousePrimaryPhone { get; set; }
        
        public bool crme_SpouseisVeteran { get; set; }
        
        public int crme_MonthlyContributiontoSpouseSupport { get; set; }
        
        public string crme_MiddleName { get; set; }
        
        public string crme_MarriageState { get; set; }
        //CSDev
        //public DateTime crme_MarriageDate { get; set; }
        public string crme_MarriageDate { get; set; }
        
        public string crme_MarriageCountry { get; set; }
        
        public string crme_MarriageCity { get; set; }
        
        public bool crme_LiveswithSpouse { get; set; }
        
        public string crme_LastName { get; set; }
        
        public string crme_FirstName { get; set; }
        //CSDEv
        //public DateTime? crme_DOB { get; set; }
        public string crme_DOB { get; set; }
        
        public int crme_DependentStatus { get; set; }
        
        public int crme_DependentRelationship { get; set; }
        
        public string crme_County { get; set; }
        
        public string crme_Country { get; set; }
        
        public string crme_City { get; set; }
        
        public bool crme_ChildSeriouslyDisabled { get; set; }
        
        public int crme_ChildRelationship { get; set; }
        
        public string crme_ChildPrimaryPhone { get; set; }
        
        public bool crme_ChildPreviouslyMarried { get; set; }
        
        public string crme_ChildPlaceofBirthState { get; set; }
        
        public string crme_ChildPlaceofBirthCountry { get; set; }
        
        public string crme_ChildPlaceofBirthCity { get; set; }
        
        public string crme_ChildLiveswithMiddleName { get; set; }
        
        public string crme_ChildLiveswithLastName { get; set; }
        
        public string crme_ChildLiveswithFirstName { get; set; }
        
        public bool crme_ChildLiveswithVet { get; set; }
        
        public bool crme_ChildAge1823InSchool { get; set; }
        
        public string crme_Address3 { get; set; }
        
        public string crme_Address2 { get; set; }
        
        public string crme_Address1 { get; set; }
        
        public string crme_name { get; set; }
        
        public string crme_SuffixName { get; set; }
        
        public string crme_ChildLIveswithSuffixName { get; set; }
        
        public string crme_TermnlDigitNbr { get; set; }
        //CSDev
        //public DateTime? crme_RelationshipBeginDate { get; set; }
        public string crme_RelationshipBeginDate { get; set; }
        //CSDev
        ////public DateTime? crme_RelationshipEndDate { get; set; }
        public string crme_RelationshipEndDate { get; set; }
        
        public string crme_AwardInd { get; set; }
    }
}
