describe("fnod Awards Section Module", function () {
    var awardSingleXml, awardMultyXml, awardNoneXml, ratingXml, corpsXml, birlsXml, birlsXml2, birlsXml3, birlsXml4;

    beforeEach(function () {
        awardSingleXml = xmlUtils.parseXmlObjectFromFile('xml/Single - findGeneralInformationByFileNumberResponse.xml').xml;
        awardMultyXml = xmlUtils.parseXmlObjectFromFile('xml/Multy - findGeneralInformationByFileNumberResponse.xml').xml;
        awardNoneXml = xmlUtils.parseXmlObjectFromFile('xml/None - findGeneralInformationByFileNumberResponse.xml').xml;
        ratingXml = xmlUtils.parseXmlObjectFromFile('xml/findRatingDataResponse.xml').xml;
        corpsXml = xmlUtils.parseXmlObjectFromFile('xml/findMilitaryRecordByPtcpntIdResponse.xml').xml;
        birlsXml = xmlUtils.parseXmlObjectFromFile('xml/findBirlsRecordByFileNumberResponse.xml').xml;
        birlsXml2 = xmlUtils.parseXmlObjectFromFile('xml/findBirlsRecordByFileNumberResponse2.xml').xml;
        birlsXml3 = xmlUtils.parseXmlObjectFromFile('xml/findBirlsRecordByFileNumberResponse3.xml').xml;
        birlsXml4 = xmlUtils.parseXmlObjectFromFile('xml/findBirlsRecordByFileNumberResponse4.xml').xml;

        _fnod_.isTest = true;
    });

    it("should get a award type CPL and status terminated from a multy award Xml", function () {
        var award = _fnod_.getAwardInfo(awardMultyXml);

        expect(award.awardType).toEqual("CPL");
        expect(award.awardStatus).toEqual('');
    });

    it("should get a award type CPL and status Authorized from a single award Xml", function () {
        var award = _fnod_.getAwardInfo(awardSingleXml);

        expect(award.awardType).toEqual("CPL");
        expect(award.awardStatus).toEqual("Authorized");
    });

    it("should get a rating of 100 from a rating disability XML response", function () {
        var rating = _fnod_.getDisabilityRatingInfo(ratingXml);

        expect(rating).toEqual("100");
    });

    it("should get a no award from a none award XML", function () {
        var award = _fnod_.getAwardInfo(awardNoneXml);

        expect(award.awardType).toEqual("");
        expect(award.awardStatus).toEqual("");
    });

    it("should get a no award from a empty Xml", function () {
        var award = _fnod_.getAwardInfo('');

        expect(award.awardType).toEqual('');
        expect(award.awardStatus).toEqual('');
    });

    it("should get a Reson of Satisfactory1, discharge of Honorable1, and verified of Y1 from a corp Xml", function () {
        var service = _fnod_.getMilitaryCorpInfo(corpsXml);

        expect(service.separationReason).toEqual('Satisfactory1');
        expect(service.dischargeType).toEqual('Honorable1');
        expect(service.verified).toEqual('Y1');
    });

    it("should get a multyService of true, bos of ARMY, serviceVerified of Y3, and characterService of Hon from a brils Xml", function () {
        var service = _fnod_.getMilitaryBirlsInfo(birlsXml);

        expect(service.multyService).toEqual(true);
        expect(service.bos).toEqual('ARMY');
        expect(service.serviceVerified).toEqual('Y3');
        expect(service.characterService).toEqual('HON');
    });

    it("should get a multyService of flase, bos of NOAA, serviceVerified of Y2, and characterService of HON2 from a brils Xml", function () {
        var service = _fnod_.getMilitaryBirlsInfo(birlsXml2);

        expect(service.multyService).toEqual(false);
        expect(service.bos).toEqual('NOAA');
        expect(service.serviceVerified).toEqual('Y2');
        expect(service.characterService).toEqual('HON2');
    });

    it("should get a multyService of true, bos of CA, serviceVerified of empty, and characterService of HON1 from a brils Xml", function () {
        var service = _fnod_.getMilitaryBirlsInfo(birlsXml3);

        expect(service.multyService).toEqual(true);
        expect(service.bos).toEqual('CG');
        expect(service.serviceVerified).toEqual('');
        expect(service.characterService).toEqual('HON1');
    });

    it("should get a multyService of false, bos of AF, serviceVerified of Y, and characterService of HON from a brils Xml", function () {
        var service = _fnod_.getMilitaryBirlsInfo(birlsXml4);

        expect(service.multyService).toEqual(false);
        expect(service.bos).toEqual('AF');
        expect(service.serviceVerified).toEqual('Y');
        expect(service.characterService).toEqual('HON');
    });
});