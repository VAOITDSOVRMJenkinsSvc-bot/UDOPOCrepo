/**
* @author Dmitri Riz
* @class VIP.controller.ContentPanel
*
* The controller for the content panel of the application. If no search 
* has yet been initiated, displays a launch panel. If a search is initiated,
* depending on the number of records returned, a selection screen
* will be added to the content panel, or the exact veteran match will be generated.
*/
Ext.define('VIP.controller.ContentPanel', {
    extend: 'Ext.app.Controller',

    refs: [
        {
            ref: 'contentPanel',
            selector: 'contentpanel'
        },
        {
            ref: 'launch',
            selector: 'launch'
        },
        {
            ref: 'parentLaunch',
            selector: 'crmlaunch'
        },
        {
            ref: 'personSelection',
            selector: 'personselection'
        },
        {
            ref: 'personTabPanel',
            selector: 'persontabpanel'
        }
    ],

    init: function () {
        var me = this;

        me.application.on({
            personinquirystarted: me.onPersonInquiryStarted,
            personinquiryended: me.clearLoadMask,
            searcherror: me.clearLoadMask,
            multiplepeoplefound: me.clearLoadMask,
            clearcontentpanelloadmask: me.clearLoadMask,
            individualidentified: me.onIdentifyIndividual,
            startservicerequest: me.onStartServiceRequest,
            personinquirynoresults: me.clearLoadMask,
            parentformnotdetected: me.onParentFormNotDetected,
            cacheddataloaded: me.onCachedDataLoaded,
            scope: me
        });

        Ext.log('The ContentPanel controller has been initialized.');
    },

    displaySearch: function () {
        var me = this;
        var launch = Ext.create('VIP.view.Launch', {
            xtype: 'launch',
            bodyStyle: {
                padding: '5'
            },
            collapsed: true
        });
        me.getContentPanel().items.insert(0, launch);
        me.getContentPanel().doLayout();
    },

    onCachedDataLoaded: function () {
    },

    onParentFormNotDetected: function () {
        var me = this;
        me.displaySearch()
    },

    onLaunch: function () {
        Ext.log('The content panel has been created.');
    },

    clearLoadMask: function () {
        this.getContentPanel().setLoading(false);
    },

    onPersonInquiryStarted: function (personInquiryModel) {
        Ext.log('A personinquirystarted has been received by the ContentPanel controller');

        var me = this,
            contentPanel = me.getContentPanel();
        
        me.refreshContentForms();
        me.refreshContentGrids();

        contentPanel.setLoading(true);
    },

    refreshContentForms: function () {
        var forms = Ext.ComponentQuery.query('form');

        if (!Ext.isEmpty(forms)) {
            for (var i = 0; i < forms.length; i++) {
                forms[i].getForm().reset();
            }
        }
    },

    refreshContentGrids: function () {
        var grids = Ext.ComponentQuery.query('grid');

        if (!Ext.isEmpty(grids)) {
            for (var i = 0; i < grids.length; i++) {
                var gridStore = grids[i].getStore();
                if (!Ext.isEmpty(gridStore)) {
                    gridStore.removeAll();
                }
            }
        }
    },

    onIdentifyIndividual: function (selectedPerson) {
        var me = this,
            contentPanel = me.getContentPanel();

        contentPanel.setLoading(false);



        me.application.fireEvent('personinquiryended', selectedPerson);
    },

    onStartServiceRequest: function (serviceRequestType, receivedFrom) {
        Ext.log('A startservicerequest event has been received ' + 'by the ContentPanel controller');

        if (!parent || !parent._CreateClaimServiceRequest) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }

        /*
        var msg = 'The client has issued a service request of type ' +
        serviceRequestType.name + ' with a value of ' +
        serviceRequestType.value + ' pertaining with ' +
        receivedFrom + '. ' +
        'This feature is not yet supported. This event is ' +
        'being received by the ContentPanel controller ' +
        'in the onStartServiceRequest method.';
        */
        if (confirm('Please confirm that you would like to' + serviceRequestType.prompt)) {
            //debugger;
            //last parameter,serviceRequestType.value, is set to null
            //if passed as a null, CRM's PhoneContactShared correctly calculates this value based on the label (e.g., 0820a)
            parent._CreateClaimServiceRequest(serviceRequestType.selection, receivedFrom, true, serviceRequestType.name, null);
        }

        Ext.log('A startservicerequest event has been processed by the ' + 'ContentPanel controller');
    }
});
