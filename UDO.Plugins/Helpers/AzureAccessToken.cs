﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSPlugins
{
    /// <summary>
    /// Token for Azure Authentication
    /// </summary>
    public class AzureAccessToken
    { 
        public string access_token { get; set; } 
        public string token_type { get; set; } 
        public string expires_in { get; set; } 
        public string expires_on { get; set; } 
        public string resource { get; set; }
    }
}