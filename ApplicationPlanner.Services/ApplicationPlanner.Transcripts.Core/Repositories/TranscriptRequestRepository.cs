using ApplicationPlanner.Transcripts.Core.Helpers;
using ApplicationPlanner.Transcripts.Core.Models;
using CC.Cache;
using CC.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Core.Repositories
{
    public interface ITranscriptRequestRepository : IRepository
    {
        Task<int> CreateTranscriptRequestAsync(
            int portfolioId,
            string inunId,
            int transcriptRequestTypeId,
            int? receivingInstitutionCode,
            string receivingInstitutionName,
            string receivingInstitutionCity,
            string receivingInstitutionStateCode,
            int createdById);
        Task<IEnumerable<TranscriptRequestModel>> GetTranscriptRequestByPortfolioIdAsync(int portfolioId);
        Task<TranscriptRequestTimelineDto> GetTranscriptRequestProgressByPortfolioIdAsync(int portfolioId);
        Task<TranscriptRequestTimelineDtoV2> GetTranscriptRequestProgressByPortfolioIdAsyncV2(int portfolioId);
        Task<IEnumerable<TranscriptRequestStudentViewModel>> GetTranscriptRequestBySchoolIdAsync(int schoolId);
        Task<int> DeleteByIdAsync(int id, int deletedById);
        Task<int> DeleteUndoByIdAsync(int id);
        Task AppendHistoryAsync(int transcriptRequestId, TranscriptRequestStatus status, int modifiedById);
        Task AppendHistoryAsync(List<int> transcriptRequestIdList, TranscriptRequestStatus status, int modifiedById);
        Task<IEnumerable<TranscriptRequestProgressViewModel>> GetTranscriptRequestProgressBySchoolIdAsync(int schoolId);
        Task<SendTranscriptViewModel> GetSendTranscriptByTranscriptRequestIdAsync(int transcriptRequestId);
        Task<IEnumerable<SendTranscriptViewModel>> GetSendTranscriptByTranscriptRequestIdListAsync(List<int> transcriptRequestIdList);
        Task UpdateTypeAsync(int transcriptRequestId, int transcriptRequestTypeId);
        Task AddTranscriptRequestSubmittedAsync(int transcriptRequestId, string receivingInstitutionEmail = null, DateTime? dateSentByMailUtc = null);
        Task<DateTime> GetRequestedDateByIdAsync(int transcriptRequestId);
    }

    public class TranscriptRequestRepository : Repository, ITranscriptRequestRepository
    {
        private readonly ISql _sql;

        public TranscriptRequestRepository(ISql sql, ICache cache)
          : base(sql, cache)
        {
            _sql = sql;
        }

        public async Task<int> CreateTranscriptRequestAsync(
            int portfolioId,
            string inunId,
            int transcriptRequestTypeId,
            int? receivingInstitutionCode,
            string receivingInstitutionName,
            string receivingInstitutionCity,
            string receivingInstitutionStateCode,
            int createdById)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestCreate]",
                param: new
                {
                    portfolioId,
                    inunId,
                    transcriptRequestTypeId,
                    receivingInstitutionCode,
                    receivingInstitutionName,
                    receivingInstitutionCity,
                    receivingInstitutionStateCode,
                    createdById
                },
                commandType: CommandType.StoredProcedure);
            
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<TranscriptRequestStudentViewModel>> GetTranscriptRequestBySchoolIdAsync(int schoolId)
        {
            return await _sql.QueryAsync<TranscriptRequestStudentViewModel>(
                sql: "[ApplicationPlanner].[TranscriptRequestGetBySchoolId]",
                param: new
                {
                    schoolId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TranscriptRequestModel>> GetTranscriptRequestByPortfolioIdAsync(int portfolioId)
        {
            return await _sql.QueryAsync<TranscriptRequestModel,TranscriptRequestHistoryModel, TranscriptRequestModel>(
                sql: "[ApplicationPlanner].[TranscriptRequestGetByPortfolioId]",
                map: (request, latestHistory) => {
                    request.LatestHistory = latestHistory;
                    return request;
                },
                splitOn: "TranscriptStatusId",
                param: new
                {
                    portfolioId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<TranscriptRequestTimelineDtoV2> GetTranscriptRequestProgressByPortfolioIdAsyncV2(int portfolioId)
        {
            var data = await _sql.MultiQueryAsync(
                sql: "[ApplicationPlanner].[TranscriptRequestTimelineGetByPortfolioIdV2]",
                types: new List<Type>
                {
                    typeof(TranscriptRequestInstitutionModel),
                    typeof(TranscriptRequestHistoryModelV2)
                },
                param: new
                {
                    portfolioId
                },
                commandType: CommandType.StoredProcedure);

            return new TranscriptRequestTimelineDtoV2
            {
                TranscriptRequestInstitutionList = await data.ReadAsync<TranscriptInstitutionModel>(),
                TranscriptRequestHistoryList = await data.ReadAsync<TranscriptRequestHistoryModelV2>()
            };
        }

        public async Task<TranscriptRequestTimelineDto> GetTranscriptRequestProgressByPortfolioIdAsync(int portfolioId)
        {
            var data = await _sql.MultiQueryAsync(
                sql: "[ApplicationPlanner].[TranscriptRequestTimelineGetByPortfolioId]",
                types: new List<Type>
                {
                    typeof(TranscriptRequestInstitutionModel),
                    typeof(TranscriptRequestHistoryModel)
                },
                param: new
                {
                    portfolioId
                },
                commandType: CommandType.StoredProcedure);

            return new TranscriptRequestTimelineDto
            {
                TranscriptRequestInstitutionList = await data.ReadAsync<TranscriptRequestInstitutionModel>(),
                TranscriptRequestHistoryList = await data.ReadAsync<TranscriptRequestHistoryModel>()
            };
        }

        public async Task<int> DeleteByIdAsync(int id, int deletedById)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestDelete]",
                param: new
                {
                    TranscriptRequestId = id,
                    deletedById
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<int> DeleteUndoByIdAsync(int id)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestDeleteUndo]",
                param: new
                {
                    TranscriptRequestId = id
                },
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task AppendHistoryAsync(int transcriptRequestId, TranscriptRequestStatus status, int modifiedById)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestHistoryAppend]",
                param: new
                {
                    transcriptRequestId,
                    statusId = (int)status,
                    modifiedById
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AppendHistoryAsync(List<int> transcriptRequestIdList, TranscriptRequestStatus status, int modifiedById)
        {
            var dt = DataConverterHelper.GetDataTableFromListOfInteger(transcriptRequestIdList, "ID");
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestHistoryAppendByIdList]",
                param: new
                {
                    TranscriptRequestIdList = dt,
                    statusId = (int)status,
                    modifiedById
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TranscriptRequestProgressViewModel>> GetTranscriptRequestProgressBySchoolIdAsync(int schoolId)
        {
            return await _sql.QueryAsync<TranscriptRequestProgressViewModel>(
                sql: "[ApplicationPlanner].[TranscriptRequestProgressGetBySchoolId]",
                param: new
                {
                    schoolId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<SendTranscriptViewModel> GetSendTranscriptByTranscriptRequestIdAsync(int transcriptRequestId)
        {
            var result = await _sql.QueryAsync<SendTranscriptViewModel>(
                sql: "[ApplicationPlanner].[SendTranscriptByTranscriptRequestId]",
                param: new
                {
                    transcriptRequestId
                },
                commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<SendTranscriptViewModel>> GetSendTranscriptByTranscriptRequestIdListAsync(List<int> transcriptRequestIdList)
        {
            var dt = DataConverterHelper.GetDataTableFromListOfInteger(transcriptRequestIdList, "ID");
            return await _sql.QueryAsync<SendTranscriptViewModel>(
                sql: "[ApplicationPlanner].[SendTranscriptByTranscriptRequestIdList]",
                param: new
                {
                    TranscriptRequestIdList = dt
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateTypeAsync(int transcriptRequestId, int transcriptRequestTypeId)
        {
            var result = await _sql.QueryAsync<int>(
                sql: "[ApplicationPlanner].[TranscriptRequestUpdateType]",
                param: new
                {
                    transcriptRequestId,
                    transcriptRequestTypeId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddTranscriptRequestSubmittedAsync(int transcriptRequestId, string receivingInstitutionEmail = null, DateTime? dateSentByMailUtc = null)
        {
            await _sql.ExecuteAsync(
                sql: "[ApplicationPlanner].[TranscriptRequestSubmittedInsert]",
                param: new
                {
                    transcriptRequestId,
                    receivingInstitutionEmail,
                    dateSentByMail = dateSentByMailUtc
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<DateTime> GetRequestedDateByIdAsync(int transcriptRequestId)
        {
            var result = await _sql.QueryAsync<DateTime>(
                sql: "[ApplicationPlanner].[TranscriptRequestGetRequestedDateById]",
                param: new
                {
                    transcriptRequestId
                },
                commandType: CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }
    }
}
