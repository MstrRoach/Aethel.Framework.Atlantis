using Aethel.Extensions.Application.Serialization;
using Atlantis.IntegrationEvents.Persistence;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal static class RedisMessage
    {
        private const string Content = "content";

        /// <summary>
        /// Convierte un mensaje en entraddas de redis
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static NameValueEntry[] AsStreamEntries(this MediumMessage message)
        {
            return new[]
            {
                new NameValueEntry(Content, ToJson(message))
            };
        }

        /// <summary>
        /// Convierte una entrada de redis en un medium message
        /// </summary>
        /// <param name="streamEntry"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static MediumMessage Create(StreamEntry streamEntry, string? groupId = null)
        {
            var bodyRaw = streamEntry[Content];

            var message = !bodyRaw.IsNullOrEmpty ? JsonConvert.DeserializeObject<MediumMessage>(bodyRaw) : null;

            return message;
        }

        /// <summary>
        /// Convierte un objeto a json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static RedisValue ToJson(object? obj)
        {
            if (obj == null)
            {
                return RedisValue.Null;
            }

            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }); 
            ///JsonSerializer.Serialize(obj, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
    }
}
