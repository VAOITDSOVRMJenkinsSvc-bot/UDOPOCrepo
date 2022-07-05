/**
* @author Jonas Dawson
* @class VIP.controller.AppStatus
*
* The controller for the application status bar.
*/
Ext.define('VIP.controller.AppStatus', {
    extend: 'Ext.app.Controller',

    stores: ['debug.WebServiceRequestHistory'],

    refs: [{
        ref: 'appStatus',
        selector: 'appstatus'
    }, {
        ref: 'processes',
        selector: 'appstatus > button[action="togglewebservicerequesthistory"]'
    }, {
        ref: 'statistics',
        selector: 'appstatus > tbtext[notificationType="statistics"]'
    }, {
        ref: 'alerts',
        selector: 'appstatus > button[action="displayalerts"]'
    }, {
        ref: 'mainTabs',
        selector: 'persontabpanel'
    }],

    init: function () {
        var me = this;

        me.control({
            'button[action="togglewebservicerequesthistory"]': {
                click: me.toggleWebServiceRequestHistory
            },
            'button[action="displayalerts"]': {
                click: me.displayAlerts
            }
        });

        me.application.on({
            webservicehistoryclose: me.onWebServiceHistoryClose,
            setstatisticstext: me.setStatisticsText,
            hidealerticon: me.hideAlertIcon,
            showalerticon: me.showAlertIcon,
            displayalerts: me.displayAlerts,
            scope: me
        });

        Ext.log('The AppStatus controller has been successfully initialized.');
    },

    toggleWebServiceRequestHistory: function () {
        this.application.fireEvent('togglewebservicerequesthistory');
    },

    onWebServiceHistoryClose: function () {
        this.getProcesses().toggle(false, true);
    },

    onLaunch: function () {
        Ext.log('The application status bar and AppStatus controller have been added.');
    },

    setStatisticsText: function (recordName, recordCount) {
        var me = this,
            statisticsText;

        if (recordName == null) {
            me.getStatistics().setText('');
        } else if (recordCount == null) {
            me.getStatistics().setText(recordName);
        } else {
            statisticsText = recordName + ': ' + recordCount;
            me.getStatistics().setText(statisticsText);
        }
    },

    hideAlertIcon: function () {
        this.getAlerts().setVisible(false);
    },

    showAlertIcon: function () {
        this.getAlerts().setVisible(true);
    },

    displayAlerts: function (button, event, eOpts) {
        /* Get tab where this was pressed. Query the service history store and gather relevant alerts.
        * Display them in a window.
        */
        var me = this,
            wsHistory = me.getDebugWebServiceRequestHistoryStore(),
            currentTab = me.getMainTabs().getActiveTab(),
            failedWebServices = new Array();

        wsHistory.filterBy(function (record, id) {
            if (record.get('success') == false) {
                return true;
            } else return false;
        });

        if (wsHistory.getCount() == 0) return;

        switch (currentTab.getXType()) {
            case 'personinfo':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findBirlsRecordByFileNumber' ||
                        method == 'findCorporateRecordByFileNumber' ||
                        method == 'findGeneralInformationByPtcpntIds' ||
                        method == 'findAllRelationships' ||
                        method == 'findDependents' ||
                        method == 'findAllPtcpntAddrsByPtcpntId' || 
                        method == 'findVeteranByPtcpntId' ||
                        method == 'getRegistrationStatus') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'poa':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findAllFiduciaryPoa') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'fiduciary':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findAllFiduciaryPoa') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'birls':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findBirlsRecordByFileNumber') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'awards':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findGeneralInformationByFileNumber' ||
                        method == 'findGeneralInformationByPtcpntIds' ||
                        method == 'findOtherAwardInformation' ||
                        method == 'findIncomeExpense' || 
                        method == 'findFiduciary') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'claims':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findDevelopmentNotes' ||
                        method == 'findBenefitClaim' ||
                            method == 'findUnsolEvdnce' ||
                                method == 'findBenefitClaimDetail' ||
                                    method == 'findTrackedItems' ||
                                        method == 'findContentions' || 
                                            method == 'findClaimStatus') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'militaryservice':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findMilitaryRecordByPtcpntId') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'paymenthistory':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findPayHistoryBySSN') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'paymentinformation':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'retrievePaymentSummary' || 
                        method == 'retrievePaymentDetail') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'ratings':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findRatingData ') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'denials':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findDenialsByPtcpntId' || 
                        method == 'findReasonsByRbaIssueId') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'appeals':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'findAppeals') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'pathways':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'PRPA_IN201305UV02' ||
                        method == 'PRPA_IN201309UV02' || 
                            method == 'readData') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
            case 'personvadir':
                wsHistory.each(function (record) {
                    var method = record.get('method');
                    if (method == 'FindPersonByEdipiRequest' ||
                        method == 'FindPersonByFnameLnameRequest' ||
                        method == 'FindPersonByFnameLnameDobRequest' ||
                        method == 'FindPersonByLnameDobRequest' ||
                        method == 'FindPersonBySsnRequest' ||
                        method == 'GetContactInfoRequest' ||
                            method == 'readData') {
                        failedWebServices.push('--- ' + method);
                    }
                });
                break;
        }

        var resultText = '';
        if (failedWebServices.length != 0) {
            resultText = failedWebServices.join('<br/>');
        }
        Ext.Msg.show({
            title: currentTab.title,
            msg: 'Warning: the following web services failed: <br/>' + resultText + '<br/>Data may be missing from this tab. <br/><br/>Would you like to see the error log?',
            icon: 'ext-mb-warning',
            buttons: Ext.Msg.YESNO,
            fn: function (selection) {
                if (selection == 'yes') {
                    me.application.fireEvent('togglewebservicerequesthistory');
                } else {
                    me.getDebugWebServiceRequestHistoryStore().clearFilter();
                }
            }
        });


    }
});