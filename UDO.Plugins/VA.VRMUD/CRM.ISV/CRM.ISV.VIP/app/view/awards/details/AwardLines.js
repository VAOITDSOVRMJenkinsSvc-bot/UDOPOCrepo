/**
* @author Jonas Dawson
* @class VIP.view.awards.details.AwardLines
*
* Award Lines for the selected award line
*/
Ext.define('VIP.view.awards.details.AwardLines', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.awardlines',
    store: 'awards.AwardLines',
    title: 'Award Lines',
    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Effective Date', dataIndex: 'effectiveDate', xtype: 'datecolumn', format: 'm/d/Y', width: 80 },
			{ header: 'Reason(s)', width: 250, renderer:
                function (value, metaData, record, rowIndex) {
                    var allReasons = '';
                    if (record.awardReasons().getCount() == 0) {
                        return '';
                    }
                    else {
                        record.awardReasons().each(function (reason, index, totalItems) {
                            var reason = reason.get('name').replace(/\s+$/, ''), //removes whitespace at the end
                                separator = index == totalItems - 1 ? '' : ', '; //adds separator between reasons except after the last one
                            allReasons += reason + separator;
                        });
                    }
                    return allReasons;
                }
			},
            { header: 'Entitlement', dataIndex: 'entitlementName', width: 210 },
            { header: 'AA/HB', dataIndex: 'aaHbInd' },
            { header: 'Spouse', dataIndex: 'spouse' },
            { header: 'Minor Child', dataIndex: 'minorChild' },
            { header: 'Helpless Child', dataIndex: 'helplessChild' },
            { header: 'School Child', dataIndex: 'schoolChild' },
            { header: 'Parent Num', dataIndex: 'parentNbr' },
            { header: 'Amount', dataIndex: 'altmnt', xtype: 'numbercolumn', format: '$0,000.00' },
            { header: 'Income', dataIndex: 'income', xtype: 'numbercolumn', format: '$0,000.00' },
            { header: 'Total', dataIndex: 'totalAward', xtype: 'numbercolumn', format: '$0,000.00' },
            { header: 'CRDP', dataIndex: 'crdpAmount' },
            { header: 'CRSC', dataIndex: 'crscAmount' },
            { header: 'Net Award', dataIndex: 'netAward', xtype: 'numbercolumn', format: '$0,000.00' },
            { header: 'Withhold Total', dataIndex: 'witholdingAmount' },
            { header: 'Other Adjustments', dataIndex: 'otherAdjustments' },
            { header: 'Withhold Drill', dataIndex: 'drillWitholding' },
            { header: 'Withhold Inst', dataIndex: 'instznWitholding' },
            { header: 'Recoup', dataIndex: 'recoupTotal' }
        ];

        me.callParent();
    }
});
