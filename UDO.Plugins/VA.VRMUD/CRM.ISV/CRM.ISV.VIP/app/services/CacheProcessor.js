/**
* @author Dmitri Riz
* @class VIP.services.CacheProcessor
*
* Loader for cached data
*/
Ext.define('VIP.services.CacheProcessor', {
    config: {
        cacheMap: undefined,
        cacheDataCollection: undefined
    },
    requires: [
        'VIP.model.Awards'
    ],
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me;
    },

    save: function (storeName, xmlText, append) {
        var me = this,
			cacheMap = me.getCacheMap(),
            value = cacheMap.get(storeName),
            attributeName;

        if (cacheMap.containsKey(storeName)) {
            if (Ext.isString(value)) {
                attributeName = value;
            }
            if (Ext.isObject(attributeName)) {
                attributeName = value.root;
            }

            parent.Xrm.Page.data.entity.attributes.get(attributeName).setValue(xmlText);

            return true;
        }
        else {
            return false;
        }
    },

    loadAll: function () {
        var me = this,
			cacheMap = me.getCacheMap(),
			cacheData = me.getCacheDataCollection();

        var processed = new Ext.util.HashMap();

        cacheMap.each(function (attributeName, index, length) {
            // if this cache already processed, go to next one
            // process allratings
            var key = attributeName;
            if (key == 'va_findratingdataresponse' || key == 'va_findpaymenthistoryresponse' || key == 'va_generalinformationresponsebypid' || key == 'va_findfiduciarypoaresponse' ||
                key == 'va_finddenialsresponse' || key == 'va_findbenefitdetailresponse') {
                key += index.toString();
            }

            if (!processed.containsKey(key)) {
                processed.add(key, index);

                var storeName = cacheMap.keys[index],
                    altModel = null,
			        xmlData = cacheData[attributeName];

                if (!Ext.isEmpty(xmlData)) {
                    var countNode,
                        count = 0;

                    if (storeName == 'Claims') {
                        //debugger;
                        countNode = xmlData.selectSingleNode('//numberOfBenefitClaimRecords') || xmlData.selectSingleNode('//participantRecord/numberOfRecords');
                        count = !Ext.isEmpty(countNode) ? parseInt(countNode.text) : 0;
                        if (count == 1) {
                            altModel = Ext.create('VIP.model.claims.ClaimDetail');
                            if (!Ext.isEmpty(altModel)) {
                                altModel.setProxy({
                                    type: 'memory',
                                    reader: {
                                        type: 'xml',
                                        record: 'participantRecord'
                                    }
                                });
                            }
                        }
                    }

                    if (storeName == 'Awards') {
                        //debugger;
                        countNode = xmlData.selectSingleNode('//numberOfAwardBenes');
                        count = !Ext.isEmpty(countNode) ? countNode.text : 0;
                        if (count == 1) { altModel = Ext.create('VIP.model.Awards'); }
                    }
                }

                //if (storeName == 'pathways.Appointment') { debugger; }

                var store = Ext.StoreManager.get(storeName);
                if (Ext.isEmpty(store)) {
                    //debugger;
                    // no store!!!
                    return;
                }
                var model = null;
                if (!Ext.isEmpty(altModel)) {
                    model = altModel;
                    store.model = model;
                }
                else {
                    model = store.model;
                }

                var reader = model.getProxy().getReader();

                if (Ext.isEmpty(model)) {
                    //debugger;
                }

                if (!Ext.isEmpty(xmlData)) {
                    var resultSet = null;
                    try { resultSet = reader.read(xmlData); } 
                    catch (re) { alert('Error while loading store ' + storeName + ': ' + re.description); }

                    if (resultSet) { store.loadRecords(resultSet.records, { addRecords: false }); }
                }

                else {
                    // awards and claims top level
                    //debugger;
                }

            }
        }, me);


    },

    reshapeXml: function (xmlData) {
        return xmlData;
    }
});