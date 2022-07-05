describe("Payment Store", function () {

    describe("Payment Store Test Payee Code", function () {

        it("when testing JAMIEWOOD no paycode of null should have Payments", function () {
            var store;
            runs(function () {
                store = Ext.create("VIP.store.Payment");

                store.load({
                    filters: [{
                        property: 'ParticipantId',
                        value: '13397031'
                    },
			    {
			        property: 'FileNumber',
			        value: '796060339'
			    },
                {
                    property: 'PayeeCode',
                    value: null
                }]
                ,
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
                expect(store.getCount()).toBeGreaterThan(1);
            });
        });

        it("when testing JAMIEWOOD paycode of undefined should have Payments", function () {
            var store = null;
            runs(function () {
                store = Ext.create("VIP.store.Payment");
                store.load({
                    filters: [{
                        property: 'ParticipantId',
                        value: '13397031'
                    },
                        {
                            property: 'FileNumber',
                            value: '796060339'
                        },
                        {
                            property: 'PayeeCode',
                            value: 'undefined'
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
                expect(store.getCount()).toBeGreaterThan(1);
            });
        });

        it("when testing JAMIEWOOD and no paycode should have Payments", function () {
            var store = null;

            runs(function () {
                store = Ext.create("VIP.store.Payment");
                store.load({
                    filters: [{
                        property: 'ParticipantId',
                        value: '13397031'
                    },
			        {
			            property: 'FileNumber',
			            value: '796060339'
			        },
                    ]
                    ,
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
                expect(store.getCount()).toBeGreaterThan(1);
            });
        });

        it("when testing JAMIEWOOD and paycode of '10' should have Payments", function () {
            var store = null;

            runs(function () {
                store = Ext.create("VIP.store.Payment");
                store.load({
                    filters: [{
                        property: 'ParticipantId',
                        value: '13397031'
                    },
			        {
			            property: 'FileNumber',
			            value: '796060339'
			        },
                    {
                        property: 'PayeeCode',
                        value: '10'
                    }]
                    ,
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
                expect(store.getCount()).toBe(0);
            });
        });
    });

    describe("Payment Store No Participant", function () {

        it("when testing JAMIEWOOD, no paycode, no Participant Id, no FileNumber should have Payments", function () {

            var store = null;
            runs(function () {
                store = Ext.create("VIP.store.Payment");
                store.load();
            });

            waitsFor(
                function () {
                    return !store.isLoading();
                },
                "load never completed",
                60000
            );

            runs(function () {
                expect(store.getCount()).toBe(0);
            });
        });

        it("when testing JAMIEWOOD no paycode, Participant Id, no FileNumber should have Payments", function () {
            var store = null;

            runs(function () {
                store = Ext.create("VIP.store.Payment");
                store.load({
                    filters: [{
                        property: 'ParticipantId',
                        value: '13397031'
                    }]
                    ,
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
                expect(store.getCount()).toBe(0);
            });
        });

        it("when testing JAMIEWOOD no paycode, no Participant Id, FileNumber should have Payments", function () {
            var store = null;
            runs(function () {
                store = Ext.create("VIP.store.Payment");
                store.load({
                    filters: [
			        {
			            property: 'FileNumber',
			            value: '796060339'
			        }]
                    ,
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

            runs(function() {
                expect(store.getCount()).toBe(0);
            });
        });
    });
});