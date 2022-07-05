
namespace UDO.LOB.Core
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Json;
	using System.Text;
	using System.Text.Json;
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json.Linq;

	public static class JsonHelper
	{
        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
		{
			var instance = Activator.CreateInstance<T>();
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				var serializer = new DataContractJsonSerializer(instance.GetType());
				return (T)serializer.ReadObject(ms);
			}
		}

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object Deserialize(string json, Type type)
		{
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				var serializer = new DataContractJsonSerializer(type);
				return serializer.ReadObject(ms);
			}
		}

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var serializer = new DataContractJsonSerializer(obj.GetType());
				serializer.WriteObject(ms, obj);
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string Serialize(object obj, Type type)
		{
			using (var ms = new MemoryStream())
			{
				var serializer = new DataContractJsonSerializer(type);
				serializer.WriteObject(ms, obj);
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}

        /// <summary>
        /// Parses the json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static JObject ParseJson(string json)
		{
			JObject jObject = JObject.Parse(json);
			return jObject;
		}

        /// <summary>
        /// Gets the json value.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string GetJsonValue(string json, string path)
		{
			string value = "";
			JObject jObject = JObject.Parse(json);
			value = (string)jObject.SelectToken(path);
			return value;
		}

        /// <summary>
        /// Gets the json value list.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static IList<string> GetJsonValueList(string token, string json)
		{
			JObject jObject = JObject.Parse(json);
			IList<string> list = jObject.SelectToken(token).Select(s => (string)s).ToList();
			return list;
		}

        /// <summary>
        /// Gets a new high performance <see cref="Utf8JsonReader"/>
        /// from a not null <see cref="Stream"/>.
        /// </summary>
        /// <param name="notNullStream">The not null stream.</param>
        /// <returns>The new <see cref="Utf8JsonReader"/> instance.</returns>
        public static Utf8JsonReader GetReader(Stream notNullStream)
        {
            if (!(notNullStream is MemoryStream memoryStream))
            {
                using (memoryStream = new MemoryStream())
                {
                    notNullStream.CopyTo(memoryStream);
                    return new Utf8JsonReader(memoryStream.ToArray(), new JsonReaderOptions
                    {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip
                    });
                }
            }

            return new Utf8JsonReader(memoryStream.ToArray(), new JsonReaderOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });
        }

        /// <summary>
        /// Gets a new high performance <see cref="Utf8JsonReader"/>
        /// from a not null or empty JSON string.
        /// </summary>
        /// <param name="notNullOrEmptyJsonString">The not null or empty JSON string.</param>
        /// <returns>The new <see cref="Utf8JsonReader"/> instance.</returns>
        public static Utf8JsonReader GetReader(string notNullOrEmptyJsonString) =>
            new Utf8JsonReader(Encoding.UTF8.GetBytes(notNullOrEmptyJsonString),
                new JsonReaderOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                });

        /// <summary>
        /// Writes a compact JSON string using the new high performance <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <param name="writerMethod">The writer method.</param>
        /// <returns>The JSON string</returns>
        public static string WriteCompactString(Action<Utf8JsonWriter> writerMethod)
        {
            var memoryStream = new MemoryStream();
            using (var utf8JsonWriter = new Utf8JsonWriter(memoryStream, new JsonWriterOptions
            {
                Indented = false
            }))
            {
                writerMethod(utf8JsonWriter);
                utf8JsonWriter.Flush();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Writes a human readable JSON string using the new high performance <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <param name="writerMethod">The writer method.</param>
        /// <returns>The JSON string</returns>
        public static string WriteHumanReadableString(Action<Utf8JsonWriter> writerMethod)
        {
            var memoryStream = new MemoryStream();
            using (var utf8JsonWriter = new Utf8JsonWriter(memoryStream, new JsonWriterOptions
            {
                Indented = true
            }))
            {
                writerMethod(utf8JsonWriter);
                utf8JsonWriter.Flush();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Writes a JSON string using the new high performance <see cref="Utf8JsonWriter"/>.
        /// </summary>
        /// <param name="writerMethod">The writer method.</param>
        /// <returns>The JSON string</returns>
        public static string WriteString(Action<Utf8JsonWriter> writerMethod)
        {
            var memoryStream = new MemoryStream();
            using (var utf8JsonWriter = new Utf8JsonWriter(memoryStream, new JsonWriterOptions
            {
                Indented = false
            }))
            {
                writerMethod(utf8JsonWriter);
                utf8JsonWriter.Flush();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
