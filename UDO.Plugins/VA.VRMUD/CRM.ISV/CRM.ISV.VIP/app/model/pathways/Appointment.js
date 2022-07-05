/**
* @author Josh Oliver
* @class VIP.model.pathways.Appointment
*
* The model for appointments
*/
Ext.define('VIP.model.pathways.Appointment', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.Pathways',
        'VIP.soap.envelopes.pathways.Appointment'
    ],
    fields: [
        {
            name: 'identity',
            type: 'string',
            mapping: 'recordIdentifier/identity'
        },
        {
            name: 'namespaceId',
            type: 'string',
            mapping: 'recordIdentifier/namespaceId'
        },
        {
            name: 'universalId',
            type: 'string',
            mapping: 'recordIdentifier/universalId' 
        },
        {
            name: 'universalIdType',
            type: 'string',
            mapping: 'recordIdentifier/universalIdType' 
        },
        {
            name: 'patientIdentity',
            type: 'string',
            mapping: 'patient/identifier/identity' 
        },
        {
            name: 'patientAssigningFacility',
            type: 'string',
            mapping: 'patient/identifier/assigningFacility' 
        },
        {
            name: 'patientAssigningAuthority',
            type: 'string',
            mapping: 'patient/identifier/assigningAuthority' 
        },
        {
            name: 'patientPrefix',
            type: 'string',
            mapping: 'patient/name/prefix' 
        },
        {
            name: 'patientGiven',
            type: 'string',
            mapping: 'patient/name/given' 
        },
        {
            name: 'patientMiddle',
            type: 'string',
            mapping: 'patient/name/middle' 
        },
        {
            name: 'patientFamily',
            type: 'string',
            mapping: 'patient/name/family' 
        },
        {
            name: 'patientSuffix',
            type: 'string',
            mapping: 'patient/name/suffix' 
        },
        {
            name: 'patientTitle',
            type: 'string',
            mapping: 'patient/name/title' 
        },
        {
            name: 'patientName',
            type: 'string',
            mapping: 'patient/name/displayName' 
        },
        {
            name: 'appointmentDateTime',
            type: 'date',
            mapping: 'appointmentDateTime/literal',
            dateFormat: 'YmdHi'
        },
        {
            name: 'appointmentDateTime_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('appointmentDateTime'))) {
                    return Ext.Date.format(record.get('appointmentDateTime'), 'm/d/Y h:i a');
                } else { return ''; }
            }
        },
        {
            name: 'locationIdentity',
            type: 'string',
            mapping: 'location/identifier/identity' 
        },
        {
            name: 'locationAssigningAuthority',
            type: 'string',
            mapping: 'location/identifier/assigningAuthority' 
        },
        {
            name: 'locationName',
            type: 'string',
            mapping: 'location/identifier/name' 
        },
        {
            name: 'locationPhone',
            type: 'string',
            mapping: 'location/telephone' 
        },
        {
            name: 'locationInstitutionIdentity',
            type: 'string',
            mapping: 'location/institution/identifier/identity' 
        },
        {
            name: 'locationInstitutionAssigningAuthority',
            type: 'string',
            mapping: 'location/institution/identifier/assigningAuthority' 
        },
        {
            name: 'locationInstitutionName',
            type: 'string',
            mapping: 'location/institution/identifier/name' 
        },
        {
            name: 'locationInstitutionShortName',
            type: 'string',
            mapping: 'location/institution/shortName' 
        },
        {
            name: 'locationInstitutionStationNumber',
            type: 'string',
            mapping: 'location/institution/stationNumber' 
        },
        {
            name: 'locationInstitutionOfficialVAName',
            type: 'string',
            mapping: 'location/institution/officialVAName' 
        },
        {
            name: 'appointmentStatusCode',
            type: 'string',
            mapping: 'appointmentStatus/code' 
        },
        {
            name: 'appointmentStatusText',
            type: 'string',
            mapping: 'appointmentStatus/displayText' 
        },
        {
            name: 'appointmentStatusCodingSystem',
            type: 'string',
            mapping: 'appointmentStatus/codingSystem' 
        },
        {
            name: 'appointmentStatus',
            type: 'string',
            mapping: 'appointmentStatus/displayText'
        },
        {
            name: 'appointmentTypeCode',
            type: 'string',
            mapping: 'appointmentType/code' 
        },
        {
            name: 'appointmentTypeText',
            type: 'string',
            mapping: 'appointmentType/displayText' 
        },
        {
            name: 'appointmentTypeCodingSystem',
            type: 'string',
            mapping: 'appointmentType/codingSystem' 
        },
        {
            name: 'appointmentType',
            type: 'string',
            mapping: 'appointmentType/displayText' 
        },
        {
            name: 'ekgDateTime',
            type: 'string',
            mapping: 'ekgDateTime/literal' 
        },
        {
            name: 'xrayDateTime',
            type: 'string',
            mapping: 'xrayDateTime/literal' 
        },
        {
            name: 'labDateTime',
            type: 'string',
            mapping: 'labDateTime/literal' 
        },
        {
            name: 'statusCode',
            type: 'string',
            mapping: 'status/code' 
        },
        {
            name: 'statusText',
            type: 'string',
            mapping: 'status/displayText' 
        },
        {
            name: 'statusCodingSystem',
            type: 'string',
            mapping: 'status/codingSystem' 
        },
        {
            name: 'status',
            type: 'string',
            mapping: 'status/displayText' 
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
            record: 'appointment'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.pathways.Appointment',
            update: '',
            destroy: ''
        }
    }
});