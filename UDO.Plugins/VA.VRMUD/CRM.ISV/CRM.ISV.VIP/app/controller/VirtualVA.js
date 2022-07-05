/**
* @author Ivan Yurisevic
* @class VIP.controller.VirtualVA
*
* The controller for the Virtual VA tab
*/

Ext.define('VIP.controller.VirtualVA', {
    extend: 'Ext.app.Controller',

    requires: [
		'VIP.services.CrmEnvironment',
		'VIP.services.DocumentGenerator'
	],

    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },

    config: {
        vvaSearchValue: '',
        vvaUserName: '',
        vvaPassword: '',
        dacUrl: ''
    },

    stores: [
		'virtualva.DocumentRecord',
		'virtualva.DocumentContent'
	],

    refs: [
		{
		    ref: 'documentList',
		    selector: '[xtype="virtualva.documentlist"]'
		},
		{
		    ref: 'documentSearchValue',
		    selector: '[id="vvaSearchValue"]'
		},
		{
		    ref: 'documentCounter',
		    selector: '[xtype="virtualva.documentlist"] > toolbar > tbtext[notificationType="documentcount"]'
		},
		{
		    ref: 'vvaTab',
		    selector: '[xtype="virtualva"]'
		}
	],

    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            getDocumentListInvoked: me.onGetDocumentListInvoked,
            triggervirtualvatabchange: me.onVVATabChange,
            scope: me
        });

        me.control({
            '[xtype="virtualva.documentlist"]': {
                itemdblclick: me.onOpenDocumentContent
            },
            '[xtype="virtualva.documentlist"] > toolbar > button[action="openDocumentContent"]': {
                click: me.onOpenDocumentContent
            },
            '[xtype="virtualva.documentlist"] > toolbar > button[action="searchDocumentList"]': {
                click: me.onSearchDocumentList
            }
        });
    },

    onCachedDataLoaded: function () {
        var me = this, claimId = null,
			documentRecord = me.getVirtualvaDocumentRecordStore().getAt(0);

        if (!Ext.isEmpty(documentRecord)) {
            claimId = documentRecord.get('claimId');
        }
        else if (parent && parent.Xrm != undefined && parent.Xrm.Page.getAttribute) {
            var attr = parent.Xrm.Page.getAttribute('va_ssn');
            if (attr) {
                claimId = attr.getValue();
            }
        }
        if (!Ext.isEmpty(claimId)) {
            me.getDocumentSearchValue().setValue(claimId);
            me.setVvaSearchValue(claimId);
        }
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this,
			searchValue = selectedPerson.get('fileNumber'),
			documentSearchValueField = me.getDocumentSearchValue();

        if (Ext.isEmpty(searchValue)) { searchValue = selectedPerson.get('ssn'); }

        if (!Ext.isEmpty(documentSearchValueField)) {
            documentSearchValueField.setValue(searchValue);
        }

        me.setVvaSearchValue(searchValue);
        me.updateDacUrl();
        //me.searchDocumentList(me.getVvaSearchValue());
    },

    onGetDocumentListInvoked: function (vvaSearchValue) {
        var me = this,
			vvaTab = me.getVvaTab();

        if (!Ext.isEmpty(vvaSearchValue)) {
            me.getDocumentSearchValue().setValue(vvaSearchValue);

            vvaTab.show();

            me.searchDocumentList(vvaSearchValue);
        }

    },

    onOpenDocumentContent: function (button, event, eventOptions) {
        var me = this,
			selectedDocument = me.getDocumentList().getSelectionModel().hasSelection() ? me.getDocumentList().getSelectionModel().getSelection()[0] : null;

        if (Ext.isEmpty(selectedDocument)) {
            Ext.Msg.alert('Open Document',
				'Please select a document record from the grid and try again.');
            return;
        }

        me.updateDocumentSearchCriteria();

        me.sendDocumentContentRequest(selectedDocument);
    },

    getPcrId: function () {
        var me = this;
        return Ext.create('VIP.services.CrmEnvironment').GetCurrentCrmUser().get('userName');
    },

    searchDocumentContent: function (selectedDocument) {
        var me = this;

        if (!Ext.isEmpty(selectedDocument)) {
            me.getDocumentList().setLoading(true, true);
            me.getVirtualvaDocumentContentStore().load({
                filters: [
					{
					    property: 'fnDcmntId',
					    value: selectedDocument.get('fileNetDocumentId')
					},
					{
					    property: 'fnDcmntSource',
					    value: selectedDocument.get('fileNetDocumentSource')
					},
					{
					    property: 'dcmntFormatCd',
					    value: selectedDocument.get('documentFormatCode')
					},
					{
					    property: 'jro',
					    value: selectedDocument.get('jro')
					},
					{
					    property: 'userId',
					    value: me.getPcrId()
					},
					{
					    property: 'vvaUserName',
					    value: me.getVvaUserName()
					},
					{
					    property: 'vvaPassword',
					    value: me.getVvaPassword()
					}
				],
                callback: function (records, operation, success) {
                    me.getDocumentList().setLoading(false);

                    if (success && !Ext.isEmpty(records)) {
                        var documentContentRecord = records[0];
                        if (Ext.isEmpty(documentContentRecord.get('documentRecordId'))) {
                            documentContentRecord.set('documentRecordId', selectedDocument.get('fileNetDocumentId'));
                            documentContentRecord.commit();
                        }
                        me.openDocument(documentContentRecord);
                    }
                },
                scope: me
            });
        }
    },

    onSearchDocumentList: function (button, event, eventOptions) {
        var me = this,
			vvaSearchValue = me.getDocumentSearchValue().getValue();

        if (Ext.isEmpty(vvaSearchValue)) { vvaSearchValue = me.getVvaSearchValue(); }

        me.searchDocumentList(vvaSearchValue);
    },

    onVVATabChange: function () {
        var me = this,
        vvaSearchValue = me.getDocumentSearchValue().value;

        //if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
        //    if (Ext.get('vvaSearchValue')) Ext.get('vvaSearchValue').hide();
        //    //if (Ext.get('id_virtualva_DocumentList_02')) Ext.get('id_virtualva_DocumentList_02').hide();
        //    if (Ext.get('id_virtualva_DocumentList_03')) Ext.get('id_virtualva_DocumentList_03').hide();
        //    if (Ext.get('id_virtualva_DocumentList_04')) Ext.get('id_virtualva_DocumentList_04').hide();
        //    if (Ext.get('id_virtualva_DocumentList_05')) Ext.get('id_virtualva_DocumentList_05').hide();
        //    if (Ext.get('id_virtualva_DocumentList_06')) Ext.get('id_virtualva_DocumentList_06').hide();

        //    $("#id_virtualva_DocumentList_02").css('left', 0);
        //}

        if (Ext.isEmpty(vvaSearchValue)) { vvaSearchValue = me.getVvaSearchValue(); }

        if (me.getDocumentList().getStore().getCount() == 0) {
            me.searchDocumentList(vvaSearchValue);
        }
    },

    searchDocumentList: function (vvaSearchValue) {
        var me = this;

        if (Ext.isEmpty(vvaSearchValue)) {
            Ext.Msg.alert('Search Document Records',
				'Please enter either a SSN, file number, or claim number in the search value text box and try again.');
            return;
        }

        me.updateDocumentSearchCriteria();

        me.getVirtualvaDocumentRecordStore().load({
            filters: [
				{
				    property: 'claimNbr',
				    value: vvaSearchValue
				},
				{
				    property: 'vvaUserName',
				    value: me.getVvaUserName()
				},
				{
				    property: 'vvaPassword',
				    value: me.getVvaPassword()
				}
			],
            callback: function (records, operation, success) {
                me.updateDocumentCount();
                me.application.fireEvent('webservicecallcomplete', operation, 'virtualva.DocumentRecord');
            },
            scope: me
        });
    },

    updateDocumentCount: function () {
        var me = this,
			documentCount = me.getDocumentList().getStore().getCount();

        me.getDocumentCounter().setText('Documents: ' + documentCount);

    },

    updateDocumentSearchCriteria: function () {
        var me = this,
			vvaUserName = me.getVvaUserName(),
			vvaPassword = me.getVvaPassword();

        if (Ext.isEmpty(vvaUserName)) {
            me.setVvaUserName(!Ext.isEmpty(me.application.crmEnvironment) ? me.application.crmEnvironment.get('VVAUser') : 'TEST');
        }

        if (Ext.isEmpty(vvaPassword)) {
            me.setVvaPassword(!Ext.isEmpty(me.application.crmEnvironment) ? me.application.crmEnvironment.get('VVAPassword') : 'YYYYY');
        }

        me.setVvaSearchValue(me.getDocumentSearchValue().getValue());
    },

    updateDacUrl: function () {
        var me = this,
			dacUrl = me.getDacUrl();

        if (Ext.isEmpty(dacUrl)) {
            me.setDacUrl(!Ext.isEmpty(me.application.crmEnvironment) ? me.application.crmEnvironment.get('globalDAC') : 'http://10.153.95.73/RedirectSvc.asmx');
        }
    },

    openDocument: function (documentRecord) {
        var me = this,
			documentContent = documentRecord.get('content'),
			documentTitle = !Ext.isEmpty(documentRecord.get('documentTitle')) ? documentRecord.get('documentTitle') : documentRecord.get('documentRecordId'),
			documentMimeType = documentRecord.get('mimeType'),
			documentFileExtension = extractFileExtension(documentMimeType),
			documentGenerator = Ext.create('VIP.services.DocumentGenerator', {
			    content: documentContent,
			    title: documentTitle,
			    mimeType: documentMimeType,
			    fileExtension: documentFileExtension
			});

        documentGenerator.openDocument();

        function extractFileExtension(mimeType) {
            var fileExtension = '';

            if (!Ext.isEmpty(mimeType)) {
                fileExtension = mimeType.substr(mimeType.lastIndexOf('/') + 1);
            }

            return fileExtension;
        }
    },

    sendDocumentContentRequest: function (selectedDocument) {
        var me = this,
			env = '<?xml version="1.0" encoding="utf-8"?>'
				+ '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">'
				+ '<soap:Body><DownloadVVADocument xmlns="http://tempuri.org/">'
				+ '<vvaServiceUri>' + (me.getEnvironment() ? me.getEnvironment().get('VVABase') : 'https://vbaphi5dopp.vba.va.gov:7002/')
				+ 'VABFI/services/vva' + '</vvaServiceUri>'
				+ '<userName>' + me.getVvaUserName() + '</userName>'
				+ '<password>' + me.getVvaPassword() + '</password>'
				+ '<fnDocId>' + selectedDocument.get('fileNetDocumentId') + '</fnDocId>'
				+ '<fnDocSource>' + selectedDocument.get('fileNetDocumentSource') + '</fnDocSource>'
				+ '<docFormatCode>' + selectedDocument.get('documentFormatCode') + '</docFormatCode>'
				+ '<jro>' + selectedDocument.get('jro') + '</jro>'
				+ '<userId>' + me.getPcrId() + '</userId>'
				+ '</DownloadVVADocument></soap:Body></soap:Envelope>',
			isProd = me.getEnvironment() ? me.getEnvironment().get('isPROD') : false,
			request = new ActiveXObject('Microsoft.XMLHTTP'),
			dacUrl = me.getDacUrl();

        if (Ext.isEmpty(dacUrl)) {
            me.setDacUrl(!Ext.isEmpty(me.application.crmEnvironment) ? me.application.crmEnvironment.get('globalDAC') : 'http://10.153.95.73/RedirectSvc.asmx');
        }

        me.getDocumentList().setLoading(true, true);

        request.open('POST', me.getDacUrl(), true);
        request.setRequestHeader('SOAPAction', '');
        request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
        request.setRequestHeader('Content-Length', env.length);
        request.onreadystatechange = function () {
            if (request.readyState == 4 /* complete */) {
                if (request.status == 200) {
                    me.getDocumentList().setLoading(false);
                    if (!Ext.isEmpty(request.responseXML)) {
                        // check for error VVAFAULT
                        //var response = request.responseXML.text;
                        var startPos = request.responseXML.text.indexOf("://") + 3; //remove the https://
                        var endPos = request.responseXML.text.substring(startPos).indexOf("/"); //the first / after https:// is the end of the domain
                        var dns = request.responseXML.text.substring(startPos, endPos + startPos);
                        //alert(dns); //debug
                        if (request.responseXML.text && request.responseXML.text.length > 7 && request.responseXML.text.substring(0, 8) === 'VVAFAULT') {
                            alert(request.responseXML.text.substring(8));
                        }
                        //verify that it is redirecting to a va.gov site and the path includes /dac/vva (case insensitive)
                        else if ((request.responseXML.text.toLowerCase().indexOf('/dac/vva')>0) && (dns.substring(dns.length - 7).toLowerCase() === '.va.gov')) {
                            window.open(request.responseXML.text);
                        }
                        else {
                            alert("Invalid VVA document location: " + request.responseXML.text);
                        }
                    }
                }
            }
        };
        request.send(env);
    },

    getEnvironment: function () {
        var app = _extApp;
        if (Ext.isEmpty(app)) {
            app = this.application;
        }
        if (!Ext.isEmpty(app) && !Ext.isEmpty(app.crmEnvironment)) {
            return app.crmEnvironment;
        }
        return null;
    }
});
