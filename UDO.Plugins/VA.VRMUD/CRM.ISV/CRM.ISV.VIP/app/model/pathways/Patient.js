/**
* @author Josh Oliver
* @class VIP.model.pathways.Patient
*
* The model for patients
*/
Ext.define('VIP.model.pathways.Patient', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.Pathways',
        'VIP.model.pathways.ExamRequest',
        'VIP.model.pathways.Exam',
        'VIP.soap.envelopes.pathways.ExamRequest'
    ],
    fields: [
        {
            name: 'nationalId',
            type: 'string',
            mapping: 'requestedNationalId',
            ignoreMappingOnRequest: true
        }       
    ],
    hasMany: [
        {
            model: 'VIP.model.pathways.ExamRequest',
            name: 'examRequests',
            associationKey: 'examRequests'
        },
        {
            model: 'VIP.model.pathways.Exam',
            name: 'exams',
            associationKey: 'exams'
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'pathways',
            record: 'patient'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.pathways.ExamRequest',
            update: '',
            destroy: ''
        }
    }
});