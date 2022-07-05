/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

Commercial Usage
Licensees holding valid commercial licenses may use this file in accordance with the Commercial Software License Agreement provided with the Software or, alternatively, in accordance with the terms contained in a written agreement between you and Sencha.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
/**
 * @class Ext.state.LocalStorageProvider
 * @extends Ext.state.Provider
 * A Provider implementation which saves and retrieves state via the HTML5 localStorage object.
 * If the browser does not support local storage, an exception will be thrown upon instantiating
 * this class.
 */

Ext.define('Ext.state.LocalStorageProvider', {
    /* Begin Definitions */
    
    extend: 'Ext.state.Provider',
    
    alias: 'state.localstorage',
    
    /* End Definitions */
   
    constructor: function(){
        var me = this;
        me.callParent(arguments);
        me.store = me.getStorageObject();
        me.state = me.readLocalStorage();
    },
    
    readLocalStorage: function(){
        var store = this.store,
            i = 0,
            len = store.length,
            prefix = this.prefix,
            prefixLen = prefix.length,
            data = {},
            key;
            
        for (; i < len; ++i) {
            key = store.key(i);
            if (key.substring(0, prefixLen) == prefix) {
                data[key.substr(prefixLen)] = this.decodeValue(store.getItem(key));
            }            
        }
        return data;
    },
    
    set : function(name, value){
        var me = this;
        
        me.clear(name);
        if (typeof value == "undefined" || value === null) {
            return;
        }
        me.store.setItem(me.prefix + name, me.encodeValue(value));
        me.callParent(arguments);
    },

    // private
    clear : function(name){
        this.store.removeItem(this.prefix + name);
        this.callParent(arguments);
    },
    
    getStorageObject: function(){
        try {
            var supports = 'localStorage' in window && window['localStorage'] !== null;
            if (supports) {
                return window.localStorage;
            }
        } catch (e) {
            return false;
        }
        //<debug>
        Ext.Error.raise('LocalStorage is not supported by the current browser');
        //</debug>
    }    
});

