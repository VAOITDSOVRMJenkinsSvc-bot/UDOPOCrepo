/**
* @author Jonas Dawson
* @class VIP.controller.SearchOverview
*
* The controller for the header of the application.
*/
Ext.define('VIP.controller.SearchOverview', {
    extend: 'Ext.app.Controller',
    uses: ['Ext.TaskManager'],

    totalExecutionTime: 0,
    callTimer: 0,

    executionTimeTask: undefined,
    callTimerTask: undefined,

    SearchErrors: new Array(),

    refs: [{
        ref: 'AppToolbar',
        selector: 'searchoverview'
    },{
        ref: 'statusText',
        selector: 'searchoverview > #statusText'
    }, {
        ref: 'executionTime',
        selector: 'searchoverview > [itemId="executiontime"]'
    }, {
        ref: 'callTimer',
        selector: 'searchoverview > [itemId="calltimer"]'
    }],

    init: function () {
        var me = this;

        me.application.on({
            searcherror: me.onSearchError,
            searchwarning: me.onSearchWarning,
            personinquirystarted: me.onPersonInquiryStarted,
            multiplepeoplefound: me.onMultiplePeopleFound,
            personinquirynoresults: me.onPersonInquiryNoResults,
            webservicerequeststarted: me.onWebServiceRequestStarted,
            webservicerequestfinished: me.onWebServiceRequestFinished,
            personinquiryended: me.stopExecutionTime,
            calltimerstarted: me.callTimerStarted,
            calltimerstopped: me.stopCallTimer,
            scope: me
        });

        Ext.Ajax.on({
            requestexception: me.onRequestException,
            scope: me
        });

        Ext.log('The ContentPanel controller has been initialized.');
    },

    onLaunch: function () {

    },

    onRequestException: function (conn, response, options) {
        var me = this;
        me.setAppStatus('Ajax Error: ' + response.status + ' - ' + response.statusText, 'error');
        me.application.fireEvent('clearcontentpanelloadmask');
    },

    onPersonInquiryNoResults: function () {
        var me = this;
        me.setAppStatus('No results were returned with the inputs specified.', 'warning');
        me.stopExecutionTime();
    },

    onWebServiceRequestFinished: function () {
        this.setAppStatus('No Processes Running', 'nominal');
    },

    onWebServiceRequestStarted: function (methodName) {
        this.setAppStatus(methodName + ' is currently executing.', 'busy');
    },

    onMultiplePeopleFound: function (corpNumberOfRecords, birlsNumberOfRecords) {
        var me = this;
        me.setAppStatus('Corp: ' + corpNumberOfRecords + ' BIRLS: ' + birlsNumberOfRecords, 'warning');
        me.stopExecutionTime();
    },

    onSearchError: function (errorMessage) {
        var me = this,
            errorText = '';

        me.SearchErrors.push(errorMessage);
        errorText = me.SearchErrors.join(' |&| ');

        me.setAppStatus('ERROR: ' + errorText, 'error');
        me.application.fireEvent('clearcontentpanelloadmask');

        if (parent && parent._SearchOnError != undefined && parent._SearchOnError) {
            parent._SearchOnError(errorText, me.SearchErrors);
        }

        me.stopExecutionTime();
    },

    onSearchWarning: function (warningMessage) {
        var me = this;
        me.setAppStatus('Warning: ' + warningMessage, 'error');
        me.stopExecutionTime();
    },

    callTimerStarted: function (timerObject) {
        var me = this,
            sec = 0,
            timerData = 0,
            stopTimer = false,
            callTimerView = me.getCallTimer();

        if (!Ext.isEmpty(timerObject)) {
            if (!Ext.isEmpty(timerObject.timerData)) {
                timerData = timerObject.timerData;
            }
            if (!Ext.isEmpty(timerObject.stopTimer)) {
                stopTimer = timerObject.stopTimer;
            }
        }

        if (!stopTimer) {
            me.callTimerTask = Ext.TaskManager.start({
                run: function () {
                    var me = this;

                    me.callTimer += 1 + timerData;
                    timerData = 0;
                    sec = me.callTimer;
                    callTimerView.setText('Call Time: ' + (Math.floor(sec / 60)).toFixed(0) + ':' + (sec % 60).toFixed(0));
                },
                interval: 1000,
                scope: me
            });
        } else {
            callTimerView.setText('Call Time: ' + (Math.floor(timerData / 60)).toFixed(0) + ':' + (timerData % 60).toFixed(0));
        }
    },

    stopCallTimer: function (timerData) {
        var me = this,
            callTimerView = me.getCallTimer();

        //debugger;
        if (!Ext.isEmpty(me.callTimerTask)) {
            Ext.TaskManager.stop(me.callTimerTask);
            callTimerView.setText('Call Time: ' + (Math.floor(timerData / 60)).toFixed(0) + ':' + (timerData % 60).toFixed(0));
        }
    },

    stopExecutionTime: function () {
        var me = this;

        Ext.TaskManager.stop(me.executionTimeTask);
    },

    onPersonInquiryStarted: function () {
        var me = this;

        me.executionTimeTask = Ext.TaskManager.start({
            run: function () {
                var me = this,
                    executionTimeView = me.getExecutionTime();

                me.totalExecutionTime += 0.1;

                executionTimeView.setText('Execution Time: ' + me.totalExecutionTime.toFixed(2) + ' seconds');
            },
            interval: 100,
            duration: 20000,
            scope: me
        });
    },

    /**
    * This is used by this class only, should be a private method
    */
    setAppStatus: function (text, level) {
        var me = this;
        me.getAppToolbar().setStatus({
            text: text,
            level: level
        });
    }
});