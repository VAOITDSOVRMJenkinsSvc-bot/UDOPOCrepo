/**
* @author Jonas Dawson
* @class VIP.view.claims.Benefits
*
* The view for the main claims grid
*/
Ext.define('VIP.view.claims.Benefits', {
	extend: 'Ext.grid.Panel',
	alias: 'widget.claims.benefits',
	requires: [
		'VIP.view.ServiceRequest',
		'VIP.view.claims.Epc'
	],
	initComponent: function () {
		var me = this;

		me.columns = {
			defaults: {
			//flex: 1
		},
		items: [
				{ header: 'Claim Id', dataIndex: 'claimId', width: 55 },
				{ header: 'Date of Claim', dataIndex: 'claimReceiveDate', xtype: 'datecolumn', format: 'm/d/Y', width: 90 },
				{ header: 'Claim Description', dataIndex: 'claimTypeName', width: 250 },
				{ header: 'Claim Status', dataIndex: 'statusTypeCode', width: 70 },
				{ header: 'Last Action Date', dataIndex: 'lastActionDate', xtype: 'datecolumn', format: 'm/d/Y', width: 100 },
				{ header: 'EPC', dataIndex: 'endProductTypeCode', width: 60 },
				{ header: 'Organization Person Title', dataIndex: 'organizationName', width: 180 },
                { header: 'Organization Name', dataIndex: 'organizationTitleTypeName', width: 250 },
				{ header: 'Person/Org Indicator', dataIndex: 'personOrOrganizationIndicator', width: 90 },
				{ header: 'Program Type Code', dataIndex: 'programTypeCode', width: 120 },
				{ header: 'Payee Cd', dataIndex: 'payeeTypeCode', width: 70 },
				{ header: 'Participant Id', dataIndex: 'participantId', width: 90 }
			]
	};

	me.dockedItems = [{
		xtype: 'toolbar',
		items: [
				{
					xtype: 'button',
					text: 'Refresh',
					tooltip: '',
					iconCls: 'icon-refresh',
					action: 'reloadAllClaims',
					id: 'id_claims_Benefits_01'
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_02'
				},
				{
					xtype: 'button',
					text: 'Script',
					tooltip: '',
					iconCls: 'icon-script',
					action: 'displayclaimscript',
					id: 'id_claims_Benefits_03'
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_04'
				},
				{
					xtype: 'servicerequest',
					text: 'Create Service Request',
					tooltip: '',
					//iconCls: 'icon-script',
					action: 'createservicerequest',
					id: 'id_claims_Benefits_05'
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_06'
				},
				{
					xtype: 'button',
					text: 'Claim Processing Times',
					tooltip: '',
					iconCls: 'icon-clock',
					action: 'claimprocessingtimes',
					id: 'id_claims_Benefits_07',
                    hidden: true
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_08',
                    hidden: true
				},
				{
					xtype: 'button',
					text: 'Initiate CADD',
					tooltip: 'Initiates Change of Address action',
					iconCls: 'icon-addrChange',
					action: 'cad',
					id: 'id_claims_Benefits_09'
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_10'
				},
				{
					xtype: 'button',
					text: 'View Claimant',
					tooltip: 'View selected Claimant in a new window.',
					iconCls: 'icon-contact',
					action: 'viewclaimantcontact',
					id: 'id_claims_Benefits_11'
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_12'
				},
                {
                    xtype: 'button',
                    text: 'Create VAI',
                    tooltip: 'Creates VAI for Selected Claim',
                    iconCls: 'icon-vai',
                    action: 'createvai',
                    hidden: true,
                    id: 'id_claims_Benefits_13'
                },
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_14'
				},
				{
					xtype: 'claims.epc',
					id: 'id_claims_Benefits_15'
				},
				{
					xtype: 'tbseparator',
					id: 'id_claims_Benefits_16'
				},

				{
					xtype: 'tbfill',
					id: 'id_claims_Benefits_17'
				},
				{
					xtype: 'tbtext',
					notificationType: 'claimcount',
					text: 'Claims: 0',
					id: 'id_claims_Benefits_18'
				}
			]
	}
		];

	me.callParent();
}

});
