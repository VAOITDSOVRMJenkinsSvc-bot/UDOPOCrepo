/**
* @author Gordon McShane
*
* Adapted for ExtJS by Jonas Dawson
*
* @class VIP.util.xml.TagBuilder
*
* Used by the XmlSerializer to build individual tags.
*/
Ext.define('VIP.util.xml.TagBuilder', {
    config: {
        prefix: '',
        tagName: undefined,
        attributes: {},
        innerText: ''
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me;
    },

    toString: function () {
        var me = this,
            output,
            prefix = (me.getPrefix() != '') ? me.getPrefix() + ':' : '',
            attributes = '',
            tagName = me.tagName + ' ',
            i = 0;

        for (var key in me.getAttributes()) {
            var attribute = me.getAttributes()[key];
            if (Ext.isString(attribute)) {
                attributes += key + '=' + '"' + me.getAttributes()[key] + '" ';
            }
            else if (attribute.value) {
                var attributePrefix = '';
                if (attribute.prefix) {
                    attributePrefix = attribute.prefix + ':';
                    attributes += attributePrefix + key + '=' + '"' + attribute.value + '" ';
                }
            }

        }

        output = '<' + Ext.String.trim(prefix + tagName + attributes) + '>' + me.innerText + '</' + Ext.String.trim(prefix + tagName) + '>';

        return output;
    }
});