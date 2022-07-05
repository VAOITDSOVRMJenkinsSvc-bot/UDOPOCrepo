/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.Decoration
*
* The model service decorations
*/
Ext.define('VIP.model.militaryservice.Decoration', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'militaryDecorationId',
        type: 'string',
        mapping: 'militaryDecorationId'
    }, {
        name: 'militaryDecorationName',
        type: 'string',
        mapping: 'militaryDecorationName'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryPersonDecorations'
        }
    }
});