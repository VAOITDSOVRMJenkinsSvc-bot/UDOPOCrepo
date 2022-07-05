/**
* @author Jonas Dawson
* @class VIP.model.BirlsPersonSelection
*
* The model for person BIRLS detail information
*/
Ext.define('VIP.model.BirlsPersonSelection', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'veteranIndicator',
        mapping: 'VET_IND',
        type: 'string'
    }, {
        name: 'lastName',
        mapping: 'LAST_NAME',
        type: 'string'
    }, {
        name: 'firstName',
        mapping: 'FIRST_NAME',
        type: 'string'
    }, {
        name: 'middleName',
        mapping: 'MIDDLE_NAME',
        type: 'string'
    }, {
        name: 'suffixName',
        mapping: 'SUFFIX',
        type: 'string'
    }, {
        name: 'fileNumber',
        mapping: 'FILE_NUMBER',
        type: 'string'
    }, {
        name: 'payeeCode',
        mapping: 'PAYEE_CODE',
        type: 'string'
    }, {
        name: 'currentLocation',
        mapping: 'CURRENT_LOCATION',
        type: 'string'
    }, {
        name: 'eod',
        mapping: 'ENTERED_ON_DUTY_DATE',
        type: 'string'
    }, {
        name: 'rad',
        mapping: 'RELEASED_ACTIVE_DUTY_DATE',
        type: 'string'
    }, {
        name: 'dob',
        mapping: 'DATE_OF_BIRTH',
        type: 'string'
    }, {
        name: 'dod',
        mapping: 'DATE_OF_DEATH',
        type: 'string'
    }, {
        name: 'ssnVerified',
        mapping: 'SSN_VERIFIED',
        type: 'string'
    }, {
        name: 'ssn',
        mapping: 'SSN',
        type: 'string'
    }, {
        name: 'serviceNumber',
        mapping: 'SERVICE_NUMBER',
        type: 'string'
    }, {
        name: 'branchOfService',
        mapping: 'BRANCH_OF_SERVICE',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'BIRLS_SELECTION'
        }
    }
});