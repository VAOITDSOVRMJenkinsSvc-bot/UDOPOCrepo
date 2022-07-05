/**
* @author Ivan Yurisevic
* @class VIP.view.requestsexams.requestsexamstabs.exams.ExamList
*
* The view for the past POA grid
*/
Ext.define('VIP.view.pathways.Mvi', {
    extend: 'Ext.grid.Panel',
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    alias: 'widget.pathways.mvi',
    title: 'MVI Person Search',
    store: 'mvi.Patient',
    //listeners: {
    //    afterrender: function () {
    //        var me = this;
    //
    //        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
    //            if (Ext.get('id_pathways_Mvi_01')) Ext.get('id_pathways_Mvi_01').hide();
    //            if (Ext.get('id_pathways_Mvi_02')) Ext.get('id_pathways_Mvi_02').hide();
    //            if (Ext.get('id_pathways_Mvi_03')) Ext.get('id_pathways_Mvi_03').hide();
    //        }
    //    }
    //},
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'National Id', dataIndex: 'nationalId' },
                { header: 'SSN', dataIndex: 'ssn' },
                { header: 'Name', dataIndex: 'name' },
                { header: 'Gender', dataIndex: 'gender' },
                { header: 'DOB', dataIndex: 'dob', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Birth Place', dataIndex: 'birthplace' }
            ]
        };

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
			    {
				    text: 'Search Appointments/Exam Requests/Exams',
				    tooltip: 'Search all available pathways systems',
				    iconCls: 'icon-refresh',
				    action: 'searchPathways',
				    id: 'id_pathways_Mvi_01'
				},
                {
                    text: 'Search eBenefits',
                    tooltip: 'Search eBenefits status',
                    iconCls: 'icon-refresh',
                    action: 'searchEBenefits',
                    id: 'id_pathways_Mvi_02'
                },
                {
                    text: 'Search Tips',
                    tooltip: 'Search Tips pop-up',
                    iconCls: 'icon-noteEdit',
                    action: 'popupTips',
                    id: 'id_pathways_Mvi_03'
                }
            ]
        }];

        me.callParent();
    }
});
