namespace UDO.VASS.Plugins.Models
{
    public class AzureAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string expires_on { get; set; }
        public string resource { get; set; }
    }
}
