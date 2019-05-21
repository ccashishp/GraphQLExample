using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Models;
using CC.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class TranscriptRequestRepositoryTests : TestBase
    {
        private readonly TranscriptRequestRepository _transcriptRequestRepository;
        private readonly QARepository _qaRepository;
        private readonly RequestTranscriptRequestModel _ncaa;

        public TranscriptRequestRepositoryTests()
        {
            _transcriptRequestRepository = new TranscriptRequestRepository(_sql, _cache);
            _qaRepository = new QARepository(_sql);
            _ncaa = new RequestTranscriptRequestModel
            {
                InunId = "45784",
                TranscriptRequestTypeId = TranscriptRequestType.InNetwork,
                ReceivingInstitutionCode = 45784,
                ReceivingInstitutionName = "NCAA - National Collegiate Athletic Association",
                ReceivingInstitutionCity = "Indianapolis",
                ReceivingInstitutionStateCode = "IN"
            };
        }

        [TestMethod]
        [TestCategory("TranscriptRequest Repository")]
        public async Task CreateTranscriptRequestAsync_Success()
        {
            // Arrange:
            int integrationTestUserAccountId = 293945;
            // Reset
            await _qaRepository.DeleteTranscriptRequestAsync(integrationTestPortfolioId);

            // Act:
            var result = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);
            // Assert:
            Assert.IsTrue(result >= 1000000); // We know in Xello TranscriptId starts at 1000000
        }

        [TestMethod]
        [TestCategory("TranscriptRequest Repository")]
        public async Task CreateTranscriptRequestAsync_should_create_new_request_and_return_its_id()
        {
            // Arrange:
            int integrationTestUserAccountId = 293945;
            int integrationTestEducatorUserAccountId = 293944;
            /* Reset:
             * 1. clean all requests for the student
             * 2. Add a new request and send it
             * */
            await _qaRepository.DeleteTranscriptRequestAsync(integrationTestPortfolioId);
            var tr1_id = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);
            await _transcriptRequestRepository.AppendHistoryAsync(tr1_id, TranscriptRequestStatus.Submitted, integrationTestEducatorUserAccountId);

            // Act:
            var result = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);
            // Assert:
            Assert.IsTrue(result > tr1_id);
        }

        [TestMethod]
        [TestCategory("TranscriptRequest Repository")]
        public async Task CreateTranscriptRequestAsync_should_not_create_new_request_and_return_id_of_requested_request()
        {
            // Arrange:
            int integrationTestUserAccountId = 293945;
            /* Reset:
             * 1. clean all requests for the student
             * 2. Add a new request
             * */
            await _qaRepository.DeleteTranscriptRequestAsync(integrationTestPortfolioId);
            var tr1_id = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);

            // Act:
            var result = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);
            // Assert:
            Assert.IsTrue(result == tr1_id);
        }

        [TestMethod]
        [TestCategory("TranscriptRequest Repository")]
        public async Task GetTranscriptRequestByPortfolioIdAsync_should_return_expected_value()
        {
            // Arrange:
            int integrationTestUserAccountId = 293945;
            // Reset
            await _qaRepository.DeleteTranscriptRequestAsync(integrationTestPortfolioId);
            var id = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);
            var expectedValue = new TranscriptRequestModel
            {
                Id = id,
                InunId = _ncaa.InunId,
                ReceivingInstitutionCode = _ncaa.ReceivingInstitutionCode ?? 0,
                LatestHistory = new TranscriptRequestHistoryModel{
                    TranscriptRequestId = id,
                    ModifiedById = integrationTestUserAccountId,
                    TranscriptStatusId = 1,
                    StatusDateUTC = null,
                    TranscriptRequestTypeId = 1

                }
            };

            // Act:
            var result = (await _transcriptRequestRepository.GetTranscriptRequestByPortfolioIdAsync(integrationTestPortfolioId)).ToList();
            
            // Assert:
            Assert.IsTrue(result.IsNotNullOrEmpty());
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(expectedValue.Id, result[0].Id);
            Assert.AreEqual(expectedValue.InunId, result[0].InunId);
            Assert.AreEqual(expectedValue.ReceivingInstitutionCode, result[0].ReceivingInstitutionCode);
        }

        [TestMethod]
        [TestCategory("TranscriptRequest Repository")]
        public async Task GetTranscriptRequestProgressByPortfolioIdAsync_should_return_expected_value()
        {
            // Arrange:
            int integrationTestUserAccountId = 293945;
            // Reset
            await _qaRepository.DeleteTranscriptRequestAsync(integrationTestPortfolioId);
            var id = await _transcriptRequestRepository.CreateTranscriptRequestAsync(integrationTestPortfolioId, _ncaa.InunId, (int)_ncaa.TranscriptRequestTypeId, _ncaa.ReceivingInstitutionCode, _ncaa.ReceivingInstitutionName, _ncaa.ReceivingInstitutionCity, _ncaa.ReceivingInstitutionStateCode, integrationTestUserAccountId);
            var expectedValue = new TranscriptRequestTimelineDto
            {
                TranscriptRequestInstitutionList = new List<TranscriptRequestInstitutionModel>
                {
                    new TranscriptRequestInstitutionModel
                    {
                        TranscriptRequestId = id,
                        InunId = _ncaa.InunId,
                        Name = _ncaa.ReceivingInstitutionName,
                        ImageName = "ap_ncaa.svg",
                        City = _ncaa.ReceivingInstitutionCity,
                        StateProvCode = _ncaa.ReceivingInstitutionStateCode,
                        StateProvName = null
                    }
                },
                TranscriptRequestHistoryList = new List<TranscriptRequestHistoryModel>
                {
                    new TranscriptRequestHistoryModel
                    {
                        TranscriptRequestId = id,
                        ModifiedById = integrationTestUserAccountId,
                        TranscriptStatusId = 1,
                        StatusDateUTC = null,
                        TranscriptRequestTypeId = 1
                    }
                }
            };

            // Act:
            var result = await _transcriptRequestRepository.GetTranscriptRequestProgressByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsTrue(result != null);
            var expectedList1 = expectedValue.TranscriptRequestInstitutionList.ToList()[0];
            var expectedList2 = expectedValue.TranscriptRequestHistoryList.ToList()[0];
            var resultList1 = result.TranscriptRequestInstitutionList.ToList()[0];
            var resultList2 = result.TranscriptRequestHistoryList.ToList()[0];

            Assert.AreEqual(expectedList1.TranscriptRequestId, resultList1.TranscriptRequestId);
            Assert.AreEqual(expectedList1.InunId, resultList1.InunId);
            Assert.AreEqual(expectedList1.Name, resultList1.Name);
            Assert.AreEqual(expectedList1.ImageName, resultList1.ImageName);
            Assert.AreEqual(expectedList1.City, resultList1.City);
            Assert.AreEqual(expectedList1.StateProvCode, resultList1.StateProvCode);
            Assert.AreEqual(expectedList1.StateProvName, resultList1.StateProvName);

            Assert.AreEqual(expectedList2.TranscriptRequestId, resultList2.TranscriptRequestId);
            Assert.AreEqual(expectedList2.ModifiedById, resultList2.ModifiedById);
            Assert.AreEqual(expectedList2.TranscriptStatusId, resultList2.TranscriptStatusId);
            Assert.IsTrue(resultList2.StatusDateUTC != null);
            Assert.AreEqual(expectedList2.TranscriptRequestTypeId, resultList2.TranscriptRequestTypeId);
        }
    }
}
