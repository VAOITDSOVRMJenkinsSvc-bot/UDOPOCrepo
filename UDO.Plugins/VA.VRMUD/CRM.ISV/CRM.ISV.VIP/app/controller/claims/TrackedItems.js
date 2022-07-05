/**
* @author Josh Oliver
* @class VIP.controller.claims.TrackedItems
*
* Controller for the claims tabs under the claims tab
*/
Ext.define('VIP.controller.claims.TrackedItems', {
	extend: 'Ext.app.Controller',
	refs: [
		{
			ref: 'trackedItems',
			selector: '[xtype="claims.details.trackeditems"]'
		},
		{
			ref: 'letters',
			selector: '[xtype="claims.details.letters"]'
		}
	],

	stores: [
        'claims.TrackedItems',
        'claims.Letters',
        'claims.Document'
	],

	init: function () {
		var me = this;

		me.application.on({
			claimloaded: me.loadTrackedItems,
			scope: me
		});

		me.control({
			'[xtype="claims.details.contentions"] > toolbar > checkbox': {
				change: me.excludeClosedClaims
			},
			'[xtype="claims.details.trackeditems"] > toolbar > button[action="opentrackeditemdocument"]': {
				click: me.downloadAttachmentTrackedItems
			},
			'[xtype="claims.details.letters"] > toolbar > button[action="openletterdocument"]': {
				click: me.downloadAttachmentLetters
			}
		});

		Ext.log('The claims.TrackedItems controller has been initialized');
	},

	excludeClosedClaims: function (checkbox, newValue, oldValue) {
		var me = this;

		Ext.Msg.alert('Exclude Closed Claims Checkbox',
					  'The closed claims checkbox has been changed from ' +
					  oldValue + ' to ' + newValue + '. ' +
					  'No further functionality has yet been added.'
		);
	},

	downloadAttachmentTrackedItems: function () {
		var me = this;

		if (!me.getTrackedItems().getSelectionModel().hasSelection()) { Ext.Msg.alert('Select Tracked Item', 'Please select a tracked item.'); return; }

		try {
			var claim = me.getClaims().getSelectionModel().getSelection()[0];
			var trackedItem = me.getTrackedItems().getSelectionModel().getSelection()[0];

			if (trackedItem.get('documentId') == '' || trackedItem.get('documentId') == undefined) { Ext.Msg.alert('No Document', 'No document exists for this tracked item.'); return; }

			var dvlpmtTc = trackedItem.get('developmentTypeCode');
			if (dvlpmtTc) dvlpmtTc = dvlpmtTc.toUpperCase();
			if (dvlpmtTc != 'CLMNTRQST' && dvlpmtTc != '3RDPRTYRQST') {
				Ext.Msg.alert('No Document', 'No document can be retrieved for this tracked item. Type Code: ' + dvlpmtTc); return;
			}

			var isClm = (dvlpmtTc == 'CLMNTRQST');

			me.getClaimsDocumentStore().setProxy({
				type: 'soap',
				headers: {
					"SOAPAction": "",
					"Content-Type": "text/xml; charset=utf-8"
				},
				reader: {
					type: 'xml',
					record: 'letters'
				},
				envelopes: {
					read: (isClm ? 'VIP.soap.envelopes.mapd.document.FindClaimantLetters' : 'VIP.soap.envelopes.mapd.document.FindThirdPartyLetters')
				}
			});

			me.getClaimsDocumentStore().load({
				filters: [{
					property: 'documentId',
					value: trackedItem.get('documentId')
				}],
				callback: function (records, operation, success) {

					if (!records[0].get('letterText')) { Ext.Msg.alert('No Document Found', 'No document was found in the web service response. Process will terminate.'); return; }

					var encryptedLetter = records[0].get('letterText');
					var letterTitle;

					if (claim && claim != undefined) {
						letterTitle = 'ClaimantLetter-' + claim.get('claimId') + '-' + trackedItem.get('shortName');
					}
					else {
						letterTitle = 'ClaimantLetter-' + trackedItem.get('shortName');
					}

					var decompressedLetter = JXG.decompress(encryptedLetter);
					me.postwith(parent.Xrm.Page.context.getServerUrl().replace(parent.Xrm.Page.context.getOrgUniqueName(), '') + 'isv/download/download.aspx', { letter: decompressedLetter, title: letterTitle, fileExtension: 'rtf' });

				},
				scope: me
			});

		}
		catch (e) {
			Ext.Msg.alert('Error', 'There was an error while trying to load. Process will terminate.'); return;
		}

	},

	downloadAttachmentLetters: function () {
		var me = this;

		if (!me.getLetters().getSelectionModel().getSelection()) { Ext.Msg.alert('Select Letter', 'Please select a letter.'); return; }

		try {
			var claim = me.getClaims().getSelectionModel().getSelection()[0];
			var letter = me.getLetters().getSelectionModel().getSelection()[0];

			if (letter.get('documentId') == '' || letter.get('documentId') == undefined) { Ext.Msg.alert('No Document', 'No document exists for this letter.'); return; }

			var dvlpmtTc = letter.get('developmentTypeCode');
			if (dvlpmtTc) dvlpmtTc = dvlpmtTc.toUpperCase();
			if (dvlpmtTc != 'CLMNTRQST' && dvlpmtTc != '3RDPRTYRQST') {
				Ext.Msg.alert('No Document', 'No document can be retrieved for this tracked item. Type Code: ' + dvlpmtTc); return;
			}

			var isClm = (dvlpmtTc == 'CLMNTRQST');

			me.getClaimsDocumentStore().setProxy({
				type: 'soap',
				headers: {
					"SOAPAction": "",
					"Content-Type": "text/xml; charset=utf-8"
				},
				reader: {
					type: 'xml',
					record: 'letters'
				},
				envelopes: {
					read: (isClm ? 'VIP.soap.envelopes.mapd.document.FindClaimantLetters' : 'VIP.soap.envelopes.mapd.document.FindThirdPartyLetters')
				}
			});

			me.getClaimsDocumentStore().load({
				filters: [{
					property: 'documentId',
					value: letter.get('documentId')
				}],
				proxy: {
					type: 'soap',
					headers: {
						"SOAPAction": "",
						"Content-Type": "text/xml; charset=utf-8"
					},
					reader: {
						type: 'xml',
						record: 'letters'
					},
					envelopes: {
						read: (isClm ? 'VIP.soap.envelopes.mapd.document.FindClaimantLetters' : 'VIP.soap.envelopes.mapd.document.FindThirdPartyLetters')
					}
				},
				callback: function (records, operation, success) {

					if (!records[0].get('letterText')) { Ext.Msg.alert('No Document Found', 'No document was found in the web service response. Process will terminate.'); return; }

					var encryptedLetter = records[0].get('letterText');
					var letterTitle;

					if (claim && claim != undefined) {
						letterTitle = 'ClaimantLetter-' + claim.get('claimId') + '-' + letter.get('name');
					}
					else {
						letterTitle = 'ClaimantLetter-' + letter.get('name');
					}

					var decompressedLetter = JXG.decompress(encryptedLetter);
					me.postwith(parent.Xrm.Page.context.getServerUrl().replace(parent.Xrm.Page.context.getOrgUniqueName(), '') + 'isv/download/download.aspx', { letter: decompressedLetter, title: letterTitle, fileExtension: 'rtf' });

				},
				scope: me
			});

		}
		catch (e) {
			Ext.Msg.alert('Error', 'There was an error while trying to load. Process will terminate.'); return;
		}
	},

	postwith: function (to, p) {
		var downloadWindow = window;

		var myForm = downloadWindow.document.createElement("form");
		myForm.method = 'post';
		myForm.action = to;
		myForm.target = '_blank';
		for (var k in p) {
			var myInput = downloadWindow.document.createElement("input");
			myInput.setAttribute("name", k);
			myInput.setAttribute("value", p[k]);
			myForm.appendChild(myInput);
		}
		downloadWindow.document.body.appendChild(myForm);
		myForm.submit();
		downloadWindow.document.body.removeChild(myForm);
	},

	loadTrackedItems: function (claimId) {

	}

});