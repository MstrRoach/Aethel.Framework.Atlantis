using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal class RedisLogger : TextWriter
    {
        private readonly ILogger logger;

        public RedisLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public override Encoding Encoding => Encoding.UTF8;


        public override void WriteLine(string value)
        {
            logger.LogInformation(value);
        }
    }
}
