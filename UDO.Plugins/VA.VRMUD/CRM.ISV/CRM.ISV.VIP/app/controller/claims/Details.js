/**
* @author Jonas Dawson
* @class VIP.controller.claims.Details
*
* Controller for the claims tabs under the claims tab
*/
Ext.define('VIP.controller.claims.Details', {
    extend: 'Ext.app.Controller',

    requires: [
		'VIP.services.ClaimsProcessing',
		'VIP.soap.envelopes.mapd.document.FindClaimantLetters',
		'VIP.soap.envelopes.mapd.document.FindThirdPartyLetters'
	],
    refs: [
		{
		    ref: 'claims',
		    selector: '[xtype="claims.benefits"]'
		},
		{
		    ref: 'claimDetails',
		    selector: '[xtype="claims.details.claimdetails"]'
		},
		{
		    ref: 'contentions',
		    selector: '[xtype="claims.details.contentions"]'
		},
		{
		    ref: 'evidence',
		    selector: '[xtype="claims.details.evidence"]'
		},
		{
		    ref: 'lifeCycle',
		    selector: '[xtype="claims.details.lifecycle"]'
		},
		{
		    ref: 'status',
		    selector: '[xtype="claims.details.status"]'
		},
		{
		    ref: 'suspenses',
		    selector: '[xtype="claims.details.suspenses"]'
		},
		{
		    ref: 'trackedItems',
		    selector: '[xtype="claims.details.trackeditems"]'
		},
		{
		    ref: 'letters',
		    selector: '[xtype="claims.details.letters"]'
		},
		{
		    ref: 'notes',
		    selector: '[xtype="claims.details.notes"]'
		}
	],

    stores: [
		'Claims',
		'claims.TrackedItems',
		'claims.Letters',
		'claims.ClaimDetail',
		'claims.Suspense',
		'claims.Status',
		'claims.LifeCycle',
		'claims.Contentions',
		'claims.Document'
	],

    init: function () {
        var me = this;

        me.application.on({
            claimrecordsloaded: me.loadClaimDetailRecords,
            claimscacheddataloaded: me.onClaimsCachedDataLoaded,
            scope: me
        });

        me.control({
            '[xtype="claims.details.contentions"] > toolbar > checkbox': {
                change: me.excludeClosedClaims
            },
            '[xtype="claims.details.lifecycle"] > toolbar > button[action="claimprocessingtimes"]': {
                click: me.claimProcessingTimesLifeCycleMenuClick
            },
            '[xtype="claims.details.status"] > toolbar > button[action="claimprocessingtimes"]': {
                click: me.claimProcessingTimesStatusMenuClick
            },
            '[xtype="claims.details.trackeditems"] > toolbar > button[action="opentrackeditemdocument"]': {
                click: me.downloadAttachmentTrackedItems
            },
            '[xtype="claims.details.letters"] > toolbar > button[action="openletterdocument"]': {
                click: me.downloadAttachmentLetters
            },
            '[xtype="claims.details.trackeditems"] > toolbar > button[action="refreshtrackeditems"]': {
                click: me.showAllTrackedItems
            },
            '[xtype="claims.details.letters"] > toolbar > button[action="refreshletters"]': {
                click: me.showAllTrackedItems
            }
        });

        Ext.log('The claims.Details controller has been initialized');
    },

    onClaimsCachedDataLoaded: function (claimsStore) {
        var me = this;
        if (claimsStore.getCount() == 1) {
            claim = claimsStore.getAt(0);
            if (!Ext.isEmpty(claim)) {
                me.getClaimDetails().loadRecord(claimsStore.getAt(0));
                _selectedClaim = claim;
                _claimDetailLoaded = true;
                _claimDetailRecord = claim;
            }
            me.getLifeCycle().reconfigure(me.getClaimsLifeCycleStore());
            me.getSuspenses().reconfigure(me.getClaimsSuspenseStore());
            me.getContentions().reconfigure(me.getClaimsContentionsStore());
            me.getTrackedItems().reconfigure(me.getClaimsTrackedItemsStore());
            me.getLetters().reconfigure(me.getClaimsLettersStore());
            me.getStatus().reconfigure(me.getClaimsStatusStore());
        }
    },

    loadClaimDetailRecords: function (claim, responseXml) {
        var me = this,
			claimId = null;

        if (!Ext.isEmpty(claim)) {
            if (!Ext.isEmpty(claim.get('claimId'))) {
                claimId = claim.get('claimId');
                if (Ext.isEmpty(responseXml)) {
                    me.loadClaimDetail(claim);
                } else {
                    me.getClaimDetails().loadRecord(claim);
                    me.loadLifeCycleSuspenseRecords(claim, responseXml);
                    //tas me.application.fireEvent('claimdetailsloaded', claim);
                }
                me.loadTrackedItems(claimId);
                me.loadContentions(claimId);
                me.loadStatuses(claimId);
            } else {
                me.application.fireEvent('claimdetailsloaded', claim);
            }
        }

    },

    loadClaimDetail: function (claim) {
        var me = this,
            claimId = claim.get('claimId');

        if (!Ext.isEmpty(claimId)) {
            me.getClaimDetails().setLoading(true, true);
            me.getLifeCycle().setLoading(true, true);
            me.getSuspenses().setLoading(true, true);

            me.getLifeCycle().getStore().removeAll();
            me.getSuspenses().getStore().removeAll();

            me.getClaimsClaimDetailStore().load({
                filters: [
					{
					    property: 'benefitClaimId',
					    value: claimId
					}
				],
                callback: function (records, operation, success) {
                    me.getClaimDetails().setLoading(false);
                    me.getLifeCycle().setLoading(false);
                    me.getSuspenses().setLoading(false);

                    if (success && !Ext.isEmpty(records)) {
                        var claimDetailRecord = records[0],
							claimIndex = -1,
							retrievedClaim;

                        if (!Ext.isEmpty(claimDetailRecord)) {
                            //me.getClaimDetails().loadRecord(claimDetailRecord);
                            //me.loadLifeCycleSuspenseRecords(claimDetailRecord, operation.response);

                            _claimDetailLoaded = true;
                            _claimDetailRecord = claimDetailRecord;

                            //Sets the proper claimant participant id on the selected claim.
                            claim.set('participantId', claimDetailRecord.get('participantClaimantID'));
                            claim.commit();
                            me.getClaimDetails().loadRecord(claimDetailRecord);
                            me.loadLifeCycleSuspenseRecords(claimDetailRecord, operation.response);
                            var cdform = me.getClaimDetails().getForm();
                            var lcrExplanation = me.lcrReason(claimDetailRecord);
                            cdform.setValues({reasonText: lcrExplanation });
                          
                            me.application.fireEvent('claimdetailsloaded', claim);
                        }

                        me.application.fireEvent('webservicecallcomplete', operation, 'claims.ClaimDetail');
                    }
                },
                scope: me
            });
        }

    },
    lcrReason: function (claimDetailRecord) {
        var lcrStore = claimDetailRecord.lifeCycleRecords();

        var lcr = lcrStore.getAt(lcrStore.getCount() - 1);
        
        var v = lcr.get('reasonText');
        if (v != '') {
            return v;
        }
        
        return 'N/A';
    },
    loadLifeCycleSuspenseRecords: function (claimDetailRecord, responseXml) {
        var me = this,
			lifeCycleResults = claimDetailRecord.lifeCycleRecords().getProxy().getReader().read(responseXml),
			suspenseResults = claimDetailRecord.suspenseRecords().getProxy().getReader().read(responseXml);

        if (!Ext.isEmpty(lifeCycleResults) && !Ext.isEmpty(lifeCycleResults.records)) {
            claimDetailRecord.lifeCycleRecords().loadRecords(lifeCycleResults.records, { addRecords: false });
            me.getLifeCycle().reconfigure(claimDetailRecord.lifeCycleRecords());
        }

        if (!Ext.isEmpty(suspenseResults) && !Ext.isEmpty(suspenseResults.records)) {
            claimDetailRecord.suspenseRecords().loadRecords(suspenseResults.records, { addRecords: false });
            me.getSuspenses().reconfigure(claimDetailRecord.suspenseRecords());
        }
    },

    loadTrackedItems: function (claimId) {
        var me = this;

        if (!Ext.isEmpty(claimId)) {
            me.getLetters().getStore().removeAll();
            me.getTrackedItems().getStore().removeAll();

            me.getLetters().setLoading(true, true);

            me.getTrackedItems().reconfigure(
				me.getClaimsTrackedItemsStore().load({
				    filters: [
						{
						    property: 'claimId',
						    value: claimId
						}
					],
				    callback: function (records, operation, success) {
				        me.getLetters().setLoading(false);

				        if (success) {
				            if (!Ext.isEmpty(operation) && !Ext.isEmpty(operation.response)) {
				                var lettersStore = me.getClaimsLettersStore(),
								lettersResultSet = lettersStore.getProxy().getReader().read(operation.response);

				                if (!Ext.isEmpty(lettersResultSet) && !Ext.isEmpty(lettersResultSet.records)) {
				                    lettersStore.loadRecords(lettersResultSet.records, { addRecords: false });
				                    me.getLetters().reconfigure(lettersStore);
				                }
				            }
				        }
				        me.application.fireEvent('webservicecallcomplete', operation, 'claims.TrackedItems');
				    },
				    scope: me
				})
			);
        }
    },

    loadContentions: function (claimId) {
        var me = this;
        if (!Ext.isEmpty(claimId)) {
            me.getContentions().getStore().removeAll();
            me.getContentions().reconfigure(
				me.getClaimsContentionsStore().load({
				    filters: [
						{
						    property: 'claimId',
						    value: claimId
						}
				],
				    callback: function (records, operation, success) {
				        if (success && !Ext.isEmpty(records)) {
				            for (var i in records) {
				                var contention = records[i];

				                contention.specialIssues().getProxy().data = operation.response;
				                contention.specialIssues().load();

				            }
				        }
				        me.application.fireEvent('webservicecallcomplete', operation, 'claims.Contentions');
				    },
				    scope: me
				})
			);
        }
    },

    loadStatuses: function (claimId) {
        var me = this;

        if (!Ext.isEmpty(claimId)) {
            me.getStatus().getStore().removeAll();
            me.getStatus().reconfigure(
				me.getClaimsStatusStore().load({
				    filters: [
						{
						    property: 'claimId',
						    value: claimId
						}
					],
				    callback: function (records, operation, success) {
				        if (success && !Ext.isEmpty(records)) {
				            calculateDaysInStatus();
				        }

				        me.application.fireEvent('webservicecallcomplete', operation, 'claims.Status');

				        function calculateDaysInStatus(statusRecords) {
				            var lastStatusIndex = me.getClaimsStatusStore().getCount() - 1,
								oneDay = 1000 * 60 * 60 * 24;

				            me.getClaimsStatusStore().each(function (status) {
				                var changedDate = status.get('changedDate'),
									daysInSatus = 0,
									nextChangedDate = new Date();

				                if (status.index != lastStatusIndex) {
				                    nextChangedDate = me.getClaimsStatusStore().getAt(status.index + 1).get('changedDate');
				                    daysInStatus = Math.ceil((nextChangedDate.getTime() - changedDate.getTime()) / (oneDay));
				                }
				                else {
				                    daysInStatus = Math.ceil((nextChangedDate.getTime() - changedDate.getTime()) / (oneDay));
				                }
				                status.set('daysInStatus', daysInStatus);
				                status.commit();
				            });
				        }
				    },
				    scope: me
				})
			);
        }
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

    claimProcessingTimesLifeCycleMenuClick: function () {
        var me = this;

        if (!me.getLifeCycle().getSelectionModel().hasSelection()) { Ext.Msg.alert('Select Life Cycle', 'Please select a life cycle.'); return; }
        var lifeCycle = me.getLifeCycle().getSelectionModel().getSelection()[0];

        var sojCode = lifeCycle.get('actionStationNumber');

        if (!sojCode) {
            Ext.Msg.alert('Average Claim Processing Times', 'No SOJ Id was found, processing times can not be found.');
        } else {
            var claimsProc = Ext.create('VIP.services.ClaimsProcessing');
            claimsProc.DisplayClaimProcessingTimes(sojCode, 'cycle');
        }
    },

    claimProcessingTimesStatusMenuClick: function () {
        var me = this;

        if (!me.getStatus().getSelectionModel().hasSelection()) { Ext.Msg.alert('Select Status', 'Please select a status.'); return; }
        var status = me.getStatus().getSelectionModel().getSelection()[0];

        var sojCode = status.get('actionLocationId');

        if (!sojCode) {
            Ext.Msg.alert('Average Claim Processing Times', 'No SOJ Id was found, processing times can not be found.');
        } else {
            var claimsProc = Ext.create('VIP.services.ClaimsProcessing');
            claimsProc.DisplayClaimProcessingTimes(sojCode, 'status');
        }
    },

    showAllTrackedItems: function () {
        var me = this,
			claimsGrid = me.getClaims();

        if (!Ext.isEmpty(claimsGrid) && claimsGrid.getSelectionModel().hasSelection()) {
            claim = claimsGrid.getSelectionModel().getSelection()[0];
            if (!Ext.isEmpty(claim)) {
                me.loadTrackedItems(claim.get('claimId'));
            }
        }
        else {
            Ext.Msg.alert('Select Claim', 'Please select a claim to perform this action.');
            return;
        }
    }
});