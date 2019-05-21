using ApplicationPlanner.Transcripts.Core.Models;
using CC.Cache;
using CC.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface IStudentRepository : IRepository
    {
        Task<StudentGeneralInfoModel> StudentGeneralInfoGetByPortfolioIdAsync(int portfolioId);
    }

    public class StudentRepository : Repository, IStudentRepository
    {
        private readonly ISql _sql;

        public StudentRepository(ISql sql, ICache cache)
          : base(sql, cache)
        {
            _sql = sql;
        }

        public async Task<StudentGeneralInfoModel> StudentGeneralInfoGetByPortfolioIdAsync(int portfolioId)
        {
            var data = await _sql.QueryAsync<StudentGeneralInfoModel>("[ApplicationPlanner].[StudentGeneralInfoGetByPortfolioId]",
                   new { portfolioId },
                   commandType: CommandType.StoredProcedure);

            return data.FirstOrDefault();
        }
    }
}
