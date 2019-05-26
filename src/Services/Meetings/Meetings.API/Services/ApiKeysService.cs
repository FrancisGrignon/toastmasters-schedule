using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Meetings.API.Services
{
    public class ApiKeysService : IApiKeysService
    {
        private string ApiKey { get; }

        public ApiKeysService(IConfiguration configuration)
        {
            ApiKey = configuration.GetValue<string>("ApiKey");
        }

        public bool Validate(string apiKey)
        {
            return ApiKey.Equals(apiKey);
        }
    }
}
