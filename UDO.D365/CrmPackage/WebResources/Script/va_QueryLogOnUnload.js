﻿function OnUnload(context) {
    alert('Query Log records cannot be created manually.');
    context.getEventArgs().preventDefault(); // RU12 Changed event.returnValue
    return false;
}