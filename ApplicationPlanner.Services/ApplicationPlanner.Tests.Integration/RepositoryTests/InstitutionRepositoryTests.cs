using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class InstitutionRepositoryTests : TestBase
    {
        private readonly InstitutionRepository _institutionRepository;

        public InstitutionRepositoryTests()
        {
            _institutionRepository = new InstitutionRepository(_sql, _cache);
        }

        [TestMethod]
        [TestCategory("Institution Repository")]
        public async Task SavedSchoolsByPortfolioIdAsync_should_return_all_saved_schools()
        {
            // Arrange:
            var expectedValue = new TranscriptInstitutionModel
            {
                InunId = "5000",
                Name = "Abilene Christian University",
                ImageName = "school-406.jpg",
                City = "Abilene",
                StateProvCode = "TX",
                StateProvName = "Texas"
            };
            // 1. Delete All Saved Schools
            await _sql.ExecuteAsync("DELETE FROM Student.EducationSchool WHERE PortfolioId = @portfolioId", new { portfolioId = integrationTestPortfolioId });
            // 2. Add a saved School
            await _sql.ExecuteAsync("INSERT INTO Student.EducationSchool (EducationSchoolId, PortfolioId) VALUES (406, @portfolioId)", new { portfolioId = integrationTestPortfolioId });

            // Act:
            var result = (await _institutionRepository.SavedSchoolsByPortfolioIdAsync(integrationTestPortfolioId, 2)).ToList();

            // Assert:
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(expectedValue.InunId == result[0].InunId);
            Assert.IsTrue(expectedValue.Name == result[0].Name);
            Assert.IsTrue(expectedValue.ImageName == result[0].ImageName);
            Assert.IsTrue(expectedValue.City == result[0].City);
            Assert.IsTrue(expectedValue.StateProvCode == result[0].StateProvCode);
            Assert.IsTrue(expectedValue.StateProvName == result[0].StateProvName);
        }

        [TestMethod]
        [TestCategory("Institution Repository")]
        public async Task GetDefaultInstitutionByEducatorIdAsync_should_return_institution()
        {
            // Arrange:
            // 1. Reset Default Institution
            await _sql.ExecuteAsync("UPDATE School.EducatorProfile SET DefaultInstitutionId = NULL WHERE EducatorId = @educatorId", new { educatorId = integrationTestEducatorId });

            // Act:
            var result = await _institutionRepository.GetDefaultInstitutionByEducatorIdAsync(integrationTestEducatorId);

            // Assert:
            Assert.IsTrue(result == null);
        }
    }
}
