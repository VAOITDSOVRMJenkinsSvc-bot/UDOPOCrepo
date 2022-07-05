describe("VADIR Contact Info Store Load test", function () {
    describe("ContactInfo Load", function () {

        it("when searching by edipi should find person", function () {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.personVadir.ContactInfo");

                store.load({
                    filters: [{
                        property: 'edipi',
                        value: '1021267380'
                    }],

                    callback: function (records, operation, success) {
                        console.log("store loaded")
                    },
                    scope: this
                });

            });

            waitsFor(
                function () {
                    return !store.isLoading();
                },
                "load never completed",
                60000
            );

            runs(function () {
                expect(store.getCount()).toBeGreaterThan(0);

                expect(store.getAt(0).data.edipi).toEqual('1021267380');

                expect(store.getAt(0).AddressesStore.getCount()).toEqual(2);
                expect(store.getAt(0).PhonesStore.getCount()).toEqual(3);
                expect(store.getAt(0).EmailsStore).toBeUndefined();
            });

        });

        it("when searching by edipi should find person using a promise", function () {
            var store, store2;
            store = Ext.create("VIP.store.personVadir.ContactInfo");
            store2 = Ext.create("VIP.store.personVadir.ContactInfo");

            var LoadStore = function (store) {
                var deferred = $.Deferred();

                store.load({
                    filters: [{
                        property: 'edipi',
                        value: '1021267380'
                    }],

                    callback: function (records, operation, success) {
                        if (success) {
                            deferred.resolve();
                        } else {
                            deferred.reject();
                        }

                    },
                    scope: this
                });

                return deferred.promise();
            };

            $.when(LoadStore(store), LoadStore(store2))
                .then(function (result1, result2) {
                    
                    expect(store.getCount()).toBeGreaterThan(0);

                    expect(store.getAt(0).data.edipi).toEqual('1021267380');

                    expect(store.getAt(0).AddressesStore.getCount()).toEqual(2);
                    expect(store.getAt(0).PhonesStore.getCount()).toEqual(3);
                    expect(store.getAt(0).EmailsStore).toBeUndefined();
                    
                }).fail(function (xhr, textStatus, errorThrown) {
                    console.log('error');
                    console.log(errorThrown);
                    
                })


        });

    });
});