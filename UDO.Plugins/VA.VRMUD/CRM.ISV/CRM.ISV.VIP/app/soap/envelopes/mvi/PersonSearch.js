Ext.define('VIP.soap.envelopes.mvi.PersonSearch', {
    extend: 'VIP.soap.envelopes.mvi.MviTemplate',
    alias: 'envelopes.mvi.PersonSearch',
    requires: [
        'VIP.util.xml.FragmentBuilder',
        'VIP.soap.analyzers.mvi.PersonSearch'
    ],
    config: {
        firstName: '',
        lastName: '',
        middleName: '',
        dob: '',
        ssn: '',
        gender: '',
        crmEnvironmentName: ''
    },

    analyzeResponse: Ext.create('VIP.soap.analyzers.mvi.PersonSearch'),

    constructor: function (config) {
        var me = this,
            idExtentionFieldValue = 'CRM-',
            queryIdExtensionFieldValue = 'CRM_',
            executedInCrmValue,
            livingSubjectNameObject,
            livingSubjectIdObject,
            genderInitial,
            extensionDate = Ext.Date.format(new Date(), 'Ymd'),
            creationDateTime = Ext.Date.format(new Date(), 'YmdHis'),
            environmentName = !Ext.isEmpty(me.getEnvironment()) ? me.getEnvironment().get('envName') : 'INTI', //Must change this to be extracted from the persistence model
            executedInCrm = true; //Must change this to be extracted from the persistence model

        me.initConfig(config);

        me.callParent();

        me.addExcludedParameter([
            'environmentName',
            'executedInCrm',
            'attendedSearch'
        ]);

        idExtentionFieldValue += environmentName + '-' + extensionDate + '-SEARCH-A01';
        queryIdExtensionFieldValue += environmentName + '_APP_main_' + extensionDate + '"';

        if (!Ext.isEmpty(me.getGender())) {
            genderInitial = me.getGender().substring(0, 1).toUpperCase();
            if (genderInitial == 'U') { genderInitial = ' '; }
        }
        else {
            genderInitial = ' ';
            if (parent && parent.Xrm && parent.Xrm.Page) {
                try {
                    var gi = parent.Xrm.Page.getAttribute('va_genderset').getText();
                    if (!Ext.isEmpty(gi) && gi != 'Unknown') { genderInitial = gi.substring(0, 1); }
                }
                catch (gie) { }
            }
        }

        executedInCrmValue = executedInCrm ? '200CRM' : '200ESR';

        if (!Ext.isEmpty(me.getSsn())) {
            if (environmentName == 'PROD') {
                livingSubjectIdObject = {
                    namespace: 'urn',
                    '@value': {
                        namespace: 'urn',
                        attributes: {
                            'root': '2.16.840.1.113883.4.1',
                            'extension': me.getSsn()
                        }
                    },
                    semanticsText: {
                        namespace: 'urn',
                        value: 'SSN'
                    }
                };
            }
            else {
                livingSubjectIdObject = {
                    namespace: '',
                    '@value': {
                        namespace: '',
                        attributes: {
                            'root': '2.16.840.1.113883.4.1',
                            'extension': me.getSsn()
                        }
                    },
                    semanticsText: {
                        namespace: '',
                        value: 'SSN'
                    }
                };
            }
        }

        if (!Ext.isEmpty(me.getLastName())) {
            livingSubjectNameObject = {
                namespace: 'urn',
                '@value': {
                    namespace: 'urn',
                    attributes: {
                        'use': 'L'
                    }
                }
            };
            if (!Ext.isEmpty(me.getFirstName())) {
                livingSubjectNameObject['@value'].given = {
                    namespace: 'urn',
                    value: me.getFirstName()
                };
            }
            if (!Ext.isEmpty(me.getMiddleName())) {
                livingSubjectNameObject['@value']['@given'] = {
                    namespace: 'urn',
                    value: me.getMiddleName()
                };
            }

            livingSubjectNameObject['@value'].family = {
                namespace: 'urn',
                value: me.getLastName()
            };

            livingSubjectNameObject.semanticsText = {
                namespace: 'urn',
                value: 'LivingSubject.name'
            };
        }

        me.setBody('PRPA_IN201305UV02', {
            namespace: environmentName == 'PROD' ? 'partns' : '',
            namespaces: {
                'partns': 'http://vaww.oed.oit.va.gov',
                'vaww': 'http://vaww.oed.oit.va.gov',
                'urn': 'urn:hl7-org:v3',
                'ps': 'http://vaww.oed.oit.va.gov'

            },
            attributes: {
                'schemaLocation': {
                    namespace: 'urn',
                    value: 'urn:hl7-org:v3 ../../schema/HL7V3/NE2008/multicacheschemas/PRPA_IN201305UV02.xsd',
                    prefix: 'xsi'
                },
                'ITSVersion': 'XML_1.0',
                'xmlns': 'http://vaww.oed.oit.va.gov'
            },
            id: {
                namespace: 'urn',
                attributes: {
                    'root': '2.16.840.1.113883.4.349',
                    'extension': idExtentionFieldValue
                }
            },
            creationTime: {
                namespace: 'urn',
                attributes: {
                    'value': creationDateTime
                }
            },
            interactionId: {
                namespace: 'urn',
                attributes: {
                    'root': '2.16.840.1.113883.1.6',
                    'extension': 'PRPA_IN201305UV02'
                }
            },
            processingCode: {
                namespace: 'urn',
                attributes: {
                    'code': 'T'
                }
            },
            processingModeCode: {
                namespace: 'urn',
                attributes: {
                    'code': 'T'
                }
            },
            acceptAckCode: {
                namespace: 'urn',
                attributes: {
                    'code': 'AL'
                }
            },
            receiver: {
                namespace: 'urn',
                attributes: {
                    'typeCode': 'RCV'
                },
                device: {
                    namespace: 'urn',
                    attributes: {
                        'classCode': 'DEV',
                        'determinerCode': 'INSTANCE'
                    },
                    id: {
                        namespace: 'urn',
                        attributes: {
                            'root': '2.16.840.1.113883.4.349'
                        }
                    }
                }
            },
            sender: {
                namespace: 'urn',
                attributes: {
                    'typeCode': 'SND'
                },
                device: {
                    namespace: 'urn',
                    attributes: {
                        'classCode': 'DEV',
                        'determinerCode': 'INSTANCE'
                    },
                    id: {
                        namespace: 'urn',
                        attributes: {
                            'root': '2.16.840.1.113883.4.349',
                            'extension': executedInCrmValue
                        }
                    }
                }
            },
            controlActProcess: {
                namespace: 'urn',
                attributes: {
                    'classCode': 'CACT',
                    'moodCode': 'EVN'
                },
                code: {
                    namespace: 'urn',
                    attributes: {
                        'code': 'PRPA_TE201309UV02',
                        'codeSystem': '2.16.840.1.113883.1.6'
                    }
                },
                dataEnterer: {
                    namespace: 'urn',
                    attributes: {
                        'typeCode': 'ENT',
                        'contextControlCode': 'AP'
                    },
                    assignedPerson: {
                        namespace: 'urn',
                        attributes: {
                            'classCode': 'ASSIGNED'
                        },
                        assignedPerson: {
                            namespace: 'urn',
                            attributes: {
                                'classCode': 'ASSIGNED'
                            },
                            assignedPerson: {
                                namespace: 'urn',
                                attributes: {
                                    'classCode': 'PSN',
                                    'determinerCode': 'INSTANCE'
                                },
                                name: {
                                    namespace: 'urn',
                                    value: 'CRM_APP'
                                }
                            }
                        }
                    }
                },
                queryByParameter: {
                    namespace: 'urn',
                    queryId: {
                        namespace: 'urn',
                        attributes: {
                            'extension': idExtentionFieldValue,
                            'root': 'CRM_APP'
                        }
                    },
                    statusCode: {
                        namespace: 'urn',
                        attributes: {
                            'code': 'new'
                        }
                    },
                    responsePriorityCode: {
                        namespace: 'urn',
                        attributes: {
                            'code': 'I'
                        }
                    },
                    initialQuantity: {
                        namespace: 'urn',
                        attributes: {
                            'value': '100'
                        }
                    },
                    parameterList: {
                        namespace: 'urn',
                        livingSubjectAdministrativeGender: {
                            namespace: 'urn',
                            '@value': {
                                namespace: 'urn',
                                attributes: {
                                    'code': genderInitial
                                }
                            },
                            semanticsText: {
                                namespace: 'urn',
                                value: 'LivingSubject.administrativeGender'
                            }
                        },
                        livingSubjectBirthTime: {
                            namespace: 'urn',
                            '@value': {
                                namespace: 'urn',
                                attributes: {
                                    'value': Ext.Date.format(new Date(me.getDob()), 'Ymd')
                                }
                            },
                            semanticsText: {
                                namespace: 'urn',
                                value: 'LivingSubject.birthTime'
                            }
                        },
                        livingSubjectId: livingSubjectIdObject,
                        livingSubjectName: livingSubjectNameObject
                    }
                }
            }
        });
    }    
});



