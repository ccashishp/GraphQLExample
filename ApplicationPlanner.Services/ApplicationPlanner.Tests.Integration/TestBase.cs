using CC.Cache;
using CC.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ApplicationPlanner.Tests.Integration
{
    public class TestBase
    {
        private IConfiguration _configuration;
        protected ICache _cache;
        protected ISql _sql;
        // All the Integration Tests accounts are set in this school: Xello-eTranscript-Test-School-2-250055
        protected const int integrationTestSchoolId = 250055;
        // student - bentest5-ap.api.integration.tests/xeppelin - PorfolioId=15935657
        protected const int integrationTestPortfolioId = 15935657;
        // educator - ap.api.integration.tests@xello.world
        protected const int integrationTestEducatorId = 8919;

        protected TestBase()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json")
                .Build();

            var redisConnection = new RedisConnection(_configuration["CacheConfig:RedisServer"]).Multiplexer.GetDatabase();
            var serializer = new JsonNetSerializer();
            _cache = new Redis(redisConnection, serializer, Convert.ToInt32(_configuration["CacheConfig:CacheExpiryTime"]));
            _sql = new Sql(_cache, new SqlConnection(_configuration["DataConfig:XelloDbServer"]));
        }

        public class SqlConnection : ISqlConnectionString
        {
            private string _connString;
            public SqlConnection(String connString)
            {
                _connString = connString;
            }
            public string GetConnectionString() => _connString;
        }
    }
}
