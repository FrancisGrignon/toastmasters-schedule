namespace Meetings.API.Services
{
    public interface IApiKeysService
    {
        bool Validate(string apiKey);
    }
}