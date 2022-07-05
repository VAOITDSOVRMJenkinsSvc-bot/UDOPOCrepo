Ext.define('VIP.util.soap.WebserviceResponseAnalyzer', {
    requires: [
        'Ext.util.MixedCollection'
    ],
    config: {
        scrubFilter: function (record) {
            var recordHasData = false;

            for (var i in record.data) {
                if (!Ext.isEmpty(record.data[i]) && record.data[i] != 0) {
                    recordHasData = true;
                    break;
                }
            }

            return recordHasData;
        },
        customFilters: undefined,
        customReadRecords: undefined
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me;
    },

    analyze: function (response, reader) {
        var me = this,
            scrubFilter = me.getScrubFilter(),
            customFilters = me.getCustomFilters(),
            filteredRecords = Ext.create('Ext.util.MixedCollection');

        me.resultSet = reader.read(response);

        // based on application singletons, analyze responses for common errors and signal completion to the container
        //me.containerIntegration(response, reader, me.resultSet);

        filteredRecords.addAll(me.resultSet.records);

        if (Ext.isFunction(scrubFilter)) {
            filteredRecords = filteredRecords.filterBy(scrubFilter);
        }

        if (!Ext.isEmpty(customFilters)) {
            for (var i in scrubFilters) {
                var customFilter = customFilters[i];

                filteredRecords = filteredRecords.filterBy(customFilter);
            }
        }

        me.resultSet.count = filteredRecords.getCount();
        me.resultSet.total = filteredRecords.getCount();
        me.resultSet.totalRecords = filteredRecords.getCount();
        me.resultSet.records = filteredRecords.getRange();

        if (Ext.isFunction(me.customReadRecords)) {
            me.resultSet = me.customReadRecords(response, me.resultSet, reader);
        }

        return me.resultSet;
    },

    containerIntegration: function (response, reader, resultset) {
        //debugger;
        if (parent && parent._VIPEndOfServiceCall) {
            var map = me.application.responseCacheMap;
            //var xml = me.getCorpStore().getProxy().getReader().rawData.xml;
            //parent._VIPEndOfServiceCall(map.get('Birls'), true, '', '', xml);
        } 
    }
});