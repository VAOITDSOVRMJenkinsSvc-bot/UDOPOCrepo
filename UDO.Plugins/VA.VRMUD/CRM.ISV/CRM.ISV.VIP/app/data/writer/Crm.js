Ext.define('VIP.data.writer.Crm', {
    extend: 'Ext.data.writer.Writer',
    alias: 'writer.crm',

    crmForm: '',//Xrm.Page.data.entity.attributes,

    //Request will always be blank in this case
    writeRecords: function (request, data) {
        var me = this,
            i = 0,
            len = data.length,
            item,
            key,
            attribute;
        //debugger;
        for (; i < len; ++i) {
            item = data[i];
            for (key in item) {
                if (item.hasOwnProperty(key)) {
                    attributeName = item[key];

                    attribute = data.get(attributeName);

                    //me.setAttributeValueByType[attribute.getAttributeType()](attribute, )
                }
            }
        }

        return request;
    },

    setAttributeValueByType: [
        {
            'boolean': function (attribute, value) {
                return attribute.getValue();
            }
        },
        {
            'datetime': function (attribute) {
                return attribute.getValue();
            }
        },
        {
            'decimal': function (attribute) {
                return attribute.getValue();
            }
        },
        {
            'double': function (attribute) {
                return attribute.getValue();
            }
        },
        {
            'integer': function (attribute) {
                return attribute.getValue();
            }
        },
        {
            'lookup': function (attribute) {
                return attribute.getValue().name;
            }
        },
        {
            'memo': function (attribute) {
                return attribute.getValue();
            }
        },
        {
            'money': function (attribute) {
                return attribute.getValue();
            }
        },
        {
            'optionset': function (attribute) {
                return attribute.getSelectedOption().text;
            }
        },
        {
            'string': function (attribute) {
                return attribute.getValue();
            }
        }
    ]
});