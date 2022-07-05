/**
* @author Josh Oliver
* @class VIP.model.appeals.Diary
*
* The model for appeal diaries
*/
Ext.define('VIP.model.appeals.Diary', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'diaryCode',
            type: 'string',
            mapping: 'DiaryCode'
        },
        {
            name: 'diaryDescription',
            type: 'string',
            mapping: 'DiaryDescription'
        },
        {
            name: 'assignedStaffMemberName',
            type: 'string',
            mapping: 'AssignedStaffMemberName'
        },
        {
            name: 'assignedToStaffMemberDate',
            type: 'date',
            mapping: 'AssignedToStaffMemberDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'daysToCompleteDiaryItemQuantity',
            type: 'string',
            mapping: 'DaysToCompleteDiaryItemQuantity'
        },
        {
            name: 'diarySuspenseDueDate',
            type: 'date',
            mapping: 'DiarySuspenseDueDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'diaryClosedDate',
            type: 'date',
            mapping: 'DiaryClosedDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'diaryStatusCode',
            type: 'string',
            mapping: 'DiaryStatusCode'
        },
        {
            name: 'diaryStatusDescription',
            type: 'string',
            mapping: 'DiaryStatusDescription'
        },
        {
            name: 'bvaOrRODiaryIndicatorText',
            type: 'string',
            mapping: 'BVAorRODiaryIndicatorText'
        },
        {
            name: 'responseNotesDescription',
            type: 'string',
            mapping: 'ResponseNotesDescription'
        },
        {
            name: 'requestedActivityDescription',
            type: 'string',
            mapping: 'RequestedActivityDescription'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Diary'
        }
    }
});