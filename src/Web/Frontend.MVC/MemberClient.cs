using Flurl;
using Flurl.Http;
using Frontend.MVC.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Frontend.MVC
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

        public async Task<bool> Exists(string email)
        {
            return await _baseUrl
                .AppendPathSegment($"members/exists")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PostJsonAsync(new
                {
                    email
                })
                .ReceiveJson<bool>();
        }
    }
}
