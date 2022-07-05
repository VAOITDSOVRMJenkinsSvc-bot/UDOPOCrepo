describe("PaymentInformation Store Test", function () {
    var store = null, controller = null;

    beforeEach(function () {
        if (!controller) {
            controller = Application.getController('PaymentInformation');
        }

        if (!store) {
            store = controller.getStore('Payment');
        }

        expect(store).toBeTruthy();

        store.load({
            filters: [{
                property: 'ParticipantId',
                value: '13397031'
            },
				{
				    property: 'FileNumber',
				    value: '796060339'
				}
                ],
            callback: function (records, operation, success) {
                console.log("store loaded")
            },
            scope: this
        });

        waitsFor(
            function () {
                return !store.isLoading();
            },
            "load never completed",
            60000
        );
    });

    it("should have Payments", function () {
        expect(store.getCount()).toBeGreaterThan(1);
    });

});