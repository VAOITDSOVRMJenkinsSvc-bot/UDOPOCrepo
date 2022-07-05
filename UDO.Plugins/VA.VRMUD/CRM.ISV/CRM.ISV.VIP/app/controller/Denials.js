/**
* @author Ivan Yurisevic
* @class VIP.controller.Denials
*
* The controller for the Fiduciary tab
*/

Ext.define('VIP.controller.Denials', {
    extend: 'Ext.app.Controller',
    stores: [
        'Denial',
        'denials.FullDenialReason'
    ],
    refs: [{
        ref: 'denialsGrid',
        selector: '[xtype="denials.denialslist"]'
    }, {
        ref: 'denialInfo',
        selector: '[xtype="denials.denialdetail"]'
    }, {
        ref: 'fullReasonField',
        selector: '[xtype="denials.denialdetail"] > container > displayfield[id="fullreason"]'
    }
    ],

    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            triggerdenialtabchange: me.onDenialsTabChange,
            scope: me
        });

        me.control({
            '[xtype="denials.denialdetail"] > container > button[action="loadfullreason"]':
                { click: me.loadFullReason },
            '[xtype="denials.denialslist"]': { selectionchange: me.onDenialSelection }
        });
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        if (!Ext.isEmpty(selectedPerson.get('participantId'))) {
            me.getDenialStore().load({
                filters: [{
                    property: 'ptcpntId',
                    value: selectedPerson.get('participantId')
                }],
                callback: function (records, operation, success) {
                    me.application.fireEvent('webservicecallcomplete', operation, 'Denial');
                },
                scope: me
            });
        }
    },

    onDenialsTabChange: function () {
        var me = this,
            denialCount = me.getDenialsGrid().getStore().getCount();

        me.application.fireEvent('setstatisticstext', 'Denials', denialCount);
    },

    loadFullReason: function () {
        var me = this,
            rbaId = null,
            selection = me.getDenialsGrid().getSelectionModel().getSelection()[0];

        if (!Ext.isEmpty(selection)) {
            rbaId = selection.get('rbaId');
            if (!Ext.isEmpty(rbaId)) { rbaId = rbaId.replace(/\s+$/, ''); }
        }
        else {
            Ext.Msg.alert(
            'Load Full Reason - Denials',
            'No Denial selected. Please select an item from the grid and try again.'
            );
            return;
        }

        if (!Ext.isEmpty(rbaId)) {
            me.getDenialsFullDenialReasonStore().load({
                filters: [{
                    property: 'rbaIssueId',
                    value: rbaId
                }],
                callback: function (records, operation, success) {
                    if (!Ext.isEmpty(records)) {
                        var reason = records[0].get('reason');
                        if (!Ext.isEmpty(reason)) {
                            reason = reason.replace(/\s+$/, '');
                            me.getFullReasonField().setValue(reason);
                        }
                    }
                    me.application.fireEvent('webservicecallcomplete', operation, 'denials.FullDenialReason');
                },
                scope: me
            });
        }
        else {
            Ext.Msg.alert(
                'Load Full Reason - Denials',
                'Full Reason not loaded.  Record does not contain a RBA Id.'
            );
            return;
        }
    },

    onDenialSelection: function (selectionModel, selection, options) {
        var me = this;
        me.getDenialInfo().loadRecord(selection[0]);
    },

    onCachedDataLoaded: function () {

    }

});
