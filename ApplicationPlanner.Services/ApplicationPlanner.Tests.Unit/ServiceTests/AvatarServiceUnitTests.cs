using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using CC.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class AvatarServiceUnitTests
    {
        private IDictionary<CountryType, IStorageAccount> _mockStorageAccounts;
        private Mock<IStorageAccount> _mockStorageAccount;
        private StudentGeneralInfoModel _mockStudentGeneralInfo;
        private string _mockImageServer;
        private string _mockAvatarDirectory;

        private readonly string _storageUrl;

        public AvatarServiceUnitTests()
        {
            _mockStorageAccount = new Mock<IStorageAccount>();
            _storageUrl = "https://test-storage.com/";
        }

        [TestInitialize]
        public void Test_Init()
        {
            _mockStorageAccounts = new Dictionary<CountryType, IStorageAccount>
                {
                    { CountryType.US, _mockStorageAccount.Object }
                };
            _mockStudentGeneralInfo = new StudentGeneralInfoModel
            {
                UserAccountId = 1234,
                AvatarFileName = "avatar.jpg",
                SchoolCountryType = CountryType.US
            };
            _mockImageServer = "https://test-image-server.com";
            _mockAvatarDirectory = "avatar";
        }

        [TestMethod]
        [TestCategory("Avatar Service")]
        public void GetStudentAvatarDefaultUrl_should_return_default_avatar_url()
        {
            // Arrange:

            // Act:
            var result = CreateService().GetStudentAvatarDefaultUrl();

            // Assert:
            Assert.AreEqual($"{_mockImageServer}/{_mockAvatarDirectory}/CamsStudentDefault.png", result);
        }

        [TestMethod]
        [TestCategory("Avatar Service")]
        public void GetStudentAvatarUrl_should_set_avatar_url()
        {
            // Arrange:
            Setup();

            // Act:
            var result = CreateService().GetStudentAvatarUrl(_mockStudentGeneralInfo);

            // Assert:
            Assert.AreEqual($"{_storageUrl}public-{_mockStudentGeneralInfo.UserAccountId}/{_mockStudentGeneralInfo.AvatarFileName}", result);
        }

        private void Setup()
        {
            _mockStorageAccount.Setup(x => x.GetBaseUrl()).Returns(_storageUrl);
        }

        private AvatarService CreateService()
        {
            return new AvatarService(_mockStorageAccounts, _mockImageServer, _mockAvatarDirectory);
        }
    }
}
