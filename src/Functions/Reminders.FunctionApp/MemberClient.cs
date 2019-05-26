using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Reminders.FunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reminders.FunctionApp
{
    public class MemberClient
    {
        public const string API_KEY_HEADER_NAME = "x-api-key";

        private readonly string _baseUrl;
        private readonly string _apiKey;

        public MemberClient(IConfiguration config)
        {
            _baseUrl = config["MemberServiceUri"];
            _apiKey = config["MemberServiceApiKey"];
        }

        public Task<List<Member>> GetAll()
        {
            return _baseUrl
                .AppendPathSegment("members")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Member>>();
        }

        public Task<Member> Get(int id)
        {
            return _baseUrl
                .AppendPathSegment($"members/{id}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<Member>();
        }
    }
}
