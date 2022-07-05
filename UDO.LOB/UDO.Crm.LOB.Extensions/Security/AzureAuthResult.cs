using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.LOB.Core
{
    public class AzureAuthResult
    {
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets the token expires on.
        /// </summary>
        /// <value>
        /// The token expires on.
        /// </value>
        public DateTimeOffset ExpiresOn { get; set; }


        /// <summary>
        /// Gets or sets the authentication token type.
        /// </summary>
        /// <value>
        /// The authentication token type.
        /// </value>
        public string TokenType { get; set; }
        
    }
}

