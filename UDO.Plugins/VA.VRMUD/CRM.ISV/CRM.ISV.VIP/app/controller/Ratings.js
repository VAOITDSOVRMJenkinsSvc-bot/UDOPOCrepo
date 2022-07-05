/**
* @author Dmitri Riz
* @class VIP.controller.Ratings
*
* The controller for the Ratings tab
*/

Ext.define('VIP.controller.Ratings', {
    extend: 'Ext.app.Controller',
    refs: [{
        ref: 'ratingTabs',
        selector: '[xtype="ratings.ratingstabpanel"]'
    }, {
        ref: 'disabilitySelectedDetails',
        selector: '[xtype="ratings.disabilityratings.disabilitydetails"]'
    }, {
        ref: 'disabilityDetails',
        selector: '[xtype="ratings.disabilityratings.disabilityrecorddetails"]'
    }, {
        ref: 'disabilityRatingGrid',
        selector: '[xtype="ratings.disabilityratings.disabilitydata"]'
    }, {
        ref: 'deathRatingGrid',
        selector: '[xtype="ratings.deathratings"]'
    }, {
        ref: 'familyRatingGrid',
        selector: '[xtype="ratings.familymemberratings"]'
    }, {
        ref: 'otherRatingGrid',
        selector: '[xtype="ratings.otherratings"]'
    }, {
        ref: 'smcParagraphGrid',
        selector: '[xtype="ratings.smcratings.paragraphratings"]'
    }, {
        ref: 'smcRatingGrid',
        selector: '[xtype="ratings.smcratings.ratings"]'
    }],

    stores: [
        'Ratings',
        'ratings.DisabilityRating',
        'ratings.DisabilityRatingRecord',
        'ratings.DeathRating',
        'ratings.FamilyRating',
        'ratings.OtherRating',
        'ratings.SmcParagraphRating',
        'ratings.SmcRating'
    ],

    init: function () {
        var me = this;

        me.control({
            '[xtype="ratings.disabilityratings.disabilitydata"]': {
                selectionchange: me.onDisabilityRatingsGridSelect
            },
            '[xtype="ratings.ratingstabpanel"]': {
                tabchange: me.onRatingTabChange
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            triggerratingtabchange: me.onRatingTabChange,
            scope: me
        });
    },

    onRatingTabChange: function(tabPanel, newCard, oldCard, eOpts){
        var me = this,
            activeTab = me.getRatingTabs().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = null;

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else if (activeTab.getXType() == 'ratings.disabilityratings') {
            tabTitle = 'Disability Ratings: ' + me.getDisabilityRatingGrid().getStore().getCount();
        } else if (activeTab.getXType() == 'ratings.smcratings') {
            tabTitle = 'SMC Ratings: ' + me.getSmcRatingGrid().getStore().getCount() +
                       ', Paragraph Ratings: ' + me.getSmcParagraphGrid().getStore().getCount();
        } else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onDisabilityRatingsGridSelect: function (selectionModel, selection, options) {
        var me = this;
        if (!Ext.isEmpty(selection)) {
            me.getDisabilitySelectedDetails().loadRecord(selection[0]);
        }
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        if (!Ext.isEmpty(selectedPerson.get('fileNumber'))) {
            me.getRatingsStore().load({
                filters: [{
                    property: 'fileNumber',
                    value: selectedPerson.get('fileNumber')
                }],
                callback: function (records, operation, success) {
                    if (success && !Ext.isEmpty(records)) {

                        me.getDisabilityDetails().loadRecord(records[0]);

                        me.getDisabilityRatingGrid().reconfigure(records[0].disabilityratings());
                        me.getDeathRatingGrid().reconfigure(records[0].deathratings());
                        me.getFamilyRatingGrid().reconfigure(records[0].familyratings());
                        me.getOtherRatingGrid().reconfigure(records[0].otherratings());
                        me.getSmcParagraphGrid().reconfigure(records[0].smcparagraphrating());
                        me.getSmcRatingGrid().reconfigure(records[0].smcrating());

                    }
                    me.application.fireEvent('webservicecallcomplete', operation, 'Ratings');
                },
                scope: me
            });

        }
    },

    onCachedDataLoaded: function () {
        var me = this,
            ratingData = me.getRatingsStore().getAt(0);
        if (!Ext.isEmpty(ratingData)) {
            //Load Panels
            me.getDisabilityDetails().loadRecord(ratingData);

            //Load Grids
            me.getDisabilityRatingGrid().reconfigure(ratingData.disabilityratings());
            me.getDeathRatingGrid().reconfigure(ratingData.deathratings());
            me.getFamilyRatingGrid().reconfigure(ratingData.familyratings());
            me.getOtherRatingGrid().reconfigure(ratingData.otherratings());
            me.getSmcParagraphGrid().reconfigure(ratingData.smcparagraphrating());
            me.getSmcRatingGrid().reconfigure(ratingData.smcrating());
            return;
        }
    }

});
