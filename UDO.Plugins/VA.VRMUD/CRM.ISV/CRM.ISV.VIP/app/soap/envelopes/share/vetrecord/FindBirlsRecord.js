Ext.define('VIP.soap.envelopes.share.vetrecord.FindBirlsRecord', {
    extend: 'VIP.soap.envelopes.share.VetRecordTemplate',
    alias: 'envelopes.FindBirlsRecord',
    config: {
        //        commandName: 'SHAR',
        //        commandType: 'I',
        //        transactionName: 'BPNQ',
        //        applicationName: 'VBMS',
        //        processName: 'BIRLS Inquiry',
        fileNumber: '',
        ssn: '',
        insuranceNumber: '',
        serviceNumber: '',
        lastName: '',
        firstName: '',
        middleName: '',
        suffix: '',
        payeeNumber: '00',
        branchOfService: '',
        dateOfBirth: '',
        dateOfDeath: '',
        enteredOnDutyDate: '',
        releasedActiveDutyDate: '',
        folderLocation: '',
        userSSN: '',
        userFileNumber: '',
        userStationNumber: '',
        userID: '',
        userIPAddress: '',
        payeeNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            if (!Ext.isEmpty(_extApp)) {
                var message = _extApp.webServiceMessageHandler.retrieveWebServiceMessages(response);
                if (!Ext.isEmpty(message) && message.success == false) {
                    //error found - searcherror event will stop loadmask and display message.
                    _extApp.fireEvent('searcherror', message.returnCode + ' - ' + message.returnMessage);
                    currentResultSet.success = false;
                    currentResultSet.total = 0;
                    currentResultSet.totalRecords = 0;
                    currentResultSet.statusText = message.returnCode + '-' + message.returnMessage;
                }
                else if (!Ext.isEmpty(message) && message.success == true) {
                    currentResultSet.statusText = message.returnCode + ' - ' + message.returnMessage;
                }
            }
            return currentResultSet;
        }
    }),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();
        me.setBody('findBirlsRecord', {
            namespace: 'ser',
            veteranRecordInput: {
                namespace: '',
                //                commandName: me.getCommandName(),
                //                commandType: me.getCommandType(),
                //                transactionName: me.getTransactionName(),
                //                applicationName: me.getApplicationName(),
                //                processName: me.getProcessName(),
                fileNumber: {
                    namespace: '',
                    value: me.getFileNumber()
                },
                ssn: {
                    namespace: '',
                    value: me.getSsn()
                },
                insuranceNumber: {
                    namespace: '',
                    value: me.getInsuranceNumber()
                },
                serviceNumber: {
                    namespace: '',
                    value: me.getServiceNumber()
                },
                lastName: {
                    namespace: '',
                    value: me.getLastName()
                },
                firstName: {
                    namespace: '',
                    value: me.getFirstName()
                },
                middleName: {
                    namespace: '',
                    value: me.getMiddleName()
                },
                suffix: {
                    namespace: '',
                    value: me.getSuffix()
                },
                payeeNumber: me.getPayeeNumber(),
                branchOfService: {
                    namespace: '',
                    value: me.getBranchOfService()
                },
                dateOfBirth: {
                    namespace: '',
                    value: me.getDateOfBirth()
                },
                dateOfDeath: {
                    namespace: '',
                    value: me.getDateOfDeath()
                },
                enteredOnDutyDate: {
                    namespace: '',
                    value: me.getEnteredOnDutyDate()
                },
                releasedActiveDutyDate: {
                    namespace: '',
                    value: me.getReleasedActiveDutyDate()
                },
                folderLocation: {
                    namespace: '',
                    value: me.getFolderLocation()
                },
                payeeNumber: {
                    namespace: '',
                    value: me.getPayeeNumber() ? me.getPayeeNumber() : '00' 
                }, 
                userSSN: {
                    namespace: '',
                    value: me.getUserSSN()
                },
                userFileNumber: {
                    namespace: '',
                    value: me.getUserFileNumber()
                },
                userStationNumber: {
                    namespace: '',
                    value: me.getUserStationNumber()
                },
                userID: {
                    namespace: '',
                    value: me.getUserID()
                },
                userIPAddress: {
                    namespace: '',
                    value: me.getUserIPAddress()
                }
            }
        });

        /*
        if (!Ext.isEmpty(config.userSsn)) {
        delete me.request.Envelope.Body.findBirlsRecord['userFileNumber'];
        }
        */
    }
});
