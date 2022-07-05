Sdk.Entity.prototype.getValue = function(n, an) { /* modified by Carlton Colter 12/20/2016 - added an (allow null) to prevent throwing an error if value is not set */
    if (typeof n !== "string") throw new Error("Sdk.Entity.getValue logicalName parameter is required and must be a string.");
	if (an!==undefined && typeof an !== "boolean") throw new Error("Sdk.Entity.getValue AllowNull parameter is optional and must be a boolean.");
	if (an===undefined) an = false;
	var t = null;
    try {
		if (an && !this.hasAttribute(n)) 
		{
			t = null;
		} else {
			t = this.getAttributes(n).getValue();
		}
    } catch (i) {
        throw i;
    }
    return t;
};
Sdk.Entity.prototype.hasAttribute = function(n) { /* added 12-20-2016 by Carlton Colter */
    if (typeof n != "string") throw new Error("Sdk.Entity.getValue logicalName parameter is required and must be a string.");
    var t = null;
    try {
        t = (this.getAttributes().getNames().indexOf(n) !== -1);
    } catch (i) {
        throw i;
    }
    return t
};