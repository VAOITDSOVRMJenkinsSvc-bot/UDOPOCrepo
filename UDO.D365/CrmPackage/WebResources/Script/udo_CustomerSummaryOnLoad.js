function UDO_OnLoad(executionContext) {
       debugger;

window.top.Microsoft.Omnichannel.getConversationId().then(
    function success(ConversationId) {
        // Get Custom Context
        Xrm.WebApi.online.retrieveMultipleRecords("msdyn_ocliveworkitemcontextitem", "?$select=msdyn_name,_msdyn_ocliveworkitemid_value,msdyn_value&$filter=(_msdyn_ocliveworkitemid_value eq '" + ConversationId + "' and msdyn_name eq 'relationshiptoveteran')").then(
            function success(data) {
                debugger;
                if ((data !== null) && (data.entities !== null) && (data.entities.length > 0)) {
                    debugger;
                    if (data.entities[0].msdyn_value!== "1 -Veteran") {
                        alert("Chatter is not the veteran. Please ask for caller information to fill out the interaction correctly.");
                    } else {
                        USDPop(ConversationId);
                    }                    
                } else {
                    alert("Cannot retrieve conversation. Please create interaction manually in USD.");
                }
            },
            function (error) {
                alert("Error retrieving custom context. Please create interaction manually in USD.");
            });        
    },
    function error() {
        console.log("An error has occurred retrieving conversation id");
    });
}

function USDPop(ConversationId) {
        // USD screen pop
        Xrm.WebApi.online.retrieveMultipleRecords("msdyn_ocliveworkitem", "?$select=_msdyn_customer_value&$filter=msdyn_ocliveworkitemid eq '" + ConversationId + "'").then(
            function success(data) {
                debugger;
                if ((data !== null) && (data.entities !== null) && (data.entities.length > 0)) {
                    console.log("*********************" + data.entities[0]._msdyn_customer_value);

                    if (data.entities[0]._msdyn_customer_value !== null) {
                        Xrm.WebApi.online.retrieveMultipleRecords("contact", "?$select=va_dobtext,udo_ssn,lastname&$filter=contactid eq '" + data.entities[0]._msdyn_customer_value + "'").then(
                            function success(data) {
                                if (data.entities.length > 0) {
                                    var xmlHttp = new XMLHttpRequest();
                                    if ((data.entities[0].udo_ssn == null) || (data.entities[0].va_dobtext == null)) {
                                        alert("Customer record does not have DOB and SSN. Please create interaction manually in USD.");
                                    } else {
                                        var newDOB = data.entities[0].va_dobtext;
                                        if (newDOB.indexOf("/") > -1) { 
                                            var newDOB = newDOB.replace(/(\d\d)\/(\d\d)\/(\d{4})/, "$3$1$2");
                                        }

                                        var Url = "http://localhost:5000/?ani=1234567890&ssn=" + data.entities[0].udo_ssn + "&dob=" + newDOB + "&conversationid=" + ConversationId;
                                        xmlHttp.open("GET", Url, true);
                                        xmlHttp.send(null);
                                        //var response = xmlHttp.responseText;
                                    }
                                } else {
                                    alert("Customer not found. Please create interaction manually in USD.");
                                }
                            },
                            function (error) {
                                alert("Customer not found. Please create interaction manually in USD.");
                            });    
                    }
                } 
            },
            function (error) {
                alert("Error retrieving conversation record. Please create interaction manually in USD.");
            });
}
