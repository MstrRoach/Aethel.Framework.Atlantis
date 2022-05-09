using Aethel.Redis.Abstractions;
using Atlantis.Configuration;
using Atlantis.IntegrationEvents.Internal;
using Atlantis.IntegrationEvents.Persistence;
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
    internal class RedisTransport : ITransport
    {
        private readonly ILogger<RedisTransport> _logger;
        private readonly RedisOptions _options;
        private readonly IntegrationOptions _integrationOptions;
        private readonly IRedisStreamManager _redis;

        /// <summary>
        /// Constructor de la capa de transporte
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public RedisTransport(IRedisStreamManager redis, IOptions<RedisOptions> options,
            ILogger<RedisTransport> logger, IOptions<IntegrationOptions> integrationOptions)
        {
            _redis = redis;
            _options = options.Value;
            _integrationOptions = integrationOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Publica un mensae utilizando redis
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<OperateResult> SendAsync(MediumMessage message)
        {
            try
            {
                var content = message.AsStreamEntries();
                await _redis.PublishAsync(_integrationOptions.DefaultStream, content);
                _logger.LogDebug($"Redis message [{_integrationOptions.DefaultStream}] has been published.");
                return OperateResult.Success;
            }
            catch (Exception ex)
            {
                return OperateResult.Failed(ex);
            }
        }
    }
}
