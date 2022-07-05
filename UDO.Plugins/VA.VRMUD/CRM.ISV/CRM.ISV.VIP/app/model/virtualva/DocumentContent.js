/**
* @author Josh Oliver
* @class VIP.model.virtualva.DocumentContent
*
* The model for Virtual VA document content
*/
Ext.define('VIP.model.virtualva.DocumentContent', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.virtualva.GetDocumentContent'
    ],
    fields: [
        {
            name: 'content'
        },
        {
            name: 'documentTitle'
        },
        {
            name: 'fileName'
        },
        {
            name: 'claimId',
            mapping: 'claimNumber'
        },
        {
            name: 'dateOfReceipt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'documentType',
            type: 'int'
        },
        {
            name: 'documentSubject'
        },
        {
            name: 'insertedByUserId'
        },
        {
            name: 'insertedByRegionalOffice',
            mapping: 'insertedByRO'
        },
        {
            name: 'sourceComment'
        },
        {
            name: 'source'
        },
        {
            name: 'documentCategory'
        },
        {
            name: 'mimeType'
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset:utf-8"
        },
        reader: {
            type: 'xml',
            record: 'GetDocumentContentResponse'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.virtualva.GetDocumentContent',
            update: '',
            destroy: ''
        }
    }
});