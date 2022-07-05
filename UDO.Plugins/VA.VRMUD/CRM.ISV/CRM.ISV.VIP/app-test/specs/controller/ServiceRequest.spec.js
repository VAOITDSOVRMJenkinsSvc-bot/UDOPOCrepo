describe('VIP.controller.services.ServiceRequest Test Cases', function () {
    var serviceRequestController = null;

    beforeEach(function () {
        if (!serviceRequestController) {
            serviceRequestController = Application.getController('services.ServiceRequest');
            serviceRequestController.init();
        }

    });

    it('should be defined', function () {
        serviceRequestController.getParticipantSRInfo();
        expect(serviceRequestController).toBeDefined();
    });

});