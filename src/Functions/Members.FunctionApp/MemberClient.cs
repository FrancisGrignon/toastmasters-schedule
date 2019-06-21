using Flurl;
using Flurl.Http;
using Members.FunctionApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Members.FunctionApp
{
    public class MemberClient
    {
        public const string API_KEY_HEADER_NAME = "x-api-key";

        private readonly string _baseUrl;
        private readonly string _apiKey;

        private readonly ILogger _logger;

        public MemberClient(IConfiguration config, ILogger logger)
        {
            _baseUrl = config["MemberServiceUri"];
            _apiKey = config["MemberServiceApiKey"];

            _logger = logger;
        }

        public Task<List<Member>> GetByToastmastersId(int toastmastersId)
        {
            return _baseUrl
                .AppendPathSegment($"members")
                .SetQueryParam("toastmastersId", toastmastersId)
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Member>>();
        }

        public Task<List<Member>> GetByEmail(string email)
        {
            return _baseUrl
                .AppendPathSegment($"members")
                .SetQueryParam("email", email)
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .GetJsonAsync<List<Member>>();
        }

        public Task<HttpResponseMessage> Create(Member member)
        {
            return _baseUrl
                .AppendPathSegment($"members")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PostJsonAsync(member);
        }

        public Task<HttpResponseMessage> Update(Member member)
        {
            return _baseUrl
                .AppendPathSegment($"members/{member.Id}")
                .WithHeader(API_KEY_HEADER_NAME, _apiKey)
                .PutJsonAsync(member);
        }

        public async Task<T> Invoke<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return default(T);
        }
    }
}
