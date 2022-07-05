/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
/// <reference path="Action.js" />

_isLoading = false;
_searchvisible = false;
_currentScriptUrl = '';
_iframesrc = '';
_isLoading = true;
_SearchFunction = null;
_IdentifyIndividual = null;
_CreateClaimServiceRequest = null;
_ChangeOfAddressOnClick = null;
_QueryDevNotes = null;
_CreateDevNoteLogEntry = null;
_ValidatePermissionToEditNote = null;
_ValidateIDProofingForAddressChange = null;
_dispStructureAndPopup = null;
_changeOfAddressData = null;
_CORP_RECORD_COUNT = 0;
_BIRLS_RECORD_COUNT = 0;
_AWARDBENEFIT_RECORD_COUNT = 0;
_CLAIM_RECORD_COUNT = 0;
_emergency = false;
_RedrawExtjsAddressTab = null;
_SetPrimaryTypeSubtype = null;

_FrameLoader = null;

// flags setting search systems
_searchCorpMin = true;
_searchCorp = false;    // all corp data
_searchCorpAwardsOnly = false;
_searchCorpClaimsOnly = false;
_searchBirls = false;
_searchVadir = false;
_searchPathways = false;
_searchOldPayments = false;

/// Memory object to store response xml from veteran general information by file number and participant id
/// Object key reference ex:
/// var awardBenefitPK = awardTypeCd + '_' + ptcpntVetId + '_' + ptcpntBeneId + '_' + ptpcntRecipId;
/// 'SA03_13086523_13086523_13086523'
/// Reference to XML string:  _AWARDBENEFIT_RESPONSE_OBJECT[awardBenefitPk];
_AWARDBENEFIT_RESPONSE_OBJECT = new Object();