﻿Ext.define('VIP.soap.envelopes.share.vetrecord.FindVeteranByPtcpntId', {
    extend: 'VIP.soap.envelopes.share.VetRecordTemplate',
    alias: 'envelopes.FindVeteranByPtcpntId',
    config: {
        ptcpntId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            var newResultSet;

            newResultSet = reader.read(response);

            if (!Ext.isEmpty(_extApp)) {
                var message = _extApp.webServiceMessageHandler.retrieveWebServiceMessages(response);
                if (!Ext.isEmpty(message) && message.success == false) {
                    //error found - searcherror event will stop loadmask and display message.
                    _extApp.fireEvent('searcherror', message.returnCode + ' - ' + message.returnMessage);
                    newResultSet.success = false;
                    newResultSet.total = 0;
                    newResultSet.totalRecords = 0;
                    newResultSet.statusText = message.returnCode + '-' + message.returnMessage;
                }
                else if (!Ext.isEmpty(message) && message.success == true) {
                    newResultSet.statusText = message.returnCode + ' - ' + message.returnMessage;
                }
            }
            return newResultSet;
        }
    }),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findVeteranByPtcpntId', {
            namespace: 'ser',
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            }
        });
    },

    analyzeResponse: function (response, reader) {
        reader.record = 'vetCorpRecord';

        return reader.read(response);
    }
});
