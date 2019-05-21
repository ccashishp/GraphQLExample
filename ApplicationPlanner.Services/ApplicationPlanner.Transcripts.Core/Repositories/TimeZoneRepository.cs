using ApplicationPlanner.Transcripts.Core.Models;
using CC.Cache;
using CC.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface ITimeZoneRepository : IRepository
    {
        Task<TimeZoneDetailModel> GeTimeZoneDetailByIdAsync(int timeZoneId);
        Task<int> GeTimeZoneIdByPortfolioIdAsync(int portfolioId);
        Task<int> GeTimeZoneIdBySchoolIdAsync(int schoolId);
    }

    public class TimeZoneRepository : Repository, ITimeZoneRepository
    {
        private readonly ISql _sql;
        private readonly ICache _cache;
        public TimeZoneRepository(ISql sql, ICache cache) : base(sql, cache)
        {
            _sql = sql;
            _cache = cache;
        }

        public Task<TimeZoneDetailModel> GeTimeZoneDetailByIdAsync(int timeZoneId)
        {
            var cachekey = _cache.CreateKey("TranscriptsGeTimeZoneDetailById", timeZoneId);
            return _sql.CacheQueryAsyncSingle<TimeZoneDetailModel>(
                cachekey,
                "dbo.TimeZoneDetailById",
                new
                {
                    TimeZoneId = timeZoneId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GeTimeZoneIdByPortfolioIdAsync(int portfolioId)
        {
            var result = await _sql.QueryAsync<int>(
                "dbo.TimeZoneIdGetByPortfolioId",
                new
                {
                    portfolioId
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<int> GeTimeZoneIdBySchoolIdAsync(int schoolId)
        {
            var result = await _sql.QueryAsync<int>(
                "dbo.TimeZoneIdGetBySchoolId",
                new
                {
                    schoolId
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
    }
}
