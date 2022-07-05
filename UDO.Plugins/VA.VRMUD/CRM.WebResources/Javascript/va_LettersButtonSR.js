//get logged on user
function Getinfo() {
    var fullname;
    var context;
    var serverUrl;
    var UserID;
    var ODataPath;
    context = Xrm.Page.context;
    serverUrl = context.getServerUrl();
    UserID = context.getUserId();
    ODataPath = serverUrl + "XRMServices/2011/OrganizationData.svc";
    var retrieveUserReq = new XMLHttpRequest();
    retrieveUserReq.open("GET", ODataPath + "/SystemUserSet(guid'" + UserID + "')", false);
    retrieveUserReq.setRequestHeader("Accept", "application/json");
    retrieveUserReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    retrieveUserReq.onreadystatechange = function () {
        fullname = retrieveUserReqCallBack(this);
    };
    retrieveUserReq.send();
    return fullname;
}

function retrieveUserReqCallBack(retrieveUserReq) {
    if (retrieveUserReq.readyState == 4 /* complete */) {
        if (retrieveUserReq.status == 200) {
            var retrievedUser = (this.parent.JSON) ? this.parent.JSON.parse(retrieveUserReq.responseText).d : this.JSON.parse(retrieveUserReq.responseText).d;
            if (retrievedUser.FullName != null)
                return retrievedUser.FullName;

            else {
                alert("Error in Fetching User data");
            }
        }
    }
}


//get reports
var reports = {
    list: [],

    getReports: function () {
        var len, doRequest, me = this,
            url = Xrm.Page.context.getServerUrl() + 'xrmservices/2011/OrganizationData.svc/ReportSet?$select=ReportId,Name';

        doRequest = function (url) {
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json'
            }).done(function (data) {
                if (data && data.d && data.d.results && data.d.results.length > 0) {
                    len = data.d.results.length;

                    for (var i = 0; i < len; i++) {
                        me.list.push({ name: data.d.results[i].Name, reportId: data.d.results[i].ReportId });
                    }

                    if (data.d.__next) {
                        doRequest(data.d.__next)
                    }
                }
            }).fail(function () {
                // console.log("fail")
            });
        };

        doRequest(url);
    }
};


reports.getReports();

