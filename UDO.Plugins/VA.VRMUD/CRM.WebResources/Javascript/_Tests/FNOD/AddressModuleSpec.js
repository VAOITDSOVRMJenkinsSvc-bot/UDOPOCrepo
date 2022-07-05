describe("MOD Addresses Module", function () {
    beforeEach(function () {
    });

    describe("MOD Addresses - Testing getAddress Function", function() {

        it("should get address correct WHEN multy addresses domestic address normal", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses.xml").xml, 'Mailing');

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual('FL');
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('USA');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(false);

        });

        it("should get address correct WHEN multy addresses international address normal", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses1.xml").xml, 'Mailing');

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual('FL');
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('MEXICO');
            expect(address.forgeinPostalCode).toEqual('14000023');
            expect(address.mltyPostalType).toEqual('AP');
            expect(address.mltyPostOfficeType).toEqual('DPO');
            expect(address.isEmpty).toEqual(false);

        });

        it("should get address correct WHEN multy addresses international address all", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses3.xml").xml, 'Mailing');

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual('SUITE 333');
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual('FL');
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('MEXICO');
            expect(address.forgeinPostalCode).toEqual('14000023');
            expect(address.mltyPostalType).toEqual('AA');
            expect(address.mltyPostOfficeType).toEqual('FPO');
            expect(address.isEmpty).toEqual(false);

        });

        it("should get address correct WHEN multy addresses address domestic", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses4.xml").xml, 'Mailing');

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual('SUITE 333');
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual('FL');
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('USA');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(false);

        });

        it("should get isEmpty correct WHEN no address type", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses4.xml").xml, '');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get isEmpty correct WHEN no xml provided", function() {
            var address = _fnod_.getAddress('', 'Mailing');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get isEmpty correct WHEN no invalid xml provided", function() {
            var address = _fnod_.getAddress('test', 'Mailing');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get isEmpty correct WHEN no valid but no addresses xml provided", function() {
            var address = _fnod_.getAddress('<test><test>', 'Mailing');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get isEmpty correct WHEN invalid address type", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses4.xml").xml, 'Test');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get isEmpty correct WHEN no mailing address", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses5.xml").xml, 'Mailing');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get isEmpty correct WHEN no mailing address", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses6.xml").xml, 'Mailing');

            expect(address.address1).toEqual('');
            expect(address.address2).toEqual('');
            expect(address.address3).toEqual('');
            expect(address.city).toEqual('');
            expect(address.state).toEqual('');
            expect(address.zip).toEqual('');
            expect(address.country).toEqual('');
            expect(address.forgeinPostalCode).toEqual('');
            expect(address.mltyPostalType).toEqual('');
            expect(address.mltyPostOfficeType).toEqual('');
            expect(address.isEmpty).toEqual(true);
        });

        it("should get address formated WHEN Domestic info", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses4.xml").xml, 'Mailing');

            var fullAddress = _fnod_.parseAddress(address);
            console.log(fullAddress)

            expect(fullAddress).toEqual('21521 EASY STREET\nAPT 204\nSUITE 333\nTAMPA, FL 33626\nUSA');
        });

        it("should get address formated WHEN International info", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses3.xml").xml, 'Mailing');

            var fullAddress = _fnod_.parseAddress(address);
            console.log(fullAddress)

            expect(fullAddress).toEqual('21521 EASY STREET\nAPT 204\nSUITE 333\nTAMPA, FL\nMEXICO\n14000023\nFPO AA');
        });

        it("should get address formated WHEN Domestic info only one address", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile("xml/Addresses7.xml").xml, 'Mailing');

            var fullAddress = _fnod_.parseAddress(address);
            console.log(fullAddress)

            expect(fullAddress).toEqual('21521 EASY STREET\nTAMPA, FL 33626\nUSA');
        });

        it("should get empty formated WHEN no xml", function() {
            var address = _fnod_.getAddress(xmlUtils.parseXmlObjectFromFile('').xml, 'Mailing');

            var fullAddress = _fnod_.parseAddress(address);
            console.log(fullAddress)

            expect(fullAddress).toEqual('');
        });
    });

    describe("MOD Addresses - Testing prepareLastKnownAddressForSpouseFields Function", function () {

        it("should get address correct WHEN multy addresses domestic address normal", function () {
            var states = [{ text: 'FL', value: 23456}];
            var address = _fnod_.prepareLastKnownAddressForSpouseFields(xmlUtils.parseXmlObjectFromFile("xml/Addresses.xml").xml, states);

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual(null);
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual(23456);
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('USA');
            expect(address.forgeinPostalCode).toEqual(null);
            expect(address.mltyPostalType).toEqual(null);
            expect(address.mltyPostOfficeType).toEqual(null);
            expect(address.spouseAddressType).toEqual(953850000);
        });

        it("should get undefined address WHEN no xml", function () {
            var states = [{ text: 'FL', value: 23456}];
            var address = _fnod_.prepareLastKnownAddressForSpouseFields('', states);

            expect(address).toEqual(undefined);
        });

        it("should get address correct WHEN multy addresses international", function () {
            var states = [{ text: 'AL', value: 111111 }, { text: 'FL', value: 23456 }, { text: 'AZ', value: 111111}];
            var address = _fnod_.prepareLastKnownAddressForSpouseFields(xmlUtils.parseXmlObjectFromFile("xml/Addresses1.xml").xml, states);

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual(null);
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual(23456);
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('MEXICO');
            expect(address.forgeinPostalCode).toEqual('14000023');
            expect(address.mltyPostalType).toEqual(953850002);
            expect(address.mltyPostOfficeType).toEqual(953850001);
            expect(address.spouseAddressType).toEqual(953850002);
        });

        it("should get address nulls WHEN address dos not have any fields", function () {
            var states = [{ text: 'AL', value: 111111 }, { text: 'FL', value: 23456 }, { text: 'AZ', value: 111111}];
            var address = _fnod_.prepareLastKnownAddressForSpouseFields(xmlUtils.parseXmlObjectFromFile("xml/Addresses8.xml").xml, states);

            expect(address.address1).toEqual(null);
            expect(address.address2).toEqual(null);
            expect(address.address3).toEqual(null);
            expect(address.city).toEqual(null);
            expect(address.state).toEqual(null);
            expect(address.zip).toEqual(null);
            expect(address.country).toEqual(null);
            expect(address.forgeinPostalCode).toEqual(null);
            expect(address.mltyPostalType).toEqual(null);
            expect(address.mltyPostOfficeType).toEqual(null);
            expect(address.spouseAddressType).toEqual(953850001);


        });

        it("should get address all filled out WHEN multy addresses has all", function () {
            var states = [{ text: 'AL', value: 111111 }, { text: 'FL', value: 23456 }, { text: 'AZ', value: 111111}];
            var address = _fnod_.prepareLastKnownAddressForSpouseFields(xmlUtils.parseXmlObjectFromFile("xml/Addresses3.xml").xml, states);

            expect(address.address1).toEqual('21521 EASY STREET');
            expect(address.address2).toEqual('APT 204');
            expect(address.address3).toEqual('SUITE 333');
            expect(address.city).toEqual('TAMPA');
            expect(address.state).toEqual(23456);
            expect(address.zip).toEqual('33626');
            expect(address.country).toEqual('MEXICO');
            expect(address.forgeinPostalCode).toEqual('14000023');
            expect(address.mltyPostalType).toEqual(953850000);
            expect(address.mltyPostOfficeType).toEqual(953850002);
            expect(address.spouseAddressType).toEqual(953850002);
        });
    });
});