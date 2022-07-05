﻿using System;
using Microsoft.Xrm.Sdk;

namespace Va.Udo.Usd.CustomControls.Shared
{
    public static class ConvertTo
    {

        public static EntityReference ToEntityReference(this string dataParameter)
        {
            var retEr = new EntityReference();

            if (dataParameter.StartsWith("EntityReference"))
            {
                dataParameter = dataParameter.Replace("EntityReference", "");
                dataParameter = dataParameter.Replace("(", "");
                dataParameter = dataParameter.Replace(")", "");

                if (dataParameter.Contains(","))
                {
                    var splitDataParameter = dataParameter.Split(',');
                    retEr.LogicalName = splitDataParameter[0];
                    retEr.Id = new Guid(splitDataParameter[1]);
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }

            return retEr;
        }
    }
}