function Letters(CommandProperties) {

    var action = Xrm.Page.getAttribute("va_action");
    var menuXmlECCAction = '<Menu Id=\"ISV.DynamicMenu\">' +
        '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
        '<Controls Id=\"ISV.Dynamic.Controls\">' +
        '<Button Id=\"ISV.Dynamic.119\" Command=\"ISV.SearchCommand\" Sequence=\"10\" LabelText=\"119- Report of Contact\" Alt=\"119- Report of Contact\" />' +
        '<Button Id=\"ISV.Dynamic.AmountLetterGross\" Command=\"ISV.SearchCommand\" Sequence=\"60\" LabelText=\"Amount Letter Gross\" Alt=\"Amount Letter Gross\" />' +
        '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\" />' +
        '<Button Id=\"ISV.Dynamic.CommissaryLetter\" Command=\"ISV.SearchCommand\" Sequence=\"80\" LabelText=\"Commissary Letter\" Alt=\"Commissary Letter\" />' +
        '<Button Id=\"ISV.Dynamic.CommissaryLetterSurvivingSpouse\" Command=\"ISV.SearchCommand\" Sequence=\"82\" LabelText=\"Commissary Letter Surviving Spouse\" Alt=\"Commissary Letter Surviving Spouse\" />' +
        '<Button Id=\"ISV.Dynamic.CommissaryLetterwithFutureExam\" Command=\"ISV.SearchCommand\" Sequence=\"83\" LabelText=\"Commissary Letter with Future Exam\" Alt=\"Commissary Letter with Future Exam\" />' +
        '<Button Id=\"ISV.Dynamic.DeathBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"84\" LabelText=\"Death Benefits Letter\" Alt=\"Death Benefits Letter\" />' +
        '<Button Id=\"ISV.Dynamic.DisabilityBreakdownLetter\" Command=\"ISV.SearchCommand\" Sequence=\"85\" LabelText=\"Disability Breakdown Letter\" Alt=\"Disability Breakdown Letter\" />' +
        '<Button Id=\"ISV.Dynamic.FaxCoverSheet\" Command=\"ISV.SearchCommand\" Sequence=\"86\" LabelText=\"Fax Cover Sheet\" Alt=\"Fax Cover Sheet\" />' +
        '<Button Id=\"ISV.Dynamic.IncomeAmountLetter\" Command=\"ISV.SearchCommand\" Sequence=\"87\" LabelText=\"Income Amount Letter\" Alt=\"Income Amount Letter\" />' +
        //'<Button Id=\"ISV.Dynamic.InformalClaimLetter\" Command=\"ISV.SearchCommand\" Sequence=\"88\" LabelText=\"Informal Claim Letter - AB10\" Alt=\"Informal Claim Letter - AB10\" />' +
        '<Button Id=\"ISV.Dynamic.MODMemorandum\" Command=\"ISV.SearchCommand\" Sequence=\"89\" LabelText=\"MOD Memorandum\" Alt=\"MOD Memorandum\" />' +
        '<Button Id=\"ISV.Dynamic.MultiplePaymentLetter\" Command=\"ISV.SearchCommand\" Sequence=\"90\" LabelText=\"Multiple Payment Letter\" Alt=\"Multiple Payment Letter\" />' +
        '<Button Id=\"ISV.Dynamic.NoBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"91\" LabelText=\"No Benefits Letter\" Alt=\"No Benefits Letter\" />' +
        '<Button Id=\"ISV.Dynamic.NOKLetter\" Command=\"ISV.SearchCommand\" Sequence=\"92\" LabelText=\"NOK Letter\" Alt=\"NOK Letter\" />' +
        '<Button Id=\"ISV.Dynamic.PreferenceLetters\" Command=\"ISV.SearchCommand\" Sequence=\"93\" LabelText=\"Preference Letters\" Alt=\"Preference Letters\" />' +
        '<Button Id=\"ISV.Dynamic.PropertyTaxDMVRegistration\" Command=\"ISV.SearchCommand\" Sequence=\"94\" LabelText=\"Property Tax DMV Registration - MT\" Alt=\"Property Tax DMV Registration - MT\" />' +
        '<Button Id=\"ISV.Dynamic.PropertyTaxExemption\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Property Tax Exemption - CA - LA\" Alt=\"Property Tax Exemption - CA - LA\" />' +
        '<Button Id=\"ISV.Dynamic.RealEstate\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Real Estate - License Fee Tax Exemption\" Alt=\"Real Estate - License Fee Tax Exemption\" />' +
        '<Button Id=\"ISV.Dynamic.ServiceVerification\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Service Verification\" Alt=\"Service Verification\" />' +
        '<Button Id=\"ISV.Dynamic.SummaryofBenefitsSurvivingSpouseLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Surviving Spouse Letter\" Alt=\"Summary of Benefits Surviving Spouse Letter\" />' +
        '<Button Id=\"ISV.Dynamic.SummaryofBenefitsVeteransLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Veterans Letter\" Alt=\"Summary of Benefits Veterans Letter\" />' +
        '<Button Id=\"ISV.Dynamic.VeteranPercentageLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran Percentage Letter\" Alt=\"Veteran Percentage Letter\" />' +
        '<Button Id=\"ISV.Dynamic.VeteranPTLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran PT Letter\" Alt=\"Veteran PT Letter\" />' +

        '</Controls>' +
        '</MenuSection></Menu>';
    var menuXmlAction = '<Menu Id=\"ISV.DynamicMenu\">' +
        '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
        '<Controls Id=\"ISV.Dynamic.Controls\">' +
        '<Button Id=\"ISV.Dynamic.0820\" Command=\"ISV.SearchCommand\" Sequence=\"20\" LabelText=\"27-0820- Report of General Information\" Alt=\"27-0820- Report of General Information\" />' +
        '<Button Id=\"ISV.Dynamic.0820a\" Command=\"ISV.SearchCommand\" Sequence=\"30\" LabelText=\"27- 0820a-Report of First Notice of Death\" Alt=\"27-0820a-Report of First Notice of Death\" />' +
        '<Button Id=\"ISV.Dynamic.0820d\" Command=\"ISV.SearchCommand\" Sequence=\"40\" LabelText=\"27- 0820d-Report of Non Reciept of Payment\" Alt=\"27-0820d-Report of Non Reciept of Payment\" />' +
        '<Button Id=\"ISV.Dynamic.0820f\" Command=\"ISV.SearchCommand\" Sequence=\"50\" LabelText=\"27- 0820f-Report of Month of Death\" Alt=\"27-0820f-Report of Month of Death\" />' +
        '<Button Id=\"ISV.Dynamic.AmountLetterGross\" Command=\"ISV.SearchCommand\" Sequence=\"60\" LabelText=\"Amount Letter Gross\" Alt=\"Amount Letter Gross\" />' +
        '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\" />' +
        '<Button Id=\"ISV.Dynamic.CommissaryLetter\" Command=\"ISV.SearchCommand\" Sequence=\"80\" LabelText=\"Commissary Letter\" Alt=\"Commissary Letter\" />' +
        '<Button Id=\"ISV.Dynamic.CommissaryLetterSurvivingSpouse\" Command=\"ISV.SearchCommand\" Sequence=\"82\" LabelText=\"Commissary Letter Surviving Spouse\" Alt=\"Commissary Letter Surviving Spouse\" />' +
        '<Button Id=\"ISV.Dynamic.CommissaryLetterwithFutureExam\" Command=\"ISV.SearchCommand\" Sequence=\"83\" LabelText=\"Commissary Letter with Future Exam\" Alt=\"Commissary Letter with Future Exam\" />' +
        '<Button Id=\"ISV.Dynamic.DeathBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"84\" LabelText=\"Death Benefits Letter\" Alt=\"Death Benefits Letter\" />' +
        '<Button Id=\"ISV.Dynamic.DisabilityBreakdownLetter\" Command=\"ISV.SearchCommand\" Sequence=\"85\" LabelText=\"Disability Breakdown Letter\" Alt=\"Disability Breakdown Letter\" />' +
        '<Button Id=\"ISV.Dynamic.FaxCoverSheet\" Command=\"ISV.SearchCommand\" Sequence=\"86\" LabelText=\"Fax Cover Sheet\" Alt=\"Fax Cover Sheet\" />' +
        '<Button Id=\"ISV.Dynamic.IncomeAmountLetter\" Command=\"ISV.SearchCommand\" Sequence=\"87\" LabelText=\"Income Amount Letter\" Alt=\"Income Amount Letter\" />' +
        //'<Button Id=\"ISV.Dynamic.InformalClaimLetter\" Command=\"ISV.SearchCommand\" Sequence=\"88\" LabelText=\"Informal Claim Letter - AB10\" Alt=\"Informal Claim Letter - AB10\" />' +
        '<Button Id=\"ISV.Dynamic.MODMemorandum\" Command=\"ISV.SearchCommand\" Sequence=\"89\" LabelText=\"MOD Memorandum\" Alt=\"MOD Memorandum\" />' +
        '<Button Id=\"ISV.Dynamic.MultiplePaymentLetter\" Command=\"ISV.SearchCommand\" Sequence=\"90\" LabelText=\"Multiple Payment Letter\" Alt=\"Multiple Payment Letter\" />' +
        '<Button Id=\"ISV.Dynamic.NoBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"91\" LabelText=\"No Benefits Letter\" Alt=\"No Benefits Letter\" />' +
        '<Button Id=\"ISV.Dynamic.NOKLetter\" Command=\"ISV.SearchCommand\" Sequence=\"92\" LabelText=\"NOK Letter\" Alt=\"NOK Letter\" />' +
        '<Button Id=\"ISV.Dynamic.PreferenceLetters\" Command=\"ISV.SearchCommand\" Sequence=\"93\" LabelText=\"Preference Letters\" Alt=\"Preference Letters\" />' +
        '<Button Id=\"ISV.Dynamic.PropertyTaxDMVRegistration\" Command=\"ISV.SearchCommand\" Sequence=\"94\" LabelText=\"Property Tax DMV Registration - MT\" Alt=\"Property Tax DMV Registration - MT\" />' +
        '<Button Id=\"ISV.Dynamic.PropertyTaxExemption\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Property Tax Exemption - CA - LA\" Alt=\"Property Tax Exemption - CA - LA\" />' +
        '<Button Id=\"ISV.Dynamic.RealEstate\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Real Estate - License Fee Tax Exemption\" Alt=\"Real Estate - License Fee Tax Exemption\" />' +
        '<Button Id=\"ISV.Dynamic.ServiceVerification\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Service Verification\" Alt=\"Service Verification\" />' +
        '<Button Id=\"ISV.Dynamic.SummaryofBenefitsSurvivingSpouseLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Surviving Spouse Letter\" Alt=\"Summary of Benefits Surviving Spouse Letter\" />' +
        '<Button Id=\"ISV.Dynamic.SummaryofBenefitsVeteransLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Veterans Letter\" Alt=\"Summary of Benefits Veterans Letter\" />' +
        '<Button Id=\"ISV.Dynamic.VeteranPercentageLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran Percentage Letter\" Alt=\"Veteran Percentage Letter\" />' +
        '<Button Id=\"ISV.Dynamic.VeteranPTLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran PT Letter\" Alt=\"Veteran PT Letter\" />' +

        '</Controls>' +
        '</MenuSection></Menu>';

    var menuXmlActionBlankLetter = '<Menu Id=\"ISV.DynamicMenu\">' +
        '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
        '<Controls Id=\"ISV.Dynamic.Controls\">' +
        '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\" />' +
        '<Button Id=\"ISV.Dynamic.FaxCoverSheet\" Command=\"ISV.SearchCommand\" Sequence=\"86\" LabelText=\"Fax Cover Sheet\" Alt=\"Fax Cover Sheet\" />' +

        '</Controls>' +
        '</MenuSection></Menu>';

    if (action != null) {
        var actionvalue = Xrm.Page.getAttribute("va_action").getValue();
        var controlId = actionvalue;

        switch (controlId) {
            //ECC
            case 953850000:
                CommandProperties.PopulationXML = menuXmlECCAction;
                break;
                //0820
            case 1:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //0820a
            case 953850001:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //0820d
            case 953850002:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //0820f
            case 953850003:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //VAI
            case 2:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //0820 & VAI
            case 953850005:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //Letter
            case 953850006:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //Non Emergency Email
            case 953850007:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
                //Email Forms
            case 953850004:
                CommandProperties.PopulationXML = menuXmlActionBlankLetter;
                break;
            default:
                CommandProperties.PopulationXML = menuXmlActionBlankLetter;

        }

    }

}


