/**
* @author Jonas Dawson
*
* @class VIP.util.Xml
*
* Various utilities for manipulating XML documents
*/
Ext.define('VIP.util.Xml', {
    singleton: true,

    wrap: function (parentNodeName, nodeToWrapName, root, xmlDoc) {
        if (!Ext.isEmpty(xmlDoc)) {
            //check if your new node already exists. If it does, exit.
            if (xmlDoc.getElementsByTagName(parentNodeName).length != 0) {
                return;
            }
            
            var rootNodes = xmlDoc.getElementsByTagName(root),
                parentNode = xmlDoc.createElement(parentNodeName);

            Ext.Array.each(rootNodes, function (name, index, allRootNodes) {
                var rootNode = rootNodes[index],
                    childNodes = rootNode.getElementsByTagName(nodeToWrapName),
                    wrapNode = parentNode.cloneNode(false);

                rootNode.appendChild(wrapNode);

                Ext.Array.each(childNodes, function (childNode, index, allChildNodes) {
                    wrapNode.appendChild(childNode);
                }, this);

            }, this);
        }
    }
});