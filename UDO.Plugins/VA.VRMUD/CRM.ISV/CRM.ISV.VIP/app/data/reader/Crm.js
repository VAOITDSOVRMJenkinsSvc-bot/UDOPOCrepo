Ext.define('VIP.data.reader.Crm', {
    extend: 'Ext.data.reader.Reader',
    alias: 'reader.crm',
    implicitIncludes: false,
    totalProperty: undefined,
    successProperty: undefined,

    createAccessor: function (attributeName) {
        var me = this;

        if (Ext.isEmpty(attributeName)) {
            return Ext.emptyFn;
        }

        if (Ext.isFunction(attributeName)) {
            return attributeName;
        }

        return function (data) {
            return me.getAttributeValue(data, attributeName);
        };
    },

    getAttributeValue: function (data, attributeName) {
        var me = this,
            attribute = data.get(attributeName),
            attributeType = attribute.getAttributeType();

        if (!attribute)
            alert('VIP.CRM.js.getAttributeValue is trying to access nonexisting attribute ' + attributeName);
        else
            return me.getAttributeValueByType[attributeType](attribute);
    },

    getRoot: function (data) {
        var xrm = data.Xrm;
        return xrm.Page.data.entity.attributes;
    },

    getAttributeValueByType: {
        'boolean': function (attribute) {
            return attribute.getValue();
        },
        'datetime': function (attribute) {
            return attribute.getValue();
        },
        'decimal': function (attribute) {
            return attribute.getValue();
        },
        'double': function (attribute) {
            return attribute.getValue();
        },
        'integer': function (attribute) {
            return attribute.getValue();
        },
        'lookup': function (attribute) {
            var val = attribute.getValue();
            if (val && val.length > 0) return val[0].id;
            return null;
        },
        'memo': function (attribute) {
            return attribute.getValue();
        },
        'money': function (attribute) {
            return attribute.getValue();
        },
        'optionset': function (attribute) {
            var option = attribute.getSelectedOption();
            if (option) return option.value;
            return null;
        },
        'string': function (attribute) {
            return attribute.getValue();
        }
    }
});