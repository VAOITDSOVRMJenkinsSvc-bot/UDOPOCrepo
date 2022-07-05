describe("MOD Validate Module", function () {

    beforeEach(function () {

    });

    it("should be valid when addresstype is domestic and zipcode exists", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850000, '78023']}]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when addresstype is domestic and zipcode empty", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850000, '']}]);

        expect(isValid).toEqual(false);
    });

    it("should be not valid when addresstype is domestic and zipcode null", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850000, null]}]);

        expect(isValid).toEqual(false);
    });

    it("should be not valid when addresstype is domestic and zipcode undefined", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850000, undefined]}]);

        expect(isValid).toEqual(false);
    });

    it("should be not valid when addresstype is not domestic and zipcode exists", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850001, '78021']}]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when addresstype is not domestic and zipcode empty", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850001, '']}]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when addresstype is not domestic and zipcode null", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850001, null]}]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when addresstype is not domestic and zipcode undefined", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists', parameters: [953850001, undefined]}]);

        expect(isValid).toEqual(true);
    });

    it("should valid when validate action does not exists addresstype is domestic and zipcode exists", function () {
        var isValid = _fnod_.validate([{ action: 'notvalid', parameters: [953850000, '78023']}]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when no params passed", function () {
        var isValid = _fnod_.validate([{ action: 'zipCodeExists'}]);

        expect(isValid).toEqual(false);
    });

    it("should be valid when no params passed", function () {
        var isValid = _fnod_.validate(null);

        expect(isValid).toEqual(true);
    });

    it("should be valid when empty params", function () {
        var isValid = _fnod_.validate('');

        expect(isValid).toEqual(true);
    });

    it("should be valid when no undefined params", function () {
        var isValid = _fnod_.validate(undefined);

        expect(isValid).toEqual(true);
    });

    it("should be valid when no params are empty", function () {
        var isValid = _fnod_.validate([]);

        expect(isValid).toEqual(true);
    });

    it("should be valid when action is empty", function () {
        var isValid = _fnod_.validate([{ action: ''}]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when using 2 validations are valid", function () {
        var isValid = _fnod_.validate([
            { action: 'zipCodeExists', parameters: [953850000, '78023'] },
            { action: 'countryUsa', parameters: ['USA'] }
        ]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when using 2 validations and one faile with wrong country", function () {
        var isValid = _fnod_.validate([
            { action: 'zipCodeExists', parameters: [953850000, '78023'] },
            { action: 'countryUsa',    parameters: ['MEXICO'] }
        ]);

        expect(isValid).toEqual(true);
    });

    it("should be not valid when using 2 validations both fails", function () {
        var isValid = _fnod_.validate([
            { action: 'zipCodeExists', parameters: [953850000, ''] },
            { action: 'countryUsa', parameters: ['MEXICO'] }
        ]);

        expect(isValid).toEqual(false);
    });
});