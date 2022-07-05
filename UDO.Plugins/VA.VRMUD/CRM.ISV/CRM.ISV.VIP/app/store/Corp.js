/**
* @author Jonas Dawson
* @class VIP.store.Corp
*
* Store associated with personinquiry corp model
*/
Ext.define('VIP.store.Corp', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Corp',
        'VIP.soap.envelopes.share.vetrecord.FindVeteranByPtcpntId',
        'VIP.soap.envelopes.share.vetrecord.FindCorporateRecordByFileNumber'
    ],
    model: 'VIP.model.Corp',
    listeners: {
        beforeload: function (store, operation) {
            // Set dafault envelop
            var readEnvelope = "VIP.soap.envelopes.share.vetrecord.FindCorporateRecord"; // this.getProxy().envelopes.read;
            for (var i in operation.filters) {
                var filter = operation.filters[i];
                if (filter.property == 'ptcpntId') {
                    readEnvelope = 'VIP.soap.envelopes.share.vetrecord.FindVeteranByPtcpntId';
                }
                if (filter.property == 'fileNumber') {
                    readEnvelope = 'VIP.soap.envelopes.share.vetrecord.FindCorporateRecordByFileNumber';
                    break; //break on file number because this is the preferred search param.
                }
            }

            this.getProxy().envelopes.read = readEnvelope;
        }
    }
});

