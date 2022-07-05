/**
* @author Ivan Yurisevic
* @class VIP.controller.Poa
*
* the controller for the Poa tab
*/
Ext.define('VIP.controller.Poa', {
	extend: 'Ext.app.Controller',

	stores: ['poa.CurrentPoa', 'poa.PastPoas'], // EJ - Build

	refs: [{
		ref: 'currentPoa',
		selector: '[xtype="poa.currentpoa"]'
	}, {
		ref: 'pastPoas',
		selector: '[xtype="poa.pastpoas"]'
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
			scope: me
		});
	},

	onCachedDataLoaded: function () {
		var me = this,
		record = me.getCurrentPoaStore().getAt(0);

		if (!Ext.isEmpty(record)) {
			me.getCurrentPoa().loadRecord(record);
		}

	},

	onIndividualIdentified: function (selectedPerson) {
		var me = this;
		me.getCurrentPoa().getForm().reset();

		if (!Ext.isEmpty(selectedPerson.get('fileNumber'))) {
			me.getCurrentPoaStore().load({
				filters: [{
					property: 'fileNumber',
					value: selectedPerson.get('fileNumber')
				}],
				callback: function (records, operation, success) {
					if (!Ext.isEmpty(records)) {
						me.getCurrentPoa().loadRecord(poaData);
					}

					me.application.fireEvent('webservicecallcomplete', operation, 'CurrentPoa');
				},
				scope: me
			});

			me.getPoaPastPoasStore().load({
				filters: [{
					property: 'fileNumber',
					value: selectedPerson.get('fileNumber')
				}],
				callback: function (records, operation, success) {
					me.application.fireEvent('webservicecallcomplete', operation, 'poa.PastPoas');          // TODO: need to update on the form?
				},
				scope: me
			});
		}
	},

	openAccreditationLink: function () {
		window.open("http://www.va.gov/ogc/apps/accreditation/index.asp");
	}
});
