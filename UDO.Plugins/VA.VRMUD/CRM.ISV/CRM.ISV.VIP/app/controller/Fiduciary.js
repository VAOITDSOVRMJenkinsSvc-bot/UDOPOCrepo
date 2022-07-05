/**
* @author Ivan Yurisevic
* @class VIP.controller.Fiduciary
*
* The controller for the Fiduciary tab
*/
Ext.define('VIP.controller.Fiduciary', {
	extend: 'Ext.app.Controller',

	stores: ['fiduciary.CurrentFiduciary', 'fiduciary.PastFiduciaries'], // EJ - Build

	refs: [{
		ref: 'currentFiduciary',
		selector: '[xtype="fiduciary.currentfiduciary"]'
	}, {
		ref: 'pastFiduciaries',
		selector: '[xtype="fiduciary.pastfiduciaries"]'
	}],

	init: function () {
		var me = this;

		me.application.on({
			individualidentified: me.onIndividualIdentified,
			cacheddataloaded: me.onCachedDataLoaded,
			scope: me
		});
	},

	onCachedDataLoaded: function () {
		var me = this,
		record = me.getCurrentFiduciaryStore().getAt(0);

		if (!Ext.isEmpty(record)) {
			me.getCurrentFiduciary().loadRecord(record);
		}

	},

	onIndividualIdentified: function (selectedPerson) {
		var me = this;
		me.getCurrentFiduciary().getForm().reset();

		if (!Ext.isEmpty(selectedPerson.get('fileNumber'))) {
			me.getCurrentFiduciaryStore().load({
				filters: [{
					property: 'fileNumber',
					value: selectedPerson.get('fileNumber')
				}],
				callback: function (records, operation, success) {
					if (!Ext.isEmpty(records)) {
						me.getCurrentFiduciary().loadRecord(records[0]);
					}
					me.application.fireEvent('webservicecallcomplete', operation, 'CurrentFiduciary');
				},
				scope: me
			});

			me.getFiduciaryPastFiduciariesStore().load({
				filters: [{
					property: 'fileNumber',
					value: selectedPerson.get('fileNumber')
				}],
				callback: function (records, operation, success) {
					//debugger
					me.application.fireEvent('webservicecallcomplete', operation, 'fiduciary.PastFiduciaries');
				},
				scope: me
			});
		}
	}
});
