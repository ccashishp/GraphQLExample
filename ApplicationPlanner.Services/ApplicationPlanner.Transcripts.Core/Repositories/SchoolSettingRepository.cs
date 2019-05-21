using ApplicationPlanner.Transcripts.Core.Models;
using CC.Cache;
using CC.Data;
using System.Data;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface ISchoolSettingRepository : IRepository
    {
        /// <summary>
        /// Get by schoolId
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        Task<SchoolSettingModel> GetBySchoolIdAsync(int schoolId);
    }
    public class SchoolSettingRepository : Repository, ISchoolSettingRepository
    {
        private readonly ISql _sql;
        private readonly ICache _cache;

        public SchoolSettingRepository(ISql sql, ICache cache): base(sql, cache)
        {
            _sql = sql;
            _cache = cache;
        }

        public Task<SchoolSettingModel> GetBySchoolIdAsync(int schoolId)
        {
            var cachekey = _cache.CreateKey("TranscriptsSchoolSettingGetBySchoolId", schoolId);
            return _sql.CacheQueryAsyncSingle<SchoolSettingModel>(
                cachekey,
                "[ApplicationPlanner].[SchoolSettingGetBySchoolId]",
                new { schoolId },
                commandType: CommandType.StoredProcedure);
        }
    }
}
