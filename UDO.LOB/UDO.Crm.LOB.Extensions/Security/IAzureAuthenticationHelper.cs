
namespace UDO.LOB.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAzureAuthenticationHelper
    {
        /// <summary>
        /// Obtains the authentication token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="apiResourceId">The API resource identifier.</param>
        /// <param name="tenant">The tenant.</param>
        /// <param name="aadInstance">The AAD instance.</param>
        /// <returns>Azure Authentication result</returns>
        Task<AzureAuthResult> ObtainAuthTokenAsync(string clientId, string clientSecret, string apiResourceId, string tenant, string aadInstance);
        Task<string> ObtainSecretFromKeyVault(string KVSecretURL);
    }
}
