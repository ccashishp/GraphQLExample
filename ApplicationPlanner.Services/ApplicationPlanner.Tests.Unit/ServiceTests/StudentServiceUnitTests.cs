using ApplicationPlanner.Transcripts.Core.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Common.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class StudentServiceUnitTests
    {
        private StudentGeneralInfoModel _mockStudentGeneralInfo;
        private Mock<IAvatarService> _mockAvatarService;

        private readonly string _avatarUrl;

        public StudentServiceUnitTests()
        {
            _avatarUrl = "https://test-storage.com/avatar.jpg";
        }

        [TestInitialize]
        public void Test_Init()
        {
            _mockStudentGeneralInfo = new StudentGeneralInfoModel
            {
                Id = 1234,
                UserAccountId = 4567,
                GradeNumber = 11,
                FirstName = "Fahad",
                AvatarFileName = "avatar.jpg",
                SchoolCountryType = CountryType.US
            };
            _mockAvatarService = new Mock<IAvatarService>(); 
        }

        [TestMethod]
        [TestCategory("Student Service")]
        public void AuthenticatedStudentGetByStudentGeneralInfo_should_set_avatar_url()
        {
            // Arrange:
            Setup();

            // Act:
            var result = CreateService().AuthenticatedStudentGetByStudentGeneralInfo(_mockStudentGeneralInfo);

            // Assert:
            Assert.AreEqual(_avatarUrl, result.AvatarUrl);
        }

        [TestMethod]
        [TestCategory("Student Service")]
        public void AuthenticatedStudentGetByStudentGeneralInfo_should_set_hasAccessToTranscripts()
        {
            // Arrange:
            Setup();

            // Act:
            var result = CreateService().AuthenticatedStudentGetByStudentGeneralInfo(_mockStudentGeneralInfo);

            // Assert:
            Assert.AreEqual(true, result.HasAccessToTranscripts);
        }

        private void Setup()
        {
            _mockAvatarService.Setup(x => x.GetStudentAvatarUrl(It.IsAny<IAvatarDetail>())).Returns(_avatarUrl);
        }

        private StudentService CreateService()
        {
            return new StudentService(_mockAvatarService.Object);
        }
    }
}
