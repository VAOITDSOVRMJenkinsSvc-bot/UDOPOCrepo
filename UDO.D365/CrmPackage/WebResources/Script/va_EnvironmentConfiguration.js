//var environmentConfigurations = (function (win) {
//    'use strict';
//    function initalize() {      
//        var globalCon = Xrm.Utility.getGlobalContext();
//        var version = globalCon.getVersion();
//        var webApi = new CrmCommonJS.WebApi(version);
//         return new Promise(function (resolve, reject) { 
//             webApi.RetrieveMultiple("va_systemsettings", ['va_name', 'va_description'], "$filter=va_type eq 953850002")
//                 .then(function (data) {
//                     if (data && data.length > 0) {
//                         var env = setEnvironmentVariables(data);
//                         win._currentEnv = env;
//                         return resolve(env);

//                     } else {
//                         reject(new Error('CRM did not return any enviroment settings!'));
//                     }

//                 }).catch(function (err) {
//                     var error = 'Failed to retrieve the System Settings.\r\nError: ' + err.statusText;
//                     if (err.status === 400 && err.responseText)
//                         error += '\r\n' + $.parseJSON(err.responseText).error.message.value;
//                     reject(new Error(error));
//                     //alert(error);
//                 });
//         });
       
//    }

//    async function get() {
//        if (!win._currentEnv) {
//            //var currEnv =
//                return await initalize();
//            //return currEnv;
//        }
       
//    }

//    function setEnvironmentVariables(data) {
//        var i, env = {};
//        for (i = 0; i < data.length; i++) {
//            if (data[i].va_Description === null || data[i].va_description === 'undefined' || data[i].va_description === '' || data[i].va_description.toLowerCase() === 'null') {
//                env[data[i].va_name] = null;
//            } else if (data[i].va_description.toLowerCase() === 'true' || data[i].va_description.toLowerCase() === 'false') {
//                env[data[i].va_name] = (data[i].va_description.toLowerCase() === 'true');
//            } else {
//                env[data[i].va_name] = data[i].va_description;
//            }
//        }
//        return env;
//    }

//    return {
//        initalize: initalize,
//        get: get
//    };
//})(window);

var environmentConfigurations = (function (win) {
    'use strict';

    function initalize() {
        var globalCon = Xrm.Utility.getGlobalContext();
        var version = globalCon.getVersion();
        var webApi = new CrmCommonJS.WebApi(version);
         return new Promise(function (resolve, reject) { 
             webApi.RetrieveMultiple("va_systemsettings", ['va_name', 'va_description'], "$filter=va_type eq 953850002")
                 .then(function (data) {
                     if (data && data.length > 0) {
                         var env = setEnvironmentVariables(data);
                         win._currentEnv = env;
                         return resolve(env);

                     } else {
                         reject(new Error('CRM did not return any enviroment settings!'));
                     }

                 }).catch(function (err) {
                     var error = 'Failed to retrieve the System Settings.\r\nError: ' + err.statusText;
                     if (err.status === 400 && err.responseText)
                         error += '\r\n' + $.parseJSON(err.responseText).error.message.value;
                     reject(new Error(error));
                     //alert(error);
                 });
         });
    }

    function get() {
        var getFunction = environmentConfigurations.initalize();
        return getFunction;
    }

    function setEnvironmentVariables(data) {
        console.log(data);
        var i, env = {};
        for (i = 0; i < data.length; i++) {
            if (data[i].va_Description === null || data[i].va_description === 'undefined' || data[i].va_description === '' || data[i].va_description.toLowerCase() === 'null') {
                env[data[i].va_name] = null;
            } else if (data[i].va_description.toLowerCase() === 'true' || data[i].va_description.toLowerCase() === 'false') {
                env[data[i].va_name] = (data[i].va_description.toLowerCase() === 'true');
            } else {
                env[data[i].va_name] = data[i].va_description;
            }
        }
        return env;
    }

    return {
        initalize: initalize,
        get: get
    };
}(window));