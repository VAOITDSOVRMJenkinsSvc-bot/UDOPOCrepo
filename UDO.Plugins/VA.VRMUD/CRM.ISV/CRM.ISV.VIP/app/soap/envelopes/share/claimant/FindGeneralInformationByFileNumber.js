Ext.define('VIP.soap.envelopes.share.claimant.FindGeneralInformationByFileNumber', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindGeneralInformationByFileNumber',
    requires: [
        'VIP.model.Awards'
    ],
    config: {
        fileNumber: ''
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findGeneralInformationByFileNumber', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            }
        });
    },

    /**
    * @method analyzeResponse customReadRecords
    *
    * This analyze response method will check to see if the return is a multiple award response
    * or a single award response.  If it is multiple, no further action is needed as the store
    * and the View's grid are configured to multiple responses by default and will load properly. 
    * @return currentResultSet
    *
    * If the response is a single award response, we will grab the single award model and read the response.
    * We then return that to the callback, where it MUST reconfigure grid columns to match the 
    * data that comes back.
    * @return singleAwardResultSet
    *
    * Error case: Just return the currentResultSet from the parameter.
    */
    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet) {

            if (Ext.isEmpty(currentResultSet) || Ext.isEmpty(response)) { return currentResultSet; }

            if (currentResultSet.count > 1) {
                currentResultSet['isMultipleResponse'] = true;
                return currentResultSet;
            }
            else {
                var singleAwardModel = Ext.create('VIP.model.Awards'),
                    singleAwardResultSet = singleAwardModel.getProxy().getReader().read(response);

                singleAwardResultSet['isMultipleResponse'] = false;
                return singleAwardResultSet;
            }
        },
        scrubFilter: function (record) {
            var recordHasData = false;

            for (var i in record.data) {
                if (!Ext.isEmpty(record.data[i]) && record.data[i] != 0 && i != 'numberOfAwardBenes') {
                    recordHasData = true;
                    break;
                }
            }

            return recordHasData;
        }
    })

});


