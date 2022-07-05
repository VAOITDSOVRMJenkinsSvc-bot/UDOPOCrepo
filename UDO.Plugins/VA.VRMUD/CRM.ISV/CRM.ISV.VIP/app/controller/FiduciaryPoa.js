/**
* @author Josh Oliver
* @class VIP.controller.FiduciaryPoa
*
* the controller for the Fiduciary/Poa tab
*/
Ext.define('VIP.controller.FiduciaryPoa', {
    extend: 'Ext.app.Controller',

    stores: ['FiduciaryPoa'],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [{
        ref: 'currentPoa',
        selector: '[xtype="poa.currentpoa"]'
    }, {
        ref: 'pastPoas',
        selector: '[xtype="poa.pastpoas"]'
    }, {
        ref: 'currentFiduciary',
        selector: '[xtype="fiduciary.currentfiduciary"]'
    }, {
        ref: 'pastFiduciaries',
        selector: '[xtype="fiduciary.pastfiduciaries"]'
    }],

    init: function () {
        var me = this;

        me.control({
            'button[action="openaccreditationlink"]': {
                click: me.openAccreditationLink
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            fidpoatabchange: me.onFiduciaryPoaTabChange,
            scope: me
        });
    },

    onFiduciaryPoaTabChange: function (tabName) {
        var me = this,
            fidPoaStore = me.getFiduciaryPoaStore().getAt(0),
            recordCount;

        if (tabName.getXType() == 'poa') {
            if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
                if (Ext.get('id_poa_CurrentPoa_01')) Ext.get('id_poa_CurrentPoa_01').hide();
            }

            recordCount = fidPoaStore ? fidPoaStore.pastPoas().getCount() : 0;
            me.application.fireEvent('setstatisticstext', 'Past POAs', recordCount);
        }
        if (tabName.getXType() == 'fiduciary') {
            recordCount = fidPoaStore ? fidPoaStore.pastFiduciaries().getCount() : 0;
            me.application.fireEvent('setstatisticstext', 'Past Fiduciaries', recordCount);
        }
    },

    onCachedDataLoaded: function () {
        var me = this,
		record = me.getFiduciaryPoaStore().getAt(0);

        var poaData = null;
        if (!Ext.isEmpty(record)) {
            poaData = record;
            var currentPoaRecord = record.currentPoa().getAt(0),
                currentFiduciaryRecord = record.currentFiduciary().getAt(0);

            if (!Ext.isEmpty(currentPoaRecord)) {
                me.getCurrentPoa().loadRecord(currentPoaRecord);
            }

            if (!Ext.isEmpty(currentFiduciaryRecord)) {
                me.getCurrentFiduciary().loadRecord(currentFiduciaryRecord);
            }

            me.getPastPoas().reconfigure(record.pastPoas());
            me.getPastFiduciaries().reconfigure(record.pastFiduciaries());
        }
        me.application.fireEvent('gotpoa', poaData);

    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;

        me.getCurrentPoa().getForm().reset();
        me.getCurrentFiduciary().getForm().reset();

        if (!Ext.isEmpty(selectedPerson.get('fileNumber'))) {
            me.getFiduciaryPoaStore().load({
                filters: [{
                    property: 'fileNumber',
                    value: selectedPerson.get('fileNumber')
                }],
                callback: function (records, operation, success) {
                    var poaData = null;
                    if (!Ext.isEmpty(records)) {
                        poaData = records[0];
                        var currentPoaRecord = poaData.currentPoa().getAt(0),
                            currentFiduciaryRecord = poaData.currentFiduciary().getAt(0);

                        if (!Ext.isEmpty(currentPoaRecord)) {
                            me.getCurrentPoa().loadRecord(currentPoaRecord);
                        }

                        if (!Ext.isEmpty(currentFiduciaryRecord)) {
                            me.getCurrentFiduciary().loadRecord(currentFiduciaryRecord);
                        }

                        me.getPastPoas().reconfigure(poaData.pastPoas());
                        me.getPastFiduciaries().reconfigure(poaData.pastFiduciaries());
                    }
                    me.application.fireEvent('gotpoa', poaData);
                    me.application.fireEvent('webservicecallcomplete', operation, 'FiduciaryPoa');
                },
                scope: me
            });
        }
    },

    openAccreditationLink: function () {
        window.open("http://www.va.gov/ogc/apps/accreditation/index.asp");
    }
});
