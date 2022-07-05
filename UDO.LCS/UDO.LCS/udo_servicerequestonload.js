/*******************BEGIN correspondence OnLoad******************/


// These methods are called OnLoad from USD Action for correspondence interactions
var CorrSubType = null;


BuildFilter = function () {
// CSDev Left Intentionally Blank 
}

FilterDocTypes = function () {
   // CSDev Left Intentionally Blank  Xrm.Page.getControl("udo_vbmsdoctype").addPreSearch(BuildFilter);
}

SRFilterOnLoad = function (type, subtype) {
// CSDev Left Intentionally Blank 
}

/*******************END correspondence OnLoad******************/

/**************BEGIN main OnLoad event (formscript)************/
// This method will be call from CRM form
function OnLoad() {
// CSDev Left Intentionally Blank 
}

// CSDev Left Intentionally Blank 

// Begin Form onload event - retrieve data from VIP if necessary
function onFormLoad() {
// CSDev Left Intentionally Blank 
}

// Show some of the  Notes section fields which contains the historical information for the migrated records. These fields will not show up for newly created UDO Service Request Records
function showHistoricalData() {
  // CSDev Left Intentionally Blank 
}

function serviceRequest() {
 // CSDev Left Intentionally Blank 
}
/*******************************END ONLOAD EVENT*****************************************/

//Retrieve the system settings information related to Service Request
function GetSystemSettings() {
// CSDev Left Intentionally Blank 
}

////Set the send email to veteran field when the action is set to Email Forms
//function EmailtoVet() {
//    if (Xrm.Page.getAttribute('udo_action').getValue() != null) {
//        if (Xrm.Page.getAttribute('udo_action').getValue() == Globals.srAction.ActionEmailFormsValue) {
//            Xrm.Page.getAttribute('udo_sendemailtoveteran').setValue(true);
//        }
//        else {
//            Xrm.Page.getAttribute('udo_sendemailtoveteran').setValue(false);
//        }
//    }
//}

function StatusChange() {
  // CSDev Left Intentionally Blank 
}

function doesControlHaveAttribute(control) {
   // CSDev Left Intentionally Blank 
}

function disableFormFields(onOff) {
 // CSDev Left Intentionally Blank 
}

//Populate the enclosure text area which is referred in the reports/email
function PopulateEnclosures() {
 // CSDev Left Intentionally Blank 
}

function SendEmailToRO(filedownloaded) {
// CSDev Left Intentionally Blank 
}

function SendEmailToROContinue(filedownloaded) {
// CSDev Left Intentionally Blank
}


function ConfirmSendEmail() {
// CSDev Left Intentionally Blank 
}

function ProcessClaimEstablishment() {
 // CSDev Left Intentionally Blank 
}

function ConfirmMarkAsSent() {
  // CSDev Left Intentionally Blank 
}

//Create the email and populate the necessary details
function CreateOutlookEmail2(filedownloaded) {
// CSDev Left Intentionally Blank 
}

//cchano - updated to reflect item #646985
//Yes&No fields associated to 0820a should be nulled out and required when 0820a is selected
function RequireYesNo(fnod) {
// CSDev Left Intentionally Blank 
}

// change visibility of sections on the form based on type
function ServiceTypeChange() {
// CSDev Left Intentionally Blank 
}

function QuickWriteChange() {
// CSDev Left Intentionally Blank 
}

function ProcessedInShareChange() {
// CSDev Left Intentionally Blank 
}

function FnodOtherChange() {
   // CSDev Left Intentionally Blank 
}

function ShowScript() {
// CSDev Left Intentionally Blank 
}

function getVBMSRole() {
// CSDev Left Intentionally Blank 

}

function getManager() {
// CSDev Left Intentionally Blank 
}

function copyMailingAddress() {
// CSDev Left Intentionally Blank 
}

String.prototype.replaceAll = function (search, replacement) {
// CSDev Left Intentionally Blank 
};

function GetLookupId(lookupAttributeName) {
// CSDev Left Intentionally Blank 
}

function HandleRestError(err, message) {
// CSDev Left Intentionally Blank 
}

// process substition tokens
// each one looks like <!va_ssn!> or <!udo_ssn!>
// will take input abe<!va_ssn!>asdf and replace with abe555667777asdf
function ReplaceFieldTokens(qw) {
// CSDev Left Intentionally Blank 
}


function openOutlookEmail(opts, filedownloaded) {
// CSDev Left Intentionally Blank 
}

