/**
* @author Hung Tran
* @class VIP.model.awards.awardinfo.ClothingAllowances
*
* Submodel for the award info response. Required with the association.
*/
Ext.define('VIP.model.awards.awardinfo.ClothingAllowances', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'eligibilityYear',
        mapping: 'eligibilityYear',
        type: 'date',
        dateFormat: 'Y'
    }, {
        name: 'netAward',
        mapping: 'netAward',
        type: 'float'
    }, {
        name: 'grossAward',
        mapping: 'grossAward',
        type: 'float'
    }, {
        name: 'incarcerationAdjustment',
        mapping: 'incarcerationAdjustment',
        type: 'string'
    }],

    //Start Memory Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'clothingAllowanceInfo'
        }
    }
});