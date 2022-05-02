using Aethel.Redis.Abstractions;
using Atlantis.IntegrationEvents.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal class RedisConsumerClientFactory : IConsumerClientFactory
    {
        private readonly ILogger<RedisConsumerClient> _logger;
        private readonly IRedisStreamManager _redis;
        private readonly IOptions<RedisOptions> _redisOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisOptions"></param>
        /// <param name="manager"></param>
        /// <param name="logger"></param>
        public RedisConsumerClientFactory(IOptions<RedisOptions> redisOptions,
            IRedisStreamManager redis, ILogger<RedisConsumerClient> logger)
        {
            _logger = logger;
            _redis = redis;
            _redisOptions = redisOptions;
        }

        public IConsumerClient Create(string groupId)
        {
            return new RedisConsumerClient(groupId,_redis, _redisOptions, _logger);
        }
    }
}
