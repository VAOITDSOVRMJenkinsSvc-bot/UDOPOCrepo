describe("Tetyana Refactor Function To Make them Testable", function () {
    beforeEach(function () {
    });

    describe("Full Name function", function () {

        it("SHOULD get empty WHEN first name is empty, last name is empty, and full name is empty", function () {
            var fullName = getfullName('', '', '')
            console.log(fullName);
            expect(fullName).toEqual('')
        });

        it("SHOULD get 'Mark Smith WHEN first name is empty, last name is empty, and full name is Mark Smith", function () {
            var fullName = getfullName(null, null, 'Mark Smith')
            console.log(fullName);
            expect(fullName).toEqual('Mark Smith')
        });

        it("SHOULD get Mark Smith WHEN first name is Mark, last name is Smith, and full name is null", function () {
            var fullName = getfullName('Mark', 'Smith', null)
            console.log(fullName);
            expect(fullName).toEqual('Mark Smith')
        });

        it("SHOULD get Smith WHEN first name is null, last name is Smith, and full name is null", function () {
            var fullName = getfullName(null, 'Smith', null)
            console.log(fullName);
            expect(fullName).toEqual('Smith')
        });

        it("SHOULD get undefined WHEN first name is null, last name is null, and full name is null", function () {
            var fullName = getfullName(null, null, null)
            console.log(fullName);
            expect(fullName).toEqual(undefined)
        });

        it("SHOULD get undefined WHEN first name is undefined, last name is undefined, and full name is undefined", function () {
            var fullName = getfullName(undefined, undefined, undefined)
            console.log(fullName);
            expect(fullName).toEqual(undefined)
        });

    });

    describe("To Upper Case", function () {

        it("SHOULD get undefined WHEN word name is empty", function () {
            var word = toUpperCaseFirstCharacter('')
            console.log(word);
            expect(word).toEqual(undefined)
        });

        it("SHOULD get null WHEN word name is null", function () {
            var word = toUpperCaseFirstCharacter(null)
            console.log(word);
            expect(word).toEqual(null)
        });

        it("SHOULD get upper case first character WHEN does not has it upper case", function () {
            var word = toUpperCaseFirstCharacter('catapult')
            console.log(word);
            expect(word).toEqual('Catapult')
        });

        it("SHOULD get upper case first character WHEN does has it upper case", function () {
            var word = toUpperCaseFirstCharacter('Catapult')
            console.log(word);
            expect(word).toEqual('Catapult')
        });

        it("SHOULD get upper case first character WHEN all characters are upper case", function () {
            var word = toUpperCaseFirstCharacter('CATAPULT')
            console.log(word);
            expect(word).toEqual('Catapult')
        });

        it("SHOULD get upper case first character WHEN the first character is not upper case and the rest are", function () {
            var word = toUpperCaseFirstCharacter('cATAPULT')
            console.log(word);
            expect(word).toEqual('Catapult')
        });

    });
});