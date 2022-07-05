namespace SecretsEncryptor
{
    using System;

    public class InternalUdoKVHelperConfig
    {

        public string ClientId { get; private set; } = string.Empty;
        public string ClientSecret { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
        public string Url { get; private set; } = string.Empty;

        public void SetClientId(string clientId)
        {
            ClientId = Guid.TryParse(clientId, out Guid clientIdGuid) ?
                clientIdGuid.ToString("D") :
                string.Empty;

            SetKeyInternal();
        }

        public void SetClientSecret(string clientSecret) => ClientSecret = clientSecret;

        public void SetUrl(string url)
        {
            var actualUrl = url;

            actualUrl = string.IsNullOrWhiteSpace(actualUrl) ? string.Empty
                : !actualUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? "https://" + actualUrl
                : Uri.TryCreate(actualUrl, UriKind.Absolute, out var uri) ? "https://" + uri.DnsSafeHost + "/"
                : string.Empty;

            Url = actualUrl;
            SetKeyInternal();
        }

        private void SetKeyInternal() => Key = Url
            .Replace("https://", string.Empty, StringComparison.Ordinal)
            .TrimEnd('/') +
            ClientId.ToUpperInvariant();
    }
}
