namespace SecretsEncryptor
{
    using System.IO;
    using System.Text;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.UserSecrets;
    using Newtonsoft.Json;

    public class UdoSecretsProvider
    {
        private readonly string _dotNetCoreJsonFilePath;
        private readonly ConcurrentDictionary<string, string> _secrets = new ConcurrentDictionary<string, string>();
        private const string UserSecretsId = "1e01257a-3171-4d0a-a517-045dd867cf1f";

        public UdoSecretsProvider()
        {
            _dotNetCoreJsonFilePath = PathHelper.GetSecretsPathFromSecretsId(UserSecretsId);

            var folderPath = Path.GetDirectoryName(_dotNetCoreJsonFilePath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            Reload();
        }

        public string GetValueAsNotNullString(string key)
        {
            if (_secrets.ContainsKey(key))
            {
                var value = _secrets[key];

                if (string.IsNullOrWhiteSpace(value))
                {
                    return string.Empty;
                }

                return value;
            }

            return string.Empty;
        }

        public void Save()
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();

                var sortedList = new SortedList<string, string>();

                foreach (var keyValuePair in _secrets)
                {
                    sortedList.Add(keyValuePair.Key, keyValuePair.Value);
                }

                foreach (var keyValuePair in sortedList)
                {
                    writer.WritePropertyName(keyValuePair.Key);
                    writer.WriteValue(keyValuePair.Value);
                }

                writer.WriteEndObject();
            }

            File.WriteAllText(_dotNetCoreJsonFilePath, sb.ToString());

            Reload();
        }

        public void SetValue(string key, string value)
        {
            _secrets[key] = value;
        }

        private void Reload()
        {
            var keyValuePairs = new ConfigurationBuilder()
                .AddUserSecrets(UserSecretsId)
                .Build()
                .AsEnumerable();

            _secrets.Clear();

            foreach (var keyValuePair in keyValuePairs)
            {
                _secrets[keyValuePair.Key] = keyValuePair.Value;
            }
        }
    }
}
