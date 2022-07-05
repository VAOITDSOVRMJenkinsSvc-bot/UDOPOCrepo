/**
* @author Dmitri Riz
* @class VIP.model.Pcr
*
* The model for the pcr making the web service search
*/
Ext.define('VIP.model.Pcr', {
    extend: 'Ext.data.Model',
    fields: [{
            name: 'userName',
            type: 'string'
        }, {
            name: 'password',
            type: 'string'
        }, {
            name: 'clientMachine',
            type: 'string'
        }, {
            name: 'stationId',
            type: 'string'
        }, {
            name: 'applicationName',
            type: 'string'
        }, {
            name: 'pcrSensitivityLevel',
            type: 'string'
        }, {
            name: 'loginName',
            type: 'string'
        }, {
            name: 'ssn',
            type: 'string'
        }, {
            name: 'fileNumber',
            type: 'string'
        }, {
            name: 'fullName',
            type: 'string'
        }, {
            name: 'email',
            type: 'string'
        }, {
            name: 'pcrId',
            type: 'string'
        }, {
            name: 'site',
            type: 'string'
        }
    ]
});