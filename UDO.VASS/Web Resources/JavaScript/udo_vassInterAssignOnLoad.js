var VASS = VASS || {};
VASS.InteractionAssignment = VASS.InteractionAssignment || {};

VASS.InteractionAssignment.setFieldsReadOnly = function(executionContext)
{
    var formContext = executionContext.getFormContext();
   formContext.ui.controls.get("udo_city").setDisabled(true);
   formContext.ui.controls.get("udo_state").setDisabled(true);
   formContext.ui.controls.get("udo_phone1").setDisabled(true);
   formContext.ui.controls.get("udo_phone2").setDisabled(true);

}