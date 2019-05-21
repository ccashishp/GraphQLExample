using ApplicationPlanner.Transcripts.Core.Models;
using CC.Common.Enum;
using CC.Storage;
using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Web.Services
{
    public interface IAvatarService
    {
        string GetStudentAvatarDefaultUrl();
        string GetStudentAvatarUrl(IAvatarDetail avatarDetail);
    }

    public class AvatarService: IAvatarService
    {
        private IDictionary<CountryType, IStorageAccount> _storageAccounts { get; }
        private readonly string _avatarBaseUrl;

        public AvatarService(IDictionary<CountryType, IStorageAccount> storageAccounts, string imageServerUrl, string avatarDirectory)
        {
            _storageAccounts = storageAccounts;
            _avatarBaseUrl = $"{imageServerUrl}/{avatarDirectory}";
        }

        public string GetStudentAvatarDefaultUrl()
        {
            return $"{_avatarBaseUrl}/CamsStudentDefault.png"; // @TODO: Copied from CAMS3.API but should probably be store in a config
        }

        public string GetStudentAvatarUrl(IAvatarDetail avatarDetail)
        {
            var baseUri = _storageAccounts[avatarDetail.SchoolCountryType].GetBaseUrl();
            var avatarUri = baseUri + "public-" + avatarDetail.UserAccountId + "/" + avatarDetail.AvatarFileName;
            return avatarUri;
        }
    }
}
