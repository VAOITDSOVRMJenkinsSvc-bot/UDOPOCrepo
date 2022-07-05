describe("ContentPanel test", function () {
    var contentPanelController = null;

    beforeEach(function () {
        if (!contentPanelController) {
            contentPanelController = Application.getController('ContentPanel');
        }
    });

    it('should be defined', function () {
        expect(contentPanelController).toBeDefined();
    });
});

