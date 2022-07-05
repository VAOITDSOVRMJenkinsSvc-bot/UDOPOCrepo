describe("VADIR Person Search Store", function () {

    describe("Person Search by SSN", function () {
        it("when searching with ssn should find person with alias", function () {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.PersonVadir");

                store.load({
                    filters: [{
                        property: 'ssn',
                        value: '810590059'
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
                expect(store.getAt(0).data.ssn).toEqual('810590059');
                //expect(store.getAt(0).aliasesStore.data.items.length).toEqual(2);
            });

        });
        it("when searching with no SSN should return a fault", function () {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.PersonVadir");

                store.load({
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
                expect(store.getCount()).toEqual(0);
            });

        });

    });

    describe("Person Search by firstname, lastname, dob", function() {

        it("when searching for Robin, web, 1980-03-10 should find person", function() {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.PersonVadir");

                store.load({
                    filters: [
                        {
                            property: 'firstName',
                            value: 'Robin'
                        },
                        {
                            property: 'lastName',
                            value: 'Webb'
                        },
                        {
                            property: 'dateOfBirth',
                            value: '1980-03-10'
                        }
                    ],

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
                expect(store.getCount()).toEqual(2);
            });

        });
    });

    describe("Person Search by lastname, dob", function () {

        it("when searching for web, 1980-03-10 should find person", function () {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.PersonVadir");

                store.load({
                    filters: [
                        {
                            property: 'lastName',
                            value: 'Webb'
                        },
                        {
                            property: 'dateOfBirth',
                            value: '1980-03-10'
                        }
                    ],

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
                expect(store.getCount()).toEqual(4);
            });

        });

    });

    describe("Person Search by firstname, lastname", function () {

        it("when searching for Robin, web should find person", function () {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.PersonVadir");

                store.load({
                    filters: [
                        {
                            property: 'firstName',
                            value: 'Robin'
                        },
                        {
                            property: 'lastName',
                            value: 'Webb'
                        }
                    ],

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
                expect(store.getCount()).toEqual(2);
            });

        });

    });

    describe("Person Search by edipi", function () {

        it("when searching for edipi should find person", function () {
            var store;

            runs(function () {
                store = Ext.create("VIP.store.PersonVadir");

                store.load({
                    filters: [
                        {
                            property: 'edipi',
                            value: '1013590059'
                        }
                    ],

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
                expect(store.getCount()).toEqual(1);
            });

        });

    });

    
});