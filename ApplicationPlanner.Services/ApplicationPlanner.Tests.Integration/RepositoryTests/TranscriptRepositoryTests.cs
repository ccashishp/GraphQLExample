using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using CC.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class TranscriptRepositoryTests : TestBase
    {
        private readonly TranscriptRepository _transcriptRepository;
        private readonly QARepository _qaRepository;

        public TranscriptRepositoryTests()
        {
            _transcriptRepository = new TranscriptRepository(_sql, _cache);
            _qaRepository = new QARepository(_sql);
        }

        [TestMethod]
        [TestCategory("Transcript Repository")]
        public async Task GetTranscriptImportedBySchoolIdAsync_should_return_expected_value()
        {
            // Arrange:
            var studentNumber = "AP-API-IT";
            var studentName = "IntegrationTests, ApplicationPlanner.API";
            var dob = "2000/01/01";
            var dobParts = dob.Split('/');
            var expectedValue = new TranscriptImportedViewModel
            {
                Id = integrationTestPortfolioId,
                UserAccountId = 293945,
                AvatarFileName = null,
                SchoolCountryType = CC.Common.Enum.CountryType.US,
                StudentName = studentName,
                DateOfBirth = new DateTime(int.Parse(dobParts[0]), int.Parse(dobParts[1]), int.Parse(dobParts[2])),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = studentNumber,
                TranscriptId = 1000000, // We know in Xello TranscriptId starts at 1000000
                ImportedDate = DateTime.UtcNow
            };
            // Reset
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE PortfolioId = @portfolioId", new { portfolioId = integrationTestPortfolioId });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.TranscriptLog WHERE StudentNumber = @studentNumber", new { studentNumber });
            // Import a transcript and automatch it to the integration test student
            await _qaRepository.ImportTranscriptAsync(integrationTestPortfolioId, integrationTestSchoolId, studentNumber, studentName, dob, "integration@tests.com");

            // Act:
            var result = (await _transcriptRepository.GetTranscriptImportedBySchoolIdAsync(integrationTestSchoolId)).ToList();

            // Assert:
            Assert.IsTrue(result.IsNotNullOrEmpty());
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(expectedValue.Id, result[0].Id);
            Assert.AreEqual(expectedValue.UserAccountId, result[0].UserAccountId);
            Assert.AreEqual(expectedValue.AvatarFileName, result[0].AvatarFileName);
            Assert.AreEqual(expectedValue.SchoolCountryType, result[0].SchoolCountryType);
            Assert.AreEqual(expectedValue.StudentName, result[0].StudentName);
            Assert.AreEqual(expectedValue.DateOfBirth, result[0].DateOfBirth);
            Assert.AreEqual(expectedValue.GradeId, result[0].GradeId);
            Assert.AreEqual(expectedValue.GradeKey, result[0].GradeKey);
            Assert.AreEqual(expectedValue.StudentId, result[0].StudentId);
            Assert.IsTrue(result[0].TranscriptId >= expectedValue.TranscriptId); // the id for the new transcript should be >= than the min allowed
            Assert.IsTrue(result[0].ImportedDate != null);
        }

        [TestMethod]
        [TestCategory("Transcript Repository")]
        public async Task GetTranscriptUnmatchedBySchoolIdAsync_should_return_expected_value()
        {
            // Arrange:
            var receivedDateUtc = DateTime.UtcNow;
            var studentNumber = "AP-API-IT-2";
            var studentName = "IntegrationTests-2, ApplicationPlanner.API";
            var dob = "2000/01/01";
            var expectedValue = new TranscriptBaseModel
            {
                TranscriptId = 1000000,
                StudentName = studentName,
                StudentNumber = studentNumber,
                DateOfBirth = dob,
                ReceivedDateUtc = receivedDateUtc
            };
            // Reset
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE StudentNumber = @studentNumber", new { studentNumber });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.TranscriptLog WHERE StudentNumber = @studentNumber", new { studentNumber });
            // Import a transcript and do not automatch it to the integration test student
            await _qaRepository.ImportTranscriptAsync(0, integrationTestSchoolId, studentNumber, studentName, dob, "integration-2@tests.com");

            // Act:
            var result = (await _transcriptRepository.GetTranscriptUnmatchedBySchoolIdAsync(integrationTestSchoolId)).ToList();

            // Assert:
            Assert.IsTrue(result.IsNotNullOrEmpty());
            Assert.IsTrue(result.Count >= 1);
            var justAddedTranscript = result.SingleOrDefault(t => t.StudentNumber == studentNumber);
            Assert.IsTrue(justAddedTranscript.TranscriptId >= expectedValue.TranscriptId); // the id for the new transcript should be >= than the min allowed
            Assert.AreEqual(justAddedTranscript.StudentName, result[0].StudentName);
            Assert.AreEqual(justAddedTranscript.StudentNumber, result[0].StudentNumber);
            Assert.AreEqual(justAddedTranscript.DateOfBirth, result[0].DateOfBirth);
            Assert.IsTrue(justAddedTranscript.ReceivedDateUtc != null);
        }

        [TestMethod]
        [TestCategory("Transcript Repository")]
        public async Task TranscriptCRUD_should_work_as_expected()
        {
            // Arrange:
            var receivedDateUtc = DateTime.UtcNow;
            var studentNumber = "AP-API-IT-2";
            var studentName = "IntegrationTests-2, ApplicationPlanner.API";
            var dob = "2000/01/01";
            var transcript = new TranscriptModel
            {
                TranscriptId = 1000000,
                SchoolId = integrationTestSchoolId,
                StudentNumber = studentNumber,
                StudentName = studentName,
                DateOfBirth = dob,
                EmailAddress = "integration-2@tests.com",
                ReceivedDateUtc = receivedDateUtc,
                PortfolioId = 0, // unmatched this should be 0
                LinkApprovedDateUTC = null, // unmatched this should be null
                EducatorId = 0,
                IsAutoLink = false, // unmatched this should be false
                IsAvailable = true,
                IsArchived = false
            };
            // Reset
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE StudentNumber = @studentNumber", new { studentNumber });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.TranscriptLog WHERE StudentNumber = @studentNumber", new { studentNumber });
            // Import a transcript and do not automatch it to the integration test student
            await _qaRepository.ImportTranscriptAsync(0, integrationTestSchoolId, studentNumber, studentName, dob, "integration-2@tests.com");
            var justAddedTranscript = (await _transcriptRepository.GetTranscriptUnmatchedBySchoolIdAsync(integrationTestSchoolId)).SingleOrDefault(t => t.StudentNumber == studentNumber);

            // Act:
            // Delete and check if deleted + Get
            await _transcriptRepository.DeleteByIdAsync(justAddedTranscript.TranscriptId);
            var result = await _transcriptRepository.GetByIdAsync(justAddedTranscript.TranscriptId);
            Assert.IsTrue(result.IsAvailable == false);

            // Undo Delete and check if added back + Get
            await _transcriptRepository.DeleteUndoByIdAsync(justAddedTranscript.TranscriptId);
            result = await _transcriptRepository.GetByIdAsync(justAddedTranscript.TranscriptId);
            Assert.IsTrue(result.IsAvailable == true);
            Assert.AreEqual(justAddedTranscript.TranscriptId, result.TranscriptId);
            Assert.AreEqual(transcript.SchoolId, result.SchoolId);
            Assert.AreEqual(transcript.StudentNumber, result.StudentNumber);
            Assert.AreEqual(transcript.StudentName, result.StudentName);
            Assert.AreEqual(transcript.DateOfBirth, result.DateOfBirth);
            Assert.AreEqual(transcript.EmailAddress, result.EmailAddress);
            Assert.IsTrue(transcript.ReceivedDateUtc != null);
            Assert.AreEqual(transcript.PortfolioId, result.PortfolioId);
            Assert.AreEqual(transcript.LinkApprovedDateUTC, result.LinkApprovedDateUTC);
            Assert.AreEqual(transcript.EducatorId, result.EducatorId);
            Assert.AreEqual(transcript.IsAutoLink, result.IsAutoLink);
            Assert.AreEqual(transcript.IsArchived, result.IsArchived);
        }

        [TestMethod]
        [TestCategory("Transcript Repository")]
        public async Task GetStudentTranscriptBySchoolIdAsync_should_return_expected_value()
        {
            // Arrange:
            var studentNumber = "AP-API-IT";
            var studentName = "IntegrationTests, ApplicationPlanner.API";
            var dob = "2000/01/01";
            var dobParts = dob.Split('/');
            var expectedValue = new StudentTranscriptViewModel
            {
                Id = integrationTestPortfolioId,
                UserAccountId = 293945,
                AvatarFileName = null,
                SchoolCountryType = CC.Common.Enum.CountryType.US,
                StudentName = studentName,
                DateOfBirth = new DateTime(int.Parse(dobParts[0]), int.Parse(dobParts[1]), int.Parse(dobParts[2])),
                GradeId = 11,
                GradeKey = "GRADE_11",
                StudentId = studentNumber,
                TranscriptId = 1000000, // We know in Xello TranscriptId starts at 1000000
                ReceivedDateUtc = null
            };
            // Reset
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE PortfolioId = @portfolioId", new { portfolioId = integrationTestPortfolioId });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.TranscriptLog WHERE StudentNumber = @studentNumber", new { studentNumber });
            // Import a transcript and automatch it to the integration test student
            await _qaRepository.ImportTranscriptAsync(integrationTestPortfolioId, integrationTestSchoolId, studentNumber, studentName, dob, "integration@tests.com");

            // Act:
            var result = (await _transcriptRepository.GetStudentTranscriptBySchoolIdAsync(integrationTestSchoolId)).ToList();

            // Assert:
            Assert.IsTrue(result.IsNotNullOrEmpty());
            Assert.IsTrue(result.Count >= 1);
            var justAddedTranscript = result.SingleOrDefault(t => t.StudentId == studentNumber);
            Assert.IsTrue(justAddedTranscript != null);
            Assert.AreEqual(expectedValue.Id, justAddedTranscript.Id);
            Assert.AreEqual(expectedValue.UserAccountId, justAddedTranscript.UserAccountId);
            Assert.AreEqual(expectedValue.AvatarFileName, justAddedTranscript.AvatarFileName);
            Assert.AreEqual(expectedValue.SchoolCountryType, justAddedTranscript.SchoolCountryType);
            Assert.AreEqual(expectedValue.StudentName, justAddedTranscript.StudentName);
            Assert.AreEqual(expectedValue.DateOfBirth, justAddedTranscript.DateOfBirth);
            Assert.AreEqual(expectedValue.GradeId, justAddedTranscript.GradeId);
            Assert.AreEqual(expectedValue.GradeKey, justAddedTranscript.GradeKey);
            Assert.AreEqual(expectedValue.StudentId, justAddedTranscript.StudentId);
            Assert.IsTrue(expectedValue.TranscriptId <= justAddedTranscript.TranscriptId); // the id for the new transcript should be >= than the min allowed
            Assert.IsTrue(justAddedTranscript.ReceivedDateUtc != null);
        }

        [TestMethod]
        [TestCategory("Transcript Repository")]
        public async Task GetStudentTranscriptByPortfolioIdAsync_should_return_expected_value()
        {
            // Arrange:
            var studentNumber = "AP-API-IT";
            var studentName = "IntegrationTests, ApplicationPlanner.API";
            var dob = "2000/01/01";
            var dobParts = dob.Split('/');
            var expectedValue = new StudentTranscriptViewModel
            {
                Id = integrationTestPortfolioId,
                UserAccountId = 293945,
                AvatarFileName = null,
                SchoolCountryType = CC.Common.Enum.CountryType.US,
                StudentName = studentName,
                StudentId = studentNumber,
                ReceivedDateUtc = null
            };
            // Reset
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE PortfolioId = @portfolioId", new { portfolioId = integrationTestPortfolioId });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.TranscriptLog WHERE StudentNumber = @studentNumber", new { studentNumber });
            // Import a transcript and automatch it to the integration test student
            await _qaRepository.ImportTranscriptAsync(integrationTestPortfolioId, integrationTestSchoolId, studentNumber, studentName, dob, "integration@tests.com");

            // Act:
            var result = await _transcriptRepository.GetStudentTranscriptByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.IsTrue(result != null);
            Assert.AreEqual(expectedValue.Id, result.Id);
            Assert.AreEqual(expectedValue.UserAccountId, result.UserAccountId);
            Assert.AreEqual(expectedValue.AvatarFileName, result.AvatarFileName);
            Assert.AreEqual(expectedValue.SchoolCountryType, result.SchoolCountryType);
            Assert.AreEqual(expectedValue.StudentName, result.StudentName);
            Assert.AreEqual(expectedValue.StudentId, result.StudentId);
            Assert.IsTrue(result.ReceivedDateUtc != null);
        }

        [TestMethod]
        [TestCategory("Transcript Repository")]
        public async Task MatchTranscriptToStudentAsync_should_work_as_expected()
        {
            // Arrange:
            var receivedDateUtc = DateTime.UtcNow;
            var studentNumber = "AP-API-IT-2";
            var studentName = "IntegrationTests-2, ApplicationPlanner.API";
            var dob = "2000/01/01";
            var expectedValue = new TranscriptBaseModel
            {
                TranscriptId = 1000000,
                StudentName = studentName,
                StudentNumber = studentNumber,
                DateOfBirth = dob,
                ReceivedDateUtc = receivedDateUtc
            };
            // Reset
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE PortfolioId = @portfolioId", new { portfolioId = integrationTestPortfolioId });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.Transcript WHERE StudentNumber = @studentNumber", new { studentNumber });
            await _sql.ExecuteAsync("DELETE FROM ApplicationPlanner.TranscriptLog WHERE StudentNumber = @studentNumber", new { studentNumber });
            // Import a transcript and do not automatch it to the integration test student
            await _qaRepository.ImportTranscriptAsync(0, integrationTestSchoolId, studentNumber, studentName, dob, "integration-2@tests.com");
            var justAddedTranscript = (await _transcriptRepository.GetTranscriptUnmatchedBySchoolIdAsync(integrationTestSchoolId)).SingleOrDefault(t => t.StudentNumber == studentNumber);

            // Act:
            await _transcriptRepository.MatchTranscriptToStudentAsync(justAddedTranscript.TranscriptId, integrationTestPortfolioId, integrationTestEducatorId);
            var result = await _transcriptRepository.GetByIdAsync(justAddedTranscript.TranscriptId);

            // Assert:
            Assert.AreEqual(integrationTestPortfolioId, result.PortfolioId);
            Assert.IsFalse(result.IsAutoLink);
            Assert.IsTrue(result.LinkApprovedDateUTC != null);
        }
    }
}
