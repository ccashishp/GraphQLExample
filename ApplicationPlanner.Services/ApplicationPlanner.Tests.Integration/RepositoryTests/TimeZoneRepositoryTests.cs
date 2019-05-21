using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Integration.RepositoryTests
{
    [TestClass]
    public class TimeZoneRepositoryTests : TestBase
    {
        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneRepositoryTests()
        {
            _timeZoneRepository = new TimeZoneRepository(_sql, _cache);
        }

        [TestMethod]
        [TestCategory("TimeZone Repository")]
        public async Task GeTimeZoneDetailByIdAsync_should_return_expected_value()
        {
            // Arrange:
            // Clear the cache to make sure we are testing the sproc
            var timezoneId = 9;
            var cachekey = _cache.CreateKey("TranscriptsGeTimeZoneDetailById", timezoneId);
            await _cache.DeleteAsync(cachekey);
            var expetedValue = new TimeZoneDetailModel
            {
                TimeZoneId = timezoneId,
                TimeZoneKey = "TIMEZONE_ATS",
                SQLKey = "Atlantic Standard Time",
                FriendlyName = "(GMT-04:00) Atlantic Time (Canada)"
            };

            // Act:
            var result = await _timeZoneRepository.GeTimeZoneDetailByIdAsync(timezoneId);

            // Assert:
            Assert.AreEqual(expetedValue.TimeZoneId, result.TimeZoneId);
            Assert.AreEqual(expetedValue.TimeZoneKey, result.TimeZoneKey);
            Assert.AreEqual(expetedValue.SQLKey, result.SQLKey);
            Assert.AreEqual(expetedValue.FriendlyName, result.FriendlyName);
        }

        [TestMethod]
        [TestCategory("TimeZone Repository")]
        public async Task GeTimeZoneIdByPortfolioIdAsync_should_return_expected_value()
        {
            // Arrange:

            // Act:
            var result = await _timeZoneRepository.GeTimeZoneIdByPortfolioIdAsync(integrationTestPortfolioId);

            // Assert:
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [TestCategory("TimeZone Repository")]
        public async Task GeTimeZoneIdBySchoolIdAsync_should_return_expected_value()
        {
            // Arrange:

            // Act:
            var result = await _timeZoneRepository.GeTimeZoneIdBySchoolIdAsync(integrationTestSchoolId);

            // Assert:
            Assert.AreEqual(3, result);
        }
    }
}