function getReportID(reportName) {
    var len = reports.list.length;

    for (var i = 0; i < len; i++) {
        if (reports.list[i].name === reportName) {
            return reports.list[i].reportId;
        }
    }

    return '';
}

//Call Report based on seletion

function Search(CommandProperties) {
    var controlId = CommandProperties.SourceControlId;
    var GUIDvalue = Xrm.Page.data.entity.getId();
    var serverUrl = Xrm.Page.context.getServerUrl();
    var lettersCreated = Xrm.Page.getAttribute("va_letterscreated");
    var pcr = Getinfo();
    var now = new Date();
    var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());

    switch (controlId) {


        case 'ISV.Dynamic.0820':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('27-0820 - Report of General Information') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Letter 0820 was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter 0820 was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.0820a':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('27-0820a - Report of First Notice of Death') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Letter 0820a was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter 0820a was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.0820d':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('27-0820d - Report of Non-Receipt of Payment') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Letter 0820d was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter 0820d was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.0820f':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('27-0820f - Report of Month of Death') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Letter 0820f was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter 0820f was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.119':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('119 - Report of Contact') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Letter 0820 was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter 0820 was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.AmountLetterGross':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Amount Letter Gross') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Amount Letter Gross was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Letter Amount Letter Gross was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.BlankLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Blank Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Blank Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Blank Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.CommissaryLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Commissary Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Commissary Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Commissary Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.CommissaryLetterSurvivingSpouse':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Commissary Letter Surviving Spouse') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Commissary Letter Surviving Spouse was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Commissary Letter Surviving Spouse was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.CommissaryLetterwithFutureExam':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Commissary Letter with Future Exam') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Commissary Letter with Future Exam was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Commissary Letter with Future Exam was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.DeathBenefitsLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Death Benefits Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Death Benefits Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Death Benefits Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.DisabilityBreakdownLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Disability Breakdown Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Disability Breakdown Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Disability Breakdown Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.FaxCoverSheet':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Fax Cover Sheet') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Fax Cover Sheet was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Fax Cover Sheet was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.IncomeAmountLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Income Amount Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Income Amount Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Income Amount Letter was created by ' + pcr + ' on ' + today);
            }
            break;
            //case 'ISV.Dynamic.InformalClaimLetter':
            //    window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Informal Claims Letter - AB10') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            //    if (lettersCreated.getValue() == null) {
            //        lettersCreated.setValue('Informal Claim Letter was created by ' + pcr + ' on ' + today);
            //    }
            //    else {
            //        lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Informal Claim Letter was created by ' + pcr + ' on ' + today);
            //    }
            //    break;
        case 'ISV.Dynamic.MODMemorandum':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('MOD Memorandum') + "%7d&records=" + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('MOD Memorandum was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'MOD Memorandum was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.MultiplePaymentLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Multiple Payment Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Multiple Payment Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Multiple Payment Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.NoBenefitsLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('No Benefits Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('No Benefits Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'No Benefits Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.NOKLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('NOK Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('NOK Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'NOK Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.PreferenceLetters':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Preference Letters') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Preference Letters was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Preference Letters was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.PropertyTaxDMVRegistration':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Property Tax DMV Registration - MT') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Property Tax DMV Registration was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Property Tax DMV Registration was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.PropertyTaxExemption':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Property Tax Exemption - CA - LA') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Property Tax Exemption was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Property Tax Exemption was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.RealEstate':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Real Estate - License Fee Tax Exemption') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Real Estate was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Real Estate was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.ServiceVerification':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Service Verification') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Service Verification was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Service Verification was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.SummaryofBenefitsSurvivingSpouseLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Summary of Benefits Surviving Spouse Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Summary of Benefits Surviving Spouse Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Summary of Benefits Surviving Spouse was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.SummaryofBenefitsVeteransLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Summary of Benefits Veterans Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Summary of Benefits Veterans Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Summary of Benefits Veterans Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.VeteranPercentageLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Veteran Percentage Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Veteran Percentage Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Veteran Percentage Letter was created by ' + pcr + ' on ' + today);
            }
            break;
        case 'ISV.Dynamic.VeteranPTLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('Veteran PT Letter') + "%7d&p:ServiceRequestGUID=" + GUIDvalue);
            if (lettersCreated.getValue() == null) {
                lettersCreated.setValue('Veteran PT Letter was created by ' + pcr + ' on ' + today);
            }
            else {
                lettersCreated.setValue(lettersCreated.getValue() + ';\n' + 'Veteran PT Letter was created by ' + pcr + ' on ' + today);
            }
            break;

        default:
            alert('Button Unknown');
    }
}