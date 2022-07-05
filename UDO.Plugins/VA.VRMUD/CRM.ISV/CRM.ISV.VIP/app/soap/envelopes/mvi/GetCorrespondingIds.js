Ext.define('VIP.soap.envelopes.mvi.GetCorrespondingIds', {
    extend: 'VIP.soap.envelopes.mvi.MviTemplate',
    alias: 'envelopes.mvi.GetCorrespondingIds',
    requires: [
        'VIP.util.xml.FragmentBuilder',
        'VIP.soap.analyzers.mvi.GetCorrespondingIds'
    ],
    config: {
        nationalId: ''
    },

    analyzeResponse: Ext.create('VIP.soap.analyzers.mvi.GetCorrespondingIds'),

    constructor: function (config) {
        var me = this,
            idExtentionFieldValue = 'CRM-',
            extensionDate = Ext.Date.format(new Date(), 'Ymd'),
            creationDateTime = Ext.Date.format(new Date(), 'YmdHis'),
            environmentName = !Ext.isEmpty(me.getEnvironment()) ? me.getEnvironment().get('envName') : 'INTI',
            executedInCrm = true;
        
        me.initConfig(config);

        me.callParent();

        idExtentionFieldValue += environmentName + '-' + extensionDate;

        me.setBody('PRPA_IN201309UV02', {
            namespace: 'ps',
            namespaces: {
                'ps': 'http://vaww.oed.oit.va.gov',
                'xsi': 'http://www.w3.org/2001/XMLSchema-instance',
                'urn': 'urn:hl7-org:v3'
            },
            attributes: {
                'schemaLocation': {
                    namespace: 'urn',
                    value: 'urn:hl7-org:v3 ../../schema/HL7V3/NE2008/multicacheschemas/PRPA_IN201309UV02.xsd',
                    prefix: 'xsi'
                },
                'ITSVersion': 'XML_1.0',
                'xmlns': 'http://vaww.oed.oit.va.gov'
            },
            id: {
                namespace: '',
                attributes: {
                    'root': '2.16.840.1.113883.4.349',
                    'extension': idExtentionFieldValue + '-GETCORIDS-A01'
                }
            },
            creationTime: {
                namespace: '',
                attributes: {
                    'value': creationDateTime
                }
            },
            interactionId: {
                namespace: '',
                attributes: {
                    'root': '2.16.840.1.113883.1.6'
                }
            },
            processingCode: {
                namespace: '',
                attributes: {
                    'code': 'T'
                }
            },
            processingModeCode: {
                namespace: '',
                attributes: {
                    'code': 'T'
                }
            },
            acceptAckCode: {
                namespace: '',
                attributes: {
                    'code': 'AL'
                }
            },
            receiver: {
                namespace: '',
                attributes: {
                    'typeCode': 'RCV'
                },
                device: {
                    namespace: '',
                    attributes: {
                        'classCode': 'DEV',
                        'determinerCode': 'INSTANCE'
                    },
                    id: {
                        namespace: '',
                        attributes: {
                            'root': '2.16.840.1.113883.4.349'
                        }
                    }
                }
            },
            sender: {
                namespace: '',
                attributes: {
                    'typeCode': 'SND'
                },
                device: {
                    namespace: '',
                    attributes: {
                        'classCode': 'DEV',
                        'determinerCode': 'INSTANCE'
                    },
                    id: {
                        namespace: '',
                        attributes: {
                            'root': '1.2.840.1.13.113883.4.349',
                            'extension': executedInCrm ? '200CRM' : '200ESR'
                        }
                    }
                }
            },
            controlActProcess: {
                namespace: '',
                attributes: {
                    'classCode': 'CACT',
                    'moodCode': 'EVN'
                },
                code: {
                    namespace: '',
                    attributes: {
                        'code': 'PRPA_TE201309UV02',
                        'codeSystem': '2.16.840.1.113883.1.6'
                    }
                },
                queryByParameter: {
                    namespace: '',
                    queryId: {
                        namespace: 'urn',
                        attributes: {
                            'extension': idExtentionFieldValue + '-SEARCH-A01',
                            'root': 'CRM_APP'
                        }
                    },
                    statusCode: {
                        namespace: '',
                        attributes: {
                            'code': 'new'
                        }
                    },
                    responsePriorityCode: {
                        namespace: '',
                        attributes: {
                            'code': 'I'
                        }
                    },
                    parameterList: {
                        namespace: '',
                        patientIdentifier: {
                            namespace: '',
                            '@value': {
                                namespace: '',
                                attributes: {
                                    'root': '2.16.840.1.113883.4.349',
                                    'extension': me.getNationalId()
                                }
                            },
                            semanticsText: {
                                namespace: '',
                                value: 'Patient.Id'
                            }
                        }
                    }
                }
            }
        });
    }
});
