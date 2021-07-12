using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetcoreRedis.Data
{
    public class RedisConnection
    {
        private ConnectionMultiplexer _conexao;
        public RedisConnection(IConfiguration configuration)
        {
            _conexao = ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("RedisServer"));
        }

        public string GetValueFromKey(string key)
        {
            var dbRedis = _conexao.GetDatabase();
            return dbRedis.StringGet(key);
        }

        public bool SetValue(string key, string value)
        {
            try
            {
                var dbRedis = _conexao.GetDatabase();
                dbRedis.StringSet(key, value, expiry: null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
