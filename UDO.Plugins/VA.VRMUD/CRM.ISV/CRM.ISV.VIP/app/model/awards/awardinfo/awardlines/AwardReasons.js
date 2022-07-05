/**
* @author Ivan Yurisevic
* @class VIP.model.awards.awardinfo.awardlines.AwardReasons
*
* Submodel for the award lines response. Required with the association.
*/
Ext.define('VIP.model.awards.awardinfo.awardlines.AwardReasons', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'name',
        mapping: 'name',
        type: 'string'
    }],

    //Start Memory Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'awardReasons'
        }
    }
});