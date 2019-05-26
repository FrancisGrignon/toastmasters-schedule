using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Reminders.FunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminders.FunctionApp
{
    public class MeetingClient
    {
        public const string API_KEY_HEADER_NAME = "x-api-key";

        private readonly string _baseUrl;
        private readonly string _apiKey;

        public MeetingClient(IConfiguration config)
        {
            _baseUrl = config["MeetingServiceUri"];
            _apiKey = config["MeetingServiceApiKey"];
        }

        public Task<Meeting> GetUpcoming()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings/upcoming")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<Meeting>();
        }

        public Task<List<Meeting>> GetPlanning()
        {
            return _baseUrl
                .AppendPathSegment("v1/meetings/planning")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Meeting>>();
        }
    }
}
