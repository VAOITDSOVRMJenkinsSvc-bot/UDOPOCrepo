/**
* @author Gordon McShane
* Adapted by Jonas Dawson for ExtJS
*
* @class VIP.util.xml.Serializer
*
* Serializes formatted JSON into XML text.
*/
Ext.define('VIP.util.xml.Serializer', {
    requires: [
        'VIP.util.xml.TagBuilder'
    ],

    serialize: function (data, rootNodeName) {
        var me = this;

        if (!rootNodeName) { rootNodeName = 'xml'; }

        var numberOfProperties = 0,
            dataElementKey;

        for (var key in data) {
            if (numberOfProperties > 1) { break; }
            dataElementKey = key;
            numberOfProperties++;
        }

        if (numberOfProperties == 1) {
            rootNodeName = dataElementKey;
            data = data[rootNodeName];
        }

        return me.internalSerialize({ namespaces: {},
            currentPrefix: ''

        }, [], rootNodeName, data);
    },

    internalSerialize: function (config, definedNamespaces, propertyName, data) {
        var me = this,
            output = '',
            prefix = '',
            tagBuilder;

        if (propertyName == 'value') { return data; }
        if (propertyName == 'cdata') { return '<![CDATA[' + data + ']]>'; }

        if (!Ext.isEmpty(propertyName) && propertyName.substring(0, 1) == '@') {
            propertyName = propertyName.substring(1);
        }

        if (data.namespaces) {
            Ext.applyIf(config.namespaces, data.namespaces);
        }

        if (data.namespace && config.namespaces[data.namespace] && data.namespace != '') {
            prefix = data.namespace;
        }
        else if (Ext.isEmpty(data.namespace, true) && config.currentPrefix) {
            prefix = config.currentPrefix;
        }
        else {
            prefix = '';
        }

        tagBuilder = Ext.create('VIP.util.xml.TagBuilder', {
            tagName: propertyName,
            prefix: prefix
        });

        if (prefix != '' && Ext.Array.indexOf(definedNamespaces, prefix) < 0) {
            tagBuilder.getAttributes()['xmlns:' + prefix] = config.namespaces[prefix];
            definedNamespaces.push(prefix);
        }

        if (data.attributes) {
            for (var attributeName in data.attributes) {

                if (!Ext.isEmpty(data.attributes[attributeName].prefix) && Ext.Array.indexOf(definedNamespaces, data.attributes[attributeName].prefix) < 0) {
                    tagBuilder.getAttributes()['xmlns:' + data.attributes[attributeName].prefix] = config.namespaces[data.attributes[attributeName].prefix];
                    definedNamespaces.push(data.attributes[attributeName].prefix);
                }
                tagBuilder.getAttributes()[attributeName] = data.attributes[attributeName];
            }
        }

        var childNamespaces = {};

        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                var node = data[key];
                if (node && node.namespace) {
                    childNamespaces[node.namespace] = childNamespaces[node.namespace] ? childNamespaces[node.namespace] + 1 : 1;
                }
            }
        }

        for (var ns in childNamespaces) {
            if (childNamespaces[ns] > 1 && Ext.Array.indexOf(definedNamespaces, ns) < 0) {
                tagBuilder.getAttributes()['xmlns:' + ns] = config.namespaces[ns];
                definedNamespaces.push(ns);
            }
        }




        if (typeof data == 'string') {
            tagBuilder.setInnerText(data);
        }
        else if (Ext.isArray(data)) {
            for (var i = 0; i < data.length; i++) {
                var record = data[i];

                for (var key in record) {
                    if (record.hasOwnProperty(key) && key != 'namespaces' && key != 'namespace' && key != 'attributes') {
                        output += me.internalSerialize({ namespaces: config.namespaces, currentPrefix: prefix },
                            Ext.Array.clone(definedNamespaces), key, record[key]);
                    }
                }
            }
            tagBuilder.setInnerText(output);
        }
        else {
            for (var key in data) {
                if (data.hasOwnProperty(key) && key != 'namespaces' && key != 'namespace' && key != 'attributes') {
                    output += me.internalSerialize({ namespaces: config.namespaces,
                        currentPrefix: prefix
                    },
                    Ext.Array.clone(definedNamespaces), key, data[key]);
                }
            }
            tagBuilder.setInnerText(output);
        }

        return tagBuilder.toString();
    }
});