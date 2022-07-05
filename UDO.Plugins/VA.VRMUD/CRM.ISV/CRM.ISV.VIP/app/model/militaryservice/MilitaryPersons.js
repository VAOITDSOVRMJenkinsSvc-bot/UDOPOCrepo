/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.MilitaryPersons
*
* The model service pows
*/
Ext.define('VIP.model.militaryservice.MilitaryPersons', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'activeDutyStatusInd',
        type: 'string',
        mapping: 'activeDutyStatusInd'
    }, {
        name: 'deathInServiceInd',
        type: 'string',
        mapping: 'deathInSvcInd'
    }, {
        name: 'disabilityServiceInd',
        type: 'string',
        mapping: 'disabilitySvcInd'
    }, {
        name: 'gulfWarRegistryInd',
        type: 'string',
        mapping: 'gulfWarRegistryInd'
    }, {
        name: 'incompetentInd',
        type: 'string',
        mapping: 'incompetentInd'
    }, {
        name: 'insuranceFileNumber',
        type: 'string',
        mapping: 'insuranceFileNumber'
    }, {
        name: 'insurancePolicyNumber',
        type: 'string',
        mapping: 'insurancePolicyNumber'
    }, {
        name: 'lgyEntitlementAmount',
        type: 'string',
        mapping: 'lgyEntitlementAmount'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }, {
        name: 'reserveInd',
        type: 'string',
        mapping: 'reserveInd'
    }, {
        name: 'totalActiveServiceDays',
        type: 'string',
        mapping: 'totalActiveSvcDays'
    }, {
        name: 'totalActiveServiceMonths',
        type: 'string',
        mapping: 'totalActiveSvcMonths'
    }, {
        name: 'totalActiveServiceYears',
        type: 'string',
        mapping: 'totalActiveSvcYears'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryPersons'
        }
    }
});