var VASS = VASS || {};
VASS.CallAttempt = VASS.CallAttempt || {};

VASS.CallAttempt.MakeReadOnly = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var userSettings = Xrm.Utility.getGlobalContext().userSettings;

    var isOnlyAgent = false;
    var isAdmin = false;

    var currentUserRoles = userSettings.roles;
    if (currentUserRoles != null || currentUserRoles != undefined) {
        for (var j = 0; j <= currentUserRoles.getLength(); j++) {
            var role = currentUserRoles.get(j);
            if (role != undefined) {
                if (role.name == "VASS Administrator") {
                    isAdmin = true;
                    break;
                }
            }
        }
        if (!isAdmin) {
            for (var i = 0; i <= currentUserRoles.getLength(); i++) {
                var role = currentUserRoles.get(i);
                if (role != undefined) {
                    if (role.name == "VASS PCR") {
                        isOnlyAgent = true;
                        break;
                    }
                }
            }

            var createdBy = formContext.getAttribute("createdby").getValue()[0].id.replace('{', '').replace('}', '');

            var userId = userSettings.userId.replace('{', '').replace('}', '');
            var createdOn = formContext.getAttribute('createdon').getValue()

            var dateNow = new Date();
            var dateCreatedTime = new Date(createdOn);

            const milliseconds = Math.abs(dateNow - dateCreatedTime);
            const hours = milliseconds / 36e5;

            var controls = formContext.ui.controls.get();
            //set form read only by default for an agent
            for (var i in controls) {
                var control = controls[i];
                control.setDisabled(true);
            }


            if (createdBy == userId && isOnlyAgent) {
                //set form editable if hours lapsed is less than 24 hrs.
                if (hours < 24) {                
                    for (var i in controls) {
                        var control = controls[i];
                        control.setDisabled(false);
                    }
                }
            }
        }
    }    
    
}



