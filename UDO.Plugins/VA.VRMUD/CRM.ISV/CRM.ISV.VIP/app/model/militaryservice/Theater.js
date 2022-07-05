/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.Theater
*
* The model service theaters
*/
Ext.define('VIP.model.militaryservice.Theater', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'beginDate',
        type: 'date',
        dateFormat: 'm/d/Y',
        mapping: 'beginDate'
    }, {
        name: 'days',
        type: 'string',
        mapping: 'days'
    }, {
        name: 'militaryPersonTourNumber',
        type: 'string',
        mapping: 'militaryPersonTourNbr'
    }, {
        name: 'militaryTheatreTypeName',
        type: 'string',
        mapping: 'militaryTheatreTypeName'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }, {
        name: 'verifiedIndicator',
        type: 'string',
        mapping: 'verifiedInd'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryTheatres'
        }
    }
});