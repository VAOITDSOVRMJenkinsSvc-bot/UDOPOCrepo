var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
//Va.Udo.Crm.Scripts.ScriptControl = Va.Udo.Crm.Scripts.ScriptControl || {};


Va.Udo.Crm.Scripts.ScriptControl = {
    baseUrl : null,
    scriptUrl : null,
    
        
    onLoadTest : function () {
        debugger;                                      
         //Va.Udo.Crm.Scripts.ScriptControl.OpenCallScript("{507FA48D-B4C2-E411-80CD-00155D5564CC}"); 
         Va.Udo.Crm.Scripts.ScriptControl.ShowScript("Scripts_CP");

    },

    
    
    //depercaited
    Initialize  : function () {
        this.baseUrl = Xrm.Page.context.getClientUrl()+'/';//.replace(Xrm.Page.context.getOrgUniqueName(),"");
        this.BuildScriptUrl("Scripts_CallerIDScript");
    },
    
    //these two methods will show any script whose url is stored in the va_systemsettings table
    ShowScript  : function (scriptName,idStr) {        
        this.baseUrl = Xrm.Page.context.getClientUrl() +'/';//.replace(Xrm.Page.context.getOrgUniqueName(),"");
        this.BuildScriptUrl(scriptName);
    },    
    BuildScriptUrl : function (scriptName,idStr,extraData) {
        if (scriptName == null || scriptName == "") return; //nothing to do
        var columns = ['va_Description','va_name', 'va_Type'];
        var filter =  "va_Description eq '" + scriptName +"'";
		if (extraData && extraData !=null) {
			extraData = encodeURIComponent("id="+idStr + "&" + extraData);
        } else {
			extraData = encodeURIComponent("id="+idStr);
        }
        var me = this; //save this pointer through REST call
        CrmRestKit2011.ByQuery("va_systemsettings",columns,filter,false)
        .done( function ( data ) {
                    var selected = false;
                    
                    if (data && data.d.results.length > 0) {
                        var scriptName = data.d.results[0].va_name;
                        if (scriptName == null || scriptName == "" || scriptName == "null") {
                            scriptName="udo_noscript.html";
                        }
                        if (data.d.results[0].va_Type.Value == 953850002) {
                            me.scriptUrl = me.baseUrl + scriptName;
                        } else {
                            me.scriptUrl = scriptName;
                        }
                        if (!idStr || idStr.length == 0) {
                        window.open(me.scriptUrl);
                        } else {
                            window.open(me.scriptUrl + "?data=" + extraData); //+ encodeURIComponent("id="+idStr));
                        }
                    }
                })
        .fail( function (err) {} );
    },
    
    
    
    OpenCallScript : function (idStr,idProofId,extraData) {
        this.baseUrl = Xrm.Page.context.getClientUrl()+'/';//.replace(Xrm.Page.context.getOrgUniqueName(),"");
        if (idStr == null || idStr == "") return; //nothing to do
        var filter = "udo_requestsubtypeId eq guid'" + idStr +"'";
        var columns = ['udo_ScriptFileName'];
        
        if (extraData && extraData !=null) {
			extraData = encodeURIComponent("id="+idProofId + "&" + extraData);
        } else {
			extraData = encodeURIComponent("id="+idProofId);
        }
       
        var me = this; //save this pointer through REST call
        CrmRestKit2011.ByQuery('udo_requestsubtype', columns, filter, false)       
        .fail(
            function (err) {
            })
        .done(
        function (data) {
            var scriptFile = null;
            var scriptUrl = "";
            if (data && data.d.results.length > 0) {
                var scriptName = data.d.results[0].udo_ScriptFileName;
                        if (scriptName == null || scriptName == "" || scriptName == "null") {
                            scriptName="udo_noscript.html";
                        }
                //console.log(scriptName);
                if (scriptName.toLowerCase().substring(0,7) == "http://" || scriptName.toLowerCase().substring(0,8) == "https://" ) 
                {                
                    scriptUrl = scriptName;
                } else {
                    scriptUrl = me.baseUrl + 'WebResources/' + scriptName;
                }
                if (!idProofId || idProofId.length == 0) {
                    var NewWindow = window.open(scriptUrl);
                    NewWindow.focus();
                } else {
                    var NewWindow = window.open(scriptUrl + "?data=" + extraData); //encodeURIComponent("id="+idProofId));
                    NewWindow.focus();
                }
            }
        });
    }
    
    
}