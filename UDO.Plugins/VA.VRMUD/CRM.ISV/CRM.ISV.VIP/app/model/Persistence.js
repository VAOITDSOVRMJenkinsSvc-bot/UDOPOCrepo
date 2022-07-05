/**
* @author Ivan Yurisevic
* @class VIP.model.Persistence
*
* The model for Persistence, all search input are here.
*/
Ext.define('VIP.model.Persistence', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.Crm',
        'VIP.data.writer.Crm'
    ],
    fields: [
    // CORP parameters
		{name: 'firstName',
		type: 'string',
		mapping: 'va_firstname'
}, {
    name: 'lastName',
    type: 'string',
    mapping: 'va_lastname'
}, {
    name: 'middleName',
    type: 'string',
    mapping: 'va_middleinitial'
},

    // TODO: must update SSN in Persistence from search results
        {
        name: 'ssn',
        type: 'string',
        mapping: 'va_ssn'
    },
    // TODO: must update PID in Persistence from search results
        {
        name: 'participantId',
        type: 'string',
        mapping: 'va_participantid'
    },
    // ****************************************
    // What to use for search (ssn, pid, edipi)
 		{
 		name: 'searchType',
 		type: 'string',
 		mapping: 'va_searchtype'
},
    // ****************************************
    // Do search old payment history
        {
        name: 'searchOldPaymentHistory',
        type: 'boolean',
        mapping: 'va_searchmonthofdeath'
    },
    // ****************************************
    // Do search Pathways
        {
        name: 'searchpathways',
        type: 'boolean',
        mapping: 'va_searchpathways'
    },
    // Pathways search options
    {
    name: 'pathwaysGender',
    type: 'string',
    mapping: 'va_genderset'
},
    {
        name: 'pathwaysApptFrom',
        type: 'date',
        mapping: 'va_appointmentfromdate'
    },
    {
        name: 'pathwaysApptTO',
        type: 'date',
        mapping: 'va_appointmentTOate'
    },
        {
            name: 'pathwaysAttendedSearch',
            type: 'boolean',
            mapping: 'va_attendedsearch'
        },
    // ****************************************
    // Appeals search options
    {
    name: 'findAppealsBy',
    type: 'string',
    mapping: 'va_findappealsby'
},
    {
        name: 'appealsSearchText',
        type: 'string',
        mapping: 'va_appealssearchtext'
    },

    // ****************************************
    // BIRLS search parameters
    // TODO: BIRLS needs all dates in format("MMddyyyy")
        {
        name: 'dob',
        type: 'string',
        mapping: 'va_dobtext'
    }, {
        name: 'city',
        type: 'string',
        mapping: 'va_citysearch'
    }, {
        name: 'state',
        type: 'string',
        mapping: 'va_statesearch'
    }, {
        name: 'zipCode',
        type: 'string',
        mapping: 'va_zipcodesearch'
    }, {
        name: 'branchOfService',
        type: 'string',
        mapping: 'va_branchofservice'
    }, {
        name: 'serviceNumber',
        type: 'string',
        mapping: 'va_servicenumber'
    }, {
        name: 'insuranceNumber',
        type: 'string',
        mapping: 'va_insurancenumber'
    }, {
        name: 'dod',
        type: 'string',
        mapping: 'va_dod'
    }, {
        name: 'enteredOnDuty',
        type: 'string',
        mapping: 'va_enteredondutydate'
    }, {
        name: 'releasedActiveDuty',
        type: 'string',
        mapping: 'va_releasedactivedutydate'
    }, {
        name: 'suffix',
        type: 'string',
        mapping: 'va_suffix'
    }, {
        name: 'payeeNumber',
        type: 'string',
        mapping: 'va_payeenumber'
    },
		{
		    name: 'folderLocation',
		    type: 'string',
		    mapping: 'va_folderlocation'
		},
		// ****************************************
        // Form XML attributes
        {name: "va_webserviceresponse", type: 'string', mapping: 'va_webserviceresponse' },
		{ name: "va_findcorprecordresponse", type: 'string', mapping: 'va_findcorprecordresponse' },
		{ name: "va_findbirlsresponse", type: 'string', mapping: 'va_findbirlsresponse' },
		{ name: "va_findveteranresponse", type: 'string', mapping: 'va_findveteranresponse' },
		{ name: "va_generalinformationresponse", type: 'string', mapping: 'va_generalinformationresponse' },
		{ name: "va_generalinformationresponsebypid", type: 'string', mapping: 'va_generalinformationresponsebypid' },
		{ name: "va_findaddressresponse", type: 'string', mapping: 'va_findaddressresponse' },
		{ name: "va_benefitclaimresponse", type: 'string', mapping: 'va_benefitclaimresponse' },
		{ name: "va_findbenefitdetailresponse", type: 'string', mapping: 'va_findbenefitdetailresponse' },
		{ name: "va_findclaimstatusresponse", type: 'string', mapping: 'va_findclaimstatusresponse' },
		{ name: "va_findclaimantlettersresponse", type: 'string', mapping: 'va_findclaimantlettersresponse' },
		{ name: "va_findcontentionsresponse", type: 'string', mapping: 'va_findcontentionsresponse' },
		{ name: "va_finddependentsresponse", type: 'string', mapping: 'va_finddependentsresponse' },
		{ name: "va_findallrelationshipsresponse", type: 'string', mapping: 'va_findallrelationshipsresponse' },
		{ name: "va_finddevelopmentnotesresponse", type: 'string', mapping: 'va_finddevelopmentnotesresponse' },
		{ name: "va_findfiduciarypoaresponse", type: 'string', mapping: 'va_findfiduciarypoaresponse' },
		{ name: "va_findmilitaryrecordbyptcpntidresponse", type: 'string', mapping: 'va_findmilitaryrecordbyptcpntidresponse' },
		{ name: "va_findpaymenthistoryresponse", type: 'string', mapping: 'va_findpaymenthistoryresponse' },
		{ name: "va_findtrackeditemsresponse", type: 'string', mapping: 'va_findtrackeditemsresponse' },
		{ name: "va_findunsolvedevidenceresponse", type: 'string', mapping: 'va_findunsolvedevidenceresponse' },
		{ name: "va_finddenialsresponse", type: 'string', mapping: 'va_finddenialsresponse' },
		{ name: "va_findawardcompensationresponse", type: 'string', mapping: 'va_findawardcompensationresponse' },
		{ name: "va_findotherawardinformationresponse", type: 'string', mapping: 'va_findotherawardinformationresponse' },
		{ name: "va_findmonthofdeathresponse", type: 'string', mapping: 'va_findmonthofdeathresponse' },
		{ name: "va_findincomeexpenseresponse", type: 'string', mapping: 'va_findincomeexpenseresponse' },
		{ name: "va_findratingdataresponse", type: 'string', mapping: 'va_findratingdataresponse' },
		{ name: "va_findappealsresponse", type: 'string', mapping: 'va_findappealsresponse' },
		{ name: "va_findindividualappealsresponse", type: 'string', mapping: 'va_findindividualappealsresponse' },
		{ name: "va_appellantaddressresponse", type: 'string', mapping: 'va_appellantaddressresponse' },
		{ name: "va_updateappellantaddressresponse", type: 'string', mapping: 'va_updateappellantaddressresponse' },
		{ name: "va_createnoteresponse", type: 'string', mapping: 'va_createnoteresponse' },
		{ name: "va_findreasonsbyrbaissueidresponse", type: 'string', mapping: 'va_findreasonsbyrbaissueidresponse' },
		{ name: "va_isaliveresponse", type: 'string', mapping: 'va_isaliveresponse' },
		{ name: "va_mviresponse", type: 'string', mapping: 'va_mviresponse' },
		{ name: "va_readdataexamresponse", type: 'string', mapping: 'va_readdataexamresponse' },
		{ name: "va_readdataappointmentresponse", type: 'string', mapping: 'va_readdataappointmentresponse' },
		{ name: "va_awardfiduciaryresponse", type: 'string', mapping: 'va_awardfiduciaryresponse' },
		{ name: "va_retrievepaymentsummaryresponse", type: 'string', mapping: 'va_retrievepaymentsummaryresponse' },
		{ name: "va_retrievepaymentdetailresponse", type: 'string', mapping: 'va_retrievepaymentdetailresponse' }
		/*,
		// **********************************
		// environment data
		{
		name: 'environmentName',
		type: 'string'
		},
		{
		name: 'isPROD',
		type: 'boolean'
		},
		{
		name: 'globalDAC',
		type: 'string'
		},
		{
		name: 'CORP',
		type: 'string'
		},
		{
		name: 'MVI',
		type: 'string'
		},
		{
		name: 'MVIDAC',
		type: 'string'
		},
		{
		name: 'Pathways',
		type: 'string'
		},
		{
		name: 'Vacols',
		type: 'string'
		},
		{
		name: 'VacolsDAC',
		type: 'string'
		},
		{
		name: 'RepWS',
		type: 'string'
		}*/
	],
proxy: {
    type: 'memory',
    data: Ext.isEmpty(parent.Xrm) ? '' : parent.Xrm.Page,
    reader: 'crm',
    writer: {
    	type: 'crm',
    	writeAllFields: false
    }
}
});