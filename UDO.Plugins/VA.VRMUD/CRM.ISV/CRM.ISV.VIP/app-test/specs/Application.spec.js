describe('app test', function () {

    it('should have ExtJS 4.0.7 loaded', function () {
        expect(Ext).toBeDefined();
        expect(Ext.getVersion()).toBeTruthy();
        expect(Ext.getVersion().getMajor()).toEqual(4);
        expect(Ext.getVersion().getMinor()).toEqual(0);
        expect(Ext.getVersion().getPatch()).toEqual(7);
    });

    it('should have loaded VIP code', function () {
        expect(VIP).toBeDefined();
    });

});