/**
* @author Ivan Yurisevic
* @class VIP.model.FiduciaryPoa
*
* The model for fiduciary and poa
*/
Ext.define('VIP.model.FiduciaryPoa', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.FiduciaryPoa',
        'VIP.soap.envelopes.share.claimant.FindAllFiduciaryPoa',
        'VIP.model.fiduciary.CurrentFiduciary',
        'VIP.model.poa.CurrentPoa',
        'VIP.model.fiduciary.PastFiduciaries',
        'VIP.model.poa.PastPoas'
    ],
    fields: [{
        name: 'numberOfFiduciaries',
        type: 'string'
    }, {
        name: 'numberOfPOA',
        type: 'string'
    }, {
        name: 'returnCode',
        type: 'string'
    }, {
        name: 'returnMessage',
        type: 'string'
    }],
    hasMany: [
        {
            model: 'VIP.model.fiduciary.CurrentFiduciary',
            name: 'currentFiduciary',
            associationKey: 'currentFiduciary'
        },
        {
            model: 'VIP.model.poa.CurrentPoa',
            name: 'currentPoa',
            associationKey: 'currentPowerOfAttorney'
        },
        {
            model: 'VIP.model.fiduciary.PastFiduciaries',
            name: 'pastFiduciaries',
            associationKey: 'fiduciaries'
        },
        {
            model: 'VIP.model.poa.PastPoas',
            name: 'pastPoas',
            associationKey: 'poas'
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'fiduciarypoa',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindAllFiduciaryPoa',
            update: '',
            destroy: ''
        }
    }
});