/**
* @author
*
* @class VIP.mixin.LoadStores
*
* Mixin for the service request and VAI
*/
Ext.define('VIP.mixin.Stores', {
    
    loadStore: function (store, filters, callback) {
        var deferred = $.Deferred();

        store.load({
            filters: filters,
            callback: function (records, operation, success) {
                if (success) {
                    if (callback)
                        callback(records, operation, success);

                    deferred.resolve();
                } else {
                    deferred.reject();
                }
            },
            scope: this
        });
        return deferred.promise();
    }
});