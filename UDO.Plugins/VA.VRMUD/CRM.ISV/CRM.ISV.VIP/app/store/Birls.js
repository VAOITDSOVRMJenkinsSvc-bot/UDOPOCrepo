/**
* @author Ivan Yurisevic
* @class VIP.store.Birls
*
* Store associated with Birls model
*/
Ext.define('VIP.store.Birls', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.Birls',
    model: 'VIP.model.Birls',
    remoteFilter: true,
    listeners: {
        beforeload: function (store, operation) {
            var readEnvelope = "VIP.soap.envelopes.share.vetrecord.FindBirlsRecord"; // this.getProxy().envelopes.read;
            for (var i in operation.filters) {
                var filter = operation.filters[i];

                if (filter.property == 'fileNumber') {
                    readEnvelope = 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber';
                    break;
                }
            }

            this.getProxy().envelopes.read = readEnvelope;
        }
    }
});
