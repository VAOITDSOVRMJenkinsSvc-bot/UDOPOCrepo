function displayIconTooltip_DaysRemaining(rowData, userLCID)
{
	var str = JSON.parse(rowData);
    var coldata = str.udo_daysremainingininterval_Value;	
    coldata = parseInt(coldata);
    var imgName = "";
    var tooltip = "";
    if (coldata != null)
    {
		if (coldata < 0)
		{			
            imgName = "udo_redcircle-32.svg";
            tooltip = "Past Due";
		}
        else if (coldata >= 0 && coldata <=30)
        {            
            imgName = "udo_yellowcircle-32.svg";
            tooltip = "Due in " + coldata.toString() + " days";
        }
        else 
        {
            imgName = "udo_greencircle-32.svg";
            tooltip = "Due in " + coldata.toString() + " days";
        }
    }

    var resultarray = [imgName, tooltip];
    return resultarray;
}
