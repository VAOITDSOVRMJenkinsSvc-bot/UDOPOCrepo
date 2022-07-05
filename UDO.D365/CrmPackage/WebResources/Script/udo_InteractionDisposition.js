"use strict";

function SaveRecord_USD(context){
    if (context.ui.getFormType() === 1) {
         context.getAttribute("udo_name").setValue("Interaction Disposition");
    }
    context.data.entity.save();
  }
