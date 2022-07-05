/**
* @author Ivan Yurisevic
* @class VIP.model.personinfo.Flashes
*
* The model for flashes associated with the person
*/
Ext.define('VIP.model.personinfo.Flashes', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'assignedInd',
        mapping: 'assignedIndicator',
        type: 'string'
    }, {
        name: 'flashName',
        mapping: 'flashName',
        type: 'string'
    }, {
        name: 'flashType',
        mapping: 'flashType',
        type: 'string'
    }],

    //Start Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'flashes'
        }
    }

});