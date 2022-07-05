
Ext.define('VIP.store.PersonVadir', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.PersonVadir',
        'VIP.soap.envelopes.vadir.personSearch.FindPersonBySsn',
        'VIP.soap.envelopes.vadir.personSearch.FindPersonByEdipi',
        'VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLnameDob',
        'VIP.soap.envelopes.vadir.personSearch.FindPersonByLnameDob',
        'VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLname'
    ],
    model: 'VIP.model.PersonVadir',

    //remoteFilter: true,
    listeners: {
        beforeload: function (store, operation) {
            var me = this;

            if (me.containsFilter(operation, 'ssn')) {
                this.getProxy().envelopes.read = 'VIP.soap.envelopes.vadir.personSearch.FindPersonBySsn';
                return;
            }

            if (me.containsFilter(operation, 'edipi')) {
                this.getProxy().envelopes.read = 'VIP.soap.envelopes.vadir.personSearch.FindPersonByEdipi';
                return;
            }

            if (me.containsFilter(operation, 'firstName') && me.containsFilter(operation, 'lastName') && me.containsFilter(operation, 'dateOfBirth')) {
                this.getProxy().envelopes.read = 'VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLnameDob';
                return;
            }

            if (me.containsFilter(operation, 'lastName') && me.containsFilter(operation, 'dateOfBirth')) {
                this.getProxy().envelopes.read = 'VIP.soap.envelopes.vadir.personSearch.FindPersonByLnameDob';
                return;
            }

            if (me.containsFilter(operation, 'firstName') && me.containsFilter(operation, 'lastName')) {
                this.getProxy().envelopes.read = 'VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLname';
                return;
            }

        }
    },

    containsFilter: function (operation, property) {
        for (var i = 0; i < operation.filters.length; i++) {
            var filter = operation.filters[i];

            if (filter.property === property) {
                return true;
            }
        }

        return false;
    }

});