function CreateAndOpenServiceRequest() {
// CSDev Left Intentionally Blank 
}
/*      var serviceRequest = {};
      var interaction = Xrm.Page.getAttribute('udo_originatinginteractionid').getValue();
      if (interaction != null) {
          serviceRequest.udo_originatinginteractionid = { Id: interaction[0].id, LogicalName: "udo_interaction" };
      }

      serviceRequest.udo_FirstName = Xrm.Page.getAttribute('udo_firstname').getValue();
      serviceRequest.udo_LastName = Xrm.Page.getAttribute('udo_lastname').getValue();
      if (Xrm.Page.getAttribute('udo_relationtoveteran').getValue() != null) {
          serviceRequest.udo_relationtoveteran = { Value: Xrm.Page.getAttribute('udo_relationtoveteran').getValue() };
      }

      serviceRequest.udo_NameofReportingIndividual = Xrm.Page.getAttribute('udo_nameofreportingindividual').getValue();

      var idProof = Xrm.Page.getAttribute('udo_servicerequestsid').getValue();
      if (idProof != null) {
          serviceRequest.udo_ServiceRequestsId = { Id: idProof[0].id, LogicalName: "udo_idproof" };
      }
      //serviceRequest.udo_ServiceRequestsId = Xrm.Page.getAttribute('udo_ServiceRequestsId').getValue();

      var veteranContact = Xrm.Page.getAttribute('udo_relatedveteranid').getValue();
      if (veteranContact != null) {
          serviceRequest.udo_RelatedVeteranId = { Id: veteranContact[0].id, LogicalName: "contact" };
      }
      //serviceRequest.udo_RelatedVeteranId = Xrm.Page.getAttribute('udo_RelatedVeteranId').getValue();
      var regionalOffice = Xrm.Page.getAttribute('udo_regionalofficeid').getValue();
      if (regionalOffice != null) {
          serviceRequest.udo_RegionalOfficeId = { Id: regionalOffice[0].id, LogicalName: "va_regionaloffice" };
      }
      //serviceRequest.udo_RegionalOfficeId = Xrm.Page.getAttribute('udo_RegionalOfficeId').getValue();

      serviceRequest.udo_EmailofVeteran = Xrm.Page.getAttribute('udo_emailofveteran').getValue();
      serviceRequest.udo_SSN = Xrm.Page.getAttribute('udo_ssn').getValue();
      serviceRequest.udo_FileNumber = Xrm.Page.getAttribute('udo_filenumber').getValue();
      serviceRequest.udo_DateofDeath = Xrm.Page.getAttribute('udo_dateofdeath').getValue();

      serviceRequest.udo_ParticipantID = Xrm.Page.getAttribute('udo_participantid').getValue();
      serviceRequest.udo_BranchofService = Xrm.Page.getAttribute('udo_branchofservice').getValue();
      serviceRequest.udo_VetFirstName = Xrm.Page.getAttribute('udo_vetfirstname').getValue();
      serviceRequest.udo_VetLastName = Xrm.Page.getAttribute('udo_vetlastname').getValue();
      var person = Xrm.Page.getAttribute('udo_personid').getValue();
      if (person != null) {
          serviceRequest.udo_PersonId = { Id: person[0].id, LogicalName: "udo_person" };
          serviceRequest.udo_IsVeteran = false;
      }
      else {
          serviceRequest.udo_IsVeteran = true;
      }
      //serviceRequest.udo_PersonId = Xrm.Page.getAttribute('udo_PersonId').getValue();
      serviceRequest.udo_SendNotestoMAPD = false;

      serviceRequest.udo_mailing_address1 = Xrm.Page.getAttribute('udo_mailing_address1').getValue();
      serviceRequest.udo_mailing_address2 = Xrm.Page.getAttribute('udo_mailing_address2').getValue();
      serviceRequest.udo_mailing_address3 = Xrm.Page.getAttribute('udo_mailing_address3').getValue();
      serviceRequest.udo_mailing_city = Xrm.Page.getAttribute('udo_mailing_city').getValue();
      serviceRequest.udo_mailing_state = Xrm.Page.getAttribute('udo_mailing_state').getValue();
      serviceRequest.udo_MailingCountry = Xrm.Page.getAttribute('udo_mailingcountry').getValue();
      serviceRequest.udo_mailing_zip = Xrm.Page.getAttribute('udo_mailing_zip').getValue();

      serviceRequest.udo_SRSSN = Xrm.Page.getAttribute('udo_srssn').getValue();

      serviceRequest.udo_DayPhone = Xrm.Page.getAttribute('udo_dayphone').getValue();
      serviceRequest.udo_EveningPhone = Xrm.Page.getAttribute('udo_eveningphone').getValue();
      serviceRequest.udo_SRGender = Xrm.Page.getAttribute('udo_srgender').getValue();
      //serviceRequest.udo_SRDOB = Xrm.Page.getAttribute('udo_SRDOB').getValue();
      serviceRequest.udo_SRDOBText = Xrm.Page.getAttribute('udo_srdobtext').getValue();
      serviceRequest.udo_SRFirstName = Xrm.Page.getAttribute('udo_srfirstname').getValue();
      serviceRequest.udo_SRLastName = Xrm.Page.getAttribute('udo_srlastname').getValue();
      serviceRequest.udo_SREmail = Xrm.Page.getAttribute('udo_sremail').getValue();

      createServiceRequest(serviceRequest);
  }
}

function createServiceRequest(serviceRequest) {

  CrmRestKit2011.Create('udo_servicerequest', serviceRequest)
  .fail(function (err) {
      HandleRestError(err, 'Failed to create Service Request');
  })
  .done(function (data, status, xhr) {
      if (data.d != null && data.d.udo_servicerequestId != null) {
          CrmRestKit2011.Update('udo_servicerequest', Xrm.Page.data.entity.getId(), { udo_SendNotestoMAPD: true}, false)
              .fail(function (err) {
                  HandleRestError(err, 'Failed to create MAPD notes for the Service Request');
              })
              .done(function (data, status, xhr) {
              });
          openServiceRequest(data.d.udo_servicerequestId);
      }
  });
}
*/

function ExecuteCloneServiceRequest() {
// CSDev Left Intentionally Blank 
}

function openServiceRequest(servicerequestId) {
   // CSDev Left Intentionally Blank 
}