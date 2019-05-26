namespace Members.API.Services
{
    public interface IApiKeysService
    {
        bool Validate(string apiKey);

        bool HasApiKey();
    }
}