var environmentConfigurations = (function (win) {
    'use strict';

    function initalize() {
        CrmRestKit2011.ByQuery('va_systemsettings', ['va_name', 'va_Description'], 'va_Type/Value eq 953850002', false)
        .done(function (data) {
            if (data && data.d && data.d.results && data.d.results.length > 0) {
                var env = setEnvironmentVariables(data.d.results);
                win._currentEnv = env;
            } else {
                throw new Error('CRM did not return any enviroment settings!');
            }
            
        }).fail(function (err) {
            var error = 'Failed to retrieve the System Settings.\r\nError: ' + err.statusText;
            if (err.status === 400 && err.responseText)
                error += '\r\n' +  $.parseJSON(err.responseText).error.message.value;

            alert(error);
            throw new Error(error);
        });
    }
    
    function get() {
        if (!win._currentEnv) {
            initalize();
        }

        return win._currentEnv;
    }

    function setEnvironmentVariables(data) {
        var i, env = {};
        for (i = 0; i < data.length; i++) {
            if (data[i].va_Description === null || data[i].va_Description === 'undefined' || data[i].va_Description === '' || data[i].va_Description.toLowerCase() === 'null') {
                env[data[i].va_name] = null;
            } else if (data[i].va_Description.toLowerCase() === 'true' || data[i].va_Description.toLowerCase() === 'false') {
                env[data[i].va_name] = (data[i].va_Description.toLowerCase() === 'true');
            } else {
                env[data[i].va_name] = data[i].va_Description;
            }
        }
        return env;
    }

    return {
        initalize: initalize,
        get: get
    };
}(window));