namespace MembersV6.API.Services
{
    public interface IApiKeysService
    {
        bool Validate(string apiKey);

        bool HasApiKey();
    }
}