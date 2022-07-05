using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace UDO.VASS.Plugins.Helpers
{
    public static class JsonHelper
    {
        public static string Serialize(object obj, Type type)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(type);
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public static T Deserialize<T>(string json)
        {
            var instance = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(instance.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
