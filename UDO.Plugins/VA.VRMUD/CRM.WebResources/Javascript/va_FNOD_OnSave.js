function OnSave(context) {
   if (ValidateZipcode() == false) {
            context.getEventArgs().preventDefault(); // RU12 Changed all event.returnValue
            return false;
   }
}

function ValidateZipcode() {
   var va_spousezipcode = Xrm.Page.getAttribute('va_spousezipcode').getValue();
   if (va_spousezipcode != null && va_spousezipcode.match(/[a-zA-Z]/)) {
      alert('Spouse/Last Known Address Zip Code field contains invalid alphabetical characters');
      return false;
   }

   return true;
}